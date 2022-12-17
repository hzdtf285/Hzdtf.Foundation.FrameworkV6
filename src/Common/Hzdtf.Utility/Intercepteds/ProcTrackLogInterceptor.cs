using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Hzdtf.Utility.Attr;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Hzdtf.Utility.Intercepteds
{
    /// <summary>
    /// 执行过程轨迹日志拦截器
    /// @ 黄振东
    /// </summary>
    public class ProcTrackLogInterceptor : IInterceptor
    {
        /// <summary>
        /// 同步日志
        /// </summary>
        private static readonly object syncLog = new object();

        /// <summary>
        /// 日志
        /// </summary>
        private static ILogger<ProcTrackLogInterceptor> log;

        /// <summary>
        /// 日志
        /// </summary>
        private static ILogger<ProcTrackLogInterceptor> Log
        {
            get
            {
                if (log == null)
                {
                    var tempLog = App.GetServiceFromInstance<ILogger<ProcTrackLogInterceptor>>();
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
        /// 拦截
        /// </summary>
        /// <param name="invocation">拦截参数</param>
        public void Intercept(IInvocation invocation)
        {
            var attr = invocation.Method.GetAttribute<ProcTrackLogAttribute>();
            string paraLog = null;
            if (attr != null)
            {
                if (attr.ExecProc)
                {
                    if (!attr.IgnoreParamValues)
                    {
                        paraLog = $",params:{ invocation.Arguments.ToJsonString()}";
                    }
                }
                else
                {
                    invocation.Proceed();

                    return;
                }
            }
            else
            {
                paraLog = $",params:{ invocation.Arguments.ToJsonString()}";
            }

            var watch = Stopwatch.StartNew();
            watch.Start();
            StringBuilder logMsg = new StringBuilder($"{invocation.TargetType.FullName} {invocation.Method}{paraLog}");

            invocation.Proceed();

            string returnValLog = null;
            if ((attr == null || !attr.IgnoreParamReturn) && invocation.ReturnValue != null && !invocation.Method.ReturnType.IsTypeTask())
            {
                returnValLog = $"ReturnValue:{invocation.ReturnValue.ToJsonString()},";
            }
            
            watch.Stop();
            logMsg.AppendFormat(",{0}timed:{1}ms", returnValLog, watch.ElapsedMilliseconds);

            Log.LogTrace(logMsg.ToString(), null, invocation.TargetType.Name);
        }
    }
}
