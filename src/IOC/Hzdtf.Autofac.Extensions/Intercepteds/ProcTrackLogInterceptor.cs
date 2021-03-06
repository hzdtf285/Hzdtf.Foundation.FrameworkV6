using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Hzdtf.Utility.Attr;
using System.Diagnostics;
using Hzdtf.Logger.Contract;
using Hzdtf.Autofac.Extensions;
using Newtonsoft.Json;

namespace Hzdtf.Autofac.Extensions.Intercepteds
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
        private static ILogable log;

        /// <summary>
        /// 日志
        /// </summary>
        private static ILogable Log
        {
            get
            {
                if (log == null)
                {
                    ILogable tempLog = AutofacTool.Resolve<ILogable>();
                    if (tempLog == null)
                    {
                        lock (syncLog)
                        {
                            log = LogTool.DefaultLog;
                        }
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

            Log.DebugAsync(logMsg.ToString(), null, invocation.TargetType.Name);
        }
    }
}
