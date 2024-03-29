﻿using Exceptionless;
using Exceptionless.Logging;
using Hzdtf.Logger.Contract;
using System;
using Hzdtf.Utility.Attr;
using Hzdtf.Utility.TheOperation;

namespace Hzdtf.Logger.Exceptionless
{
    /// <summary>
    /// Exceptionless分布式日志
    /// @ 黄振东
    /// </summary>
    [Inject]
    public class ExceptionlessLog : LogBase
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="logRecordLevel">日志记录级别</param>
        /// <param name="theOperation">本地操作</param>
        public ExceptionlessLog(ILogRecordLevel logRecordLevel = null, ITheOperation theOperation = null)
            : base(logRecordLevel, theOperation)
        {
        }

        /// <summary>
        /// 静态构造方法
        /// </summary>
        static ExceptionlessLog()
        {
            ExceptionlessTool.DefaultInit();
        }

        /// <summary>
        /// 将消息与异常写入到存储设备里
        /// </summary>
        /// <param name="level">级别</param>
        /// <param name="msg">消息</param>
        /// <param name="eventId">事件ID</param>
        /// <param name="ex">异常</param>
        /// <param name="source">来源</param>
        /// <param name="tags">标签</param>
        protected override void WriteStorage(string level, string msg, string eventId, Exception ex = null, string source = null, params string[] tags)
        {
            if (string.IsNullOrWhiteSpace(source) && ex != null)
            {
                source = ex.Source;
            }
            var logLevel = LogLevelHelper.Parse(level);
            LogLevel exLevel = null;
            switch (logLevel)
            {
                case LogLevelEnum.TRACE:
                    exLevel = LogLevel.Trace;

                    break;

                case LogLevelEnum.DEBUG:
                    exLevel = LogLevel.Debug;

                    break;

                case LogLevelEnum.INFO:
                    exLevel = LogLevel.Info;

                    break;

                case LogLevelEnum.WRAN:
                    exLevel = LogLevel.Warn;

                    break;

                case LogLevelEnum.ERROR:
                    exLevel = LogLevel.Error;

                    break;

                case LogLevelEnum.FATAL:
                    exLevel = LogLevel.Fatal;

                    break;

                default:

                    return;
            }

            EventBuilder builder = null;
            if (ex == null)
            {
                builder = ExceptionlessClient.Default.CreateLog(source, msg, exLevel);
            }
            else
            {
                builder = ExceptionlessClient.Default.CreateException(ex);
                if (!string.IsNullOrWhiteSpace(source))
                {
                    builder.SetSource(source);
                }
                if (!string.IsNullOrWhiteSpace(msg))
                {
                    builder.SetMessage(msg);
                }
            }
            builder.AddTags(AppendLocalIdTags(eventId, tags));
            builder.Submit();
        }
    }
}
