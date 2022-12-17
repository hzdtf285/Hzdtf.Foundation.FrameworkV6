using Hzdtf.Utility.Model;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Hzdtf.Utility.ProcessCall;
using Hzdtf.Utility.Attr;

namespace Hzdtf.Utility.Proxy
{
    /// <summary>
    /// RPC动态代理客户端
    /// @ 黄振东
    /// </summary>
    /// <typeparam name="SubClassT">子类类型</typeparam>
    [Inject]
    public class RpcDispatchProxyClient<SubClassT> : BusinessDispatchProxyBase<SubClassT>
        where SubClassT : BusinessDispatchProxyBase<SubClassT>
    {
        /// <summary>
        /// Rpc客户端方法
        /// </summary>
        protected IRpcClientMethod rpcClientMethod;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="rpcClientMethod">Rpc客户端方法</param>
        public RpcDispatchProxyClient(IRpcClientMethod rpcClientMethod = null)
        {
            this.rpcClientMethod = rpcClientMethod;
        }

        /// <summary>
        /// 执行业务
        /// </summary>
        /// <param name="targetMethod">目标方法</param>
        /// <param name="args">方法参数数组</param>
        /// <returns>业务返回值</returns>
        protected override object InvokeBusiness(MethodInfo targetMethod, object[] args)
        {
            if (rpcClientMethod == null)
            {
                rpcClientMethod = CreateRpcClientMethod();
                if (rpcClientMethod == null)
                {
                    throw new NullReferenceException("找不到Rpc客户端方法对象");
                }
            }
            var rpcData = new RpcDataInfo()
            {
                MethodFullPath = string.Format("{0},{1}.{2}", targetMethod.ReflectedType.Assembly.GetName().Name, targetMethod.ReflectedType.FullName, targetMethod.Name),
                MethodParams = args
            };

            return rpcClientMethod.Call(targetMethod, rpcData);
        }

        /// <summary>
        /// 创建默认Rpc客户端方法
        /// </summary>
        /// <returns>Rpc客户端方法</returns>
        protected virtual IRpcClientMethod CreateRpcClientMethod() => null;
    }

    /// <summary>
    /// RPC动态代理客户端
    /// @ 黄振东
    /// </summary>
    [Inject]
    public class RpcDispatchProxyClient : RpcDispatchProxyClient<RpcDispatchProxyClient>
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="rpcClientMethod">Rpc客户端方法</param>
        public RpcDispatchProxyClient(IRpcClientMethod rpcClientMethod = null)
            : base (rpcClientMethod)
        {
        }
    }
}
