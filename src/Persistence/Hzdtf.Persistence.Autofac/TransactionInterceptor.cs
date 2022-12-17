using Castle.DynamicProxy;
using FoxUC.Persistence.Contract.Basic;
using FoxUC.Utility.Attr;
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using FoxUC.Utility.Data;
using FoxUC.Utility.Model.Return;
using FoxUC.Utility.Model;
using FoxUC.Utility.Utils;
using System.Reflection;
using FoxUC.Utility.Intercepteds;
using FoxUC.Utility;

namespace FoxUC.Persistence.Autofac
{
    /// <summary>
    /// 事务拦截器
    /// connectionId是关键点，引用方法必须要指定该参数的索引位置
    /// 此拦截器会根据索引位置获取到connectionId，如果之前有设置值，则在本拦截器里不会开启新的事务
    /// 开启了新事务后，会执行业务方法会把新创建的connectionId传入到业务方法对应参数里
    /// 如果业务方法里有抛出异常或返回值为ReturnInfo.Code失败，则会回滚
    /// @ 网狐
    /// </summary>
    public class TransactionInterceptor : AttrInterceptorBase<TransactionAttribute>
    {
        /// <summary>
        /// 事务执行前方法缓存
        /// </summary>
        private readonly static TransactionBeforeMethodCache beforeMethodCache = new TransactionBeforeMethodCache();

        /// <summary>
        /// 拦截
        /// </summary>
        /// <param name="basicReturn">基本返回</param>
        /// <param name="invocation">拦截参数</param>
        /// <param name="attr">特性</param>
        /// <param name="isExecProceeded">是否已执行</param>
        protected override void Intercept(BasicReturnInfo basicReturn, IInvocation invocation, TransactionAttribute attr, out bool isExecProceeded)
        {
            isExecProceeded = true;
            BasicReturnInfo returnInfo = new BasicReturnInfo();
            object connId = null;
            if (attr.ConnectionIdIndex != -1)
            {
                connId = invocation.GetArgumentValue(attr.ConnectionIdIndex);
            }
            IGetObject<IPersistenceConnection> getPerConn = App.GetServiceFromInstance(invocation.TargetType) as IGetObject<IPersistenceConnection>;
            if (getPerConn == null)
            {
                basicReturn.SetFailureMsg("未实现IGetObject<IPersistenceConnection>接口");
                return;
            }

            IPersistenceConnection perConn = getPerConn.Get();
            string connectionId = null;
            // 当有连接ID传过来，判断是否存在该连接事务，存在则不开启新事务
            if (connId != null)
            {
                string connIdStr = connId.ToString();
                if (perConn.GetDbTransaction(connIdStr) != null)
                {
                    ExecBeforeTransaction(invocation, attr);
                    invocation.Proceed();
                    return;
                }

                connectionId = connIdStr;
            }
            else
            {
                connectionId = perConn.NewConnectionId();
            }

            // 如果需要通用数据参数
            CommonUseData comUseData = null;
            if (attr.CommonUseDataIndex != -1)
            {
                var comData = invocation.GetArgumentValue(attr.CommonUseDataIndex);
                if (comData == null)
                {
                    comUseData = new CommonUseData();
                    invocation.SetArgumentValue(attr.CommonUseDataIndex, comUseData);
                }
                if (comData is CommonUseData)
                {
                    comUseData = comData as CommonUseData;
                }
                else
                {
                    throw new ArgumentException($"参数位置[{attr.CommonUseDataIndex}]不是CommonUseData类型");
                }
            }

            IDbTransaction dbTransaction = null;
            var isNotTransFinish = true; // 事务是否未完成
            invocation.SetArgumentValue(attr.ConnectionIdIndex, connectionId);
            try
            {
                // 开启事务前先执行动作
                ExecBeforeTransaction(invocation, attr);

                dbTransaction = perConn.BeginTransaction(connectionId, attr);
                invocation.Proceed();

                var re = GetExecedReturn(invocation);
                // 如果返回值为失败标识，也回滚
                if (re != null && re.Failure())
                {
                    dbTransaction.Rollback();

                    return;
                }

                dbTransaction.Commit();
                isNotTransFinish = false;

                ExecSuccessCallback(comUseData);
            }
            catch (Exception ex)
            {
                if (dbTransaction != null && isNotTransFinish)
                {
                    dbTransaction.Rollback();
                }
                throw new Exception(ex.Message, ex);
            }
            finally
            {
                perConn.Release(connectionId);
            }
        }

        /// <summary>
        /// 获取执行完的返回对象
        /// </summary>
        /// <param name="invocation">拦截参数</param>
        /// <returns>返回对象</returns>
        private BasicReturnInfo GetExecedReturn(IInvocation invocation)
        {
            var returnType = invocation.Method.ReturnType;
            if (invocation.Method.ReturnType.IsReturnType() && invocation.ReturnValue is BasicReturnInfo)
            {
                return invocation.ReturnValue as BasicReturnInfo;
            }

            return null;
        }

        /// <summary>
        /// 执行成功后的回调
        /// </summary>
        /// <param name="comData">通用数据</param>
        private void ExecSuccessCallback(CommonUseData comData)
        {
            if (comData == null || comData.Callbacks.IsNullOrCount0())
            {
                return;
            }

            foreach (var item in comData.Callbacks)
            {
                item.Key(item.Value);
            }

            comData.ClearCallback();
        }

        /// <summary>
        /// 执行开启事务前
        /// 执行前必须先设置好连接ID
        /// </summary>
        /// <param name="invocation">拦截参数</param>
        /// <param name="attr">特性</param>
        private void ExecBeforeTransaction(IInvocation invocation, TransactionAttribute attr)
        {
            if (string.IsNullOrWhiteSpace(attr.BeforeMethod))
            {
                return;
            }

            MethodInfo method = null;
            // 如果使用缓存，则先从缓存里获取映射方法
            if (attr.BeforeMethodUseCache)
            {
                var key = $"{invocation.TargetType.FullName}.{attr.BeforeMethod}";
                if (beforeMethodCache.Exists(key))
                {
                    method = beforeMethodCache.Get(key);
                }
                else
                {
                    method = invocation.TargetType.GetMethod(attr.BeforeMethod);
                    beforeMethodCache.Add(key, method);
                }
            }
            else
            {
                method = invocation.TargetType.GetMethod(attr.BeforeMethod);
            }

            // 不需要返回值
            if (attr.BeforeMethodReturnValueInIndex == -1)
            {
                method.Invoke(invocation.InvocationTarget, invocation.Arguments);
            }
            else
            {
                var result = method.Invoke(invocation.InvocationTarget, invocation.Arguments);
                invocation.SetArgumentValue(attr.BeforeMethodReturnValueInIndex, result);
            }
        }
    }
}
