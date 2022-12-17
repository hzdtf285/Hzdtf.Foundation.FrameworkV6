using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Hzdtf.Utility.Model.Return;
using Hzdtf.Utility.Attr;
using Hzdtf.Utility.Localization;
using Microsoft.Extensions.Logging;

namespace Hzdtf.Utility.Intercepteds
{
    /// <summary>
    /// 捕获异常拦截器，方法的返回值必须是BasicReturnInfo或BasicReturnInfo子类，否则会抛出异常
    /// @ 黄振东
    /// </summary>
    public class TryExceptionInterceptor : IInterceptor
    {
        /// <summary>
        /// 同步日志
        /// </summary>
        private static readonly object syncLog = new object();

        /// <summary>
        /// 日志
        /// </summary>
        private static ILogger<TryExceptionInterceptor> log;

        /// <summary>
        /// 日志
        /// </summary>
        private static ILogger<TryExceptionInterceptor> Log
        {
            get
            {
                if (log == null)
                {
                    var tempLog = App.GetServiceFromInstance<ILogger<TryExceptionInterceptor>>();
                    if (tempLog == null)
                    {
                        throw new ArgumentNullException("没有找到任何日志实现类");
                    }
                    else
                    {
                        lock (syncLog)
                        {
                            log = tempLog;
                        }
                    }
                }

                return log;
            }
        }

        /// <summary>
        /// 同步本地化
        /// </summary>
        private static readonly object syncLocalize = new object();

        /// <summary>
        /// 本地化
        /// </summary>
        private static ILocalization localize;

        /// <summary>
        /// 本地化
        /// </summary>
        private static ILocalization Localize
        {
            get
            {
                if (localize == null)
                {
                    var locali = App.GetServiceFromInstance<ILocalization>();
                    if (locali != null)
                    {
                        lock (syncLocalize)
                        {
                            localize = locali;
                        }
                    }
                }

                return localize;
            }
        }

        /// <summary>
        /// 拦截
        /// </summary>
        /// <param name="invocation">拦截参数</param>
        public void Intercept(IInvocation invocation)
        {
            var attr = invocation.Method.GetAttribute<NotTryExceptionAttribute>();
            if (attr != null)
            {
                invocation.Proceed();
                return;
            }

            try
            {
                invocation.Proceed();
                FilterReturnValue(invocation, null);
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message, ex, invocation.Method.Name);
                if (FilterReturnValue(invocation, ex))
                {
                    return;
                }

                throw new Exception(ex.Message, ex);
            }
        }

        /// <summary>
        /// 过滤返回值
        /// </summary>
        /// <param name="invocation">拦截参数</param>
        /// <param name="ex"></param>
        /// <returns>是否已过滤</returns>
        private bool FilterReturnValue(IInvocation invocation, Exception ex)
        {
            if (invocation.Method.ReturnType.IsReturnType())
            {
                if (ex == null)
                {
                    BasicReturnInfo basicReturnInfo;
                    if (invocation.ReturnValue != null)
                    {
                        basicReturnInfo = invocation.ReturnValue as BasicReturnInfo;
                    }
                    else
                    {
                        basicReturnInfo = invocation.Method.ReturnType.CreateInstance<BasicReturnInfo>();
                        invocation.ReturnValue = basicReturnInfo;
                    }
                    if (string.IsNullOrWhiteSpace(basicReturnInfo.Msg))
                    {
                        if (basicReturnInfo.Success())
                        {
                            var msg = Localize != null ? Localize.Get(CommonCodeDefine.OPER_SUCCESS_KEY, "操作成功") : "操作成功";
                            basicReturnInfo.SetSuccessMsg(msg);
                        }
                        else
                        {
                            var msg = Localize != null ? Localize.Get(CommonCodeDefine.OPER_FAILURE_KEY, "操作失败") : "操作失败";
                            basicReturnInfo.SetSuccessMsg(msg);
                        }
                    }
                }
                else
                {
                    var msg = Localize != null ? Localize.Get(CommonCodeDefine.SYSTEM_EXCEPTION_KEY, "操作异常，请联系管理员") : "操作异常，请联系管理员";
                    BasicReturnInfo basicReturnInfo = invocation.Method.ReturnType.CreateInstance<BasicReturnInfo>();
                    basicReturnInfo.SetFailureMsg(msg, ex.Message, ex);
                    invocation.ReturnValue = basicReturnInfo;
                }

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
