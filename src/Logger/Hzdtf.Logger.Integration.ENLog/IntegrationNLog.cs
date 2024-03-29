﻿using Hzdtf.Logger.Contract;
using Hzdtf.Utility.Attr;
using Hzdtf.Utility.TheOperation;
using System;

namespace Hzdtf.Logger.Integration.ENLog
{
    /// <summary>
    /// 集成NLog
    /// 需要在应用程序根目录下创建nlog.config配置文件
    /// @ 黄振东
    /// </summary>
    [Inject]
    public class IntegrationNLog : LogBase
    {
        /// <summary>
        /// NLog
        /// </summary>
        private readonly NLog.Logger nlog;

        /// <summary>
        /// 名称
        /// </summary>
        private readonly string name;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="logRecordLevel">日志记录级别</param>
        /// <param name="theOperation">本地操作</param>
        public IntegrationNLog(string name = null, ILogRecordLevel logRecordLevel = null, ITheOperation theOperation = null)
            : base(logRecordLevel, theOperation)
        {
            if (name == null)
            {
                this.name = typeof(IntegrationNLog).Name;
            }
            else
            {
                this.name = name;
            }
            nlog = NLog.LogManager.GetLogger(this.name);
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
            var logger = string.IsNullOrWhiteSpace(source) || name.Equals(source) ? nlog : NLog.LogManager.GetLogger(source);
            msg += " 标签:" + string.Join(",", AppendLocalIdTags(eventId, tags));

            var levelEnum = LogLevelHelper.Parse(level);
            switch (levelEnum)
            {
                case LogLevelEnum.TRACE:
                    logger.Trace(ex, msg);

                    break;

                case LogLevelEnum.DEBUG:
                    logger.Debug(ex, msg);

                    break;

                case LogLevelEnum.INFO:
                    logger.Info(ex, msg);

                    break;

                case LogLevelEnum.WRAN:
                    logger.Warn(ex, msg);

                    break;

                case LogLevelEnum.ERROR:
                    logger.Error(ex, msg);

                    break;

                case LogLevelEnum.FATAL:
                    logger.Fatal(ex, msg);

                    break;
            }
        }
    }
}
