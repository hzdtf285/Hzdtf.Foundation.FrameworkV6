﻿using Hzdtf.Utility.Attr;
using Hzdtf.Utility.Data;
using Hzdtf.Utility.InterfaceImpl;
using Hzdtf.Utility.Model;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Hzdtf.Utility.ProcessCall
{
    /// <summary>
    /// Rpc服务监听
    /// @ 黄振东
    /// </summary>
    [Inject]
    public class RpcServerListen : IRpcServerListen
    {
        /// <summary>
        /// 关闭后事件
        /// </summary>
        public event DataHandler Closed;

        /// <summary>
        /// 字节数组序列化，默认为json序列化
        /// </summary>
        protected readonly IBytesSerialization bytesSerialization;

        /// <summary>
        /// 接口映射实现，默认为InterfaceMapImplCache
        /// </summary>
        protected readonly IInterfaceMapImpl interfaceMapImpl;

        /// <summary>
        /// 方法调用，默认为MethodCallCache
        /// </summary>
        protected readonly IMethodCall methodCall;

        /// <summary>
        /// Rpc服务端
        /// </summary>
        protected readonly IRpcServer rpcServer;

        /// <summary>
        /// 接收中错误事件
        /// </summary>
        public event Action<string, Exception> ReceivingError;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="bytesSerialization">字节数组序列化，默认为json序列化</param>
        /// <param name="interfaceMapImpl">接口映射实现，默认为InterfaceMapImplCache</param>
        /// <param name="methodCall">方法调用，默认为MethodCallCache</param>
        /// <param name="rpcServer">Rpc服务端</param>
        public RpcServerListen(IBytesSerialization bytesSerialization = null, IInterfaceMapImpl interfaceMapImpl = null, IMethodCall methodCall = null, IRpcServer rpcServer = null)
        {
            if (bytesSerialization == null)
            {
                this.bytesSerialization = new JsonBytesSerialization();
            }
            else
            {
                this.bytesSerialization = bytesSerialization;
            }
            if (interfaceMapImpl == null)
            {
                this.interfaceMapImpl = new InterfaceMapImplCache();
            }
            else
            {
                this.interfaceMapImpl = interfaceMapImpl;
            }
            if (methodCall == null)
            {
                this.methodCall = new MethodCallCache();
            }
            else
            {
                this.methodCall = methodCall;
            }

            this.rpcServer = rpcServer;
        }

        /// <summary>
        /// 监听
        /// </summary>
        public void Listen()
        {
            rpcServer.Receive(inData =>
            {
                object result = null;
                try
                {
                    var rpcDataInfo = bytesSerialization.Deserialize<RpcDataInfo>(inData);
                    if (rpcDataInfo == null)
                    {
                        OnReceivingError("传过来的数据不是RpcDataInfo类型的");
                    }
                    else if (string.IsNullOrWhiteSpace(rpcDataInfo.MethodFullPath))
                    {
                        OnReceivingError("方法全路径不能为空");
                    }
                    else
                    {
                        string classFullName;
                        var methodName = ReflectExtensions.GetMethodName(rpcDataInfo.MethodFullPath, out classFullName);

                        var implClassFullName = interfaceMapImpl.Reader(classFullName);

                        MethodInfo method;
                        var methodReturnValue = methodCall.Invoke(string.Format("{0}.{1}", implClassFullName, methodName), out method, rpcDataInfo.MethodParams);

                        // 如果方法返回是Void，则直接返回null
                        if (method.IsMethodReturnVoid())
                        {
                            return null;
                        } // 如果方法是异步方法，则转换为Task并等待执行结束后返回Result给客户端
                        else if (method.ReturnType.IsTypeTask())
                        {
                            // 如果带泛型，则返回任务的Result给客户端
                            if (method.ReturnType.IsTypeGenericityTask())
                            {
                                var resultProperty = method.ReturnType.GetProperty("Result");
                                result = resultProperty.GetValue(methodReturnValue);
                            }
                            else
                            {
                                var methodReturnTask = methodReturnValue as Task;
                                methodReturnTask.Wait();
                            }
                        }
                        else
                        {
                            result = methodReturnValue;
                        }
                    }

                    if (result == null)
                    {
                        return null;
                    }

                    return bytesSerialization.Serialize(result);
                }
                catch (Exception ex)
                {
                    try
                    {
                        OnReceivingError(ex.Message, ex);
                    }
                    catch { }

                    return null;
                }
            });
        }

        /// <summary>
        /// 异步监听
        /// </summary>
        public void ListenAsync()
        {
            Task.Run(() => Listen());
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public virtual void Close()
        {
            rpcServer.Close();

            OnClosed();
        }

        /// <summary>
        /// 执行关闭后事件
        /// </summary>
        protected void OnClosed()
        {
            if (Closed != null)
            {
                Closed(this, new DataEventArgs());
            }
        }

        /// <summary>
        /// 执行接收中错误事件
        /// </summary>
        /// <param name="err">错误消息</param>
        /// <param name="ex">异常</param>
        protected void OnReceivingError(string err, Exception ex = null)
        {
            if (ReceivingError != null)
            {
                ReceivingError(err, ex);
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            Close();
        }
    }
}
