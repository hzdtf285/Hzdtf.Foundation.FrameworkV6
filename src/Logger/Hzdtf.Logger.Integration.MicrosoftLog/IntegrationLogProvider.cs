using Hzdtf.Logger.Contract;
using Hzdtf.Logger.Text.Integration.MicrosoftLog;
using Hzdtf.Utility;
using Hzdtf.Utility.Attr;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hzdtf.Logger.Integration.MicrosoftLog
{
    /// <summary>
    /// 集成日志提供者
    /// @ 黄振东
    /// </summary>
    [Inject]
    public class IntegrationLogProvider : ILoggerProvider
    {
        /// <summary>
        /// 原生日志
        /// </summary>
        protected ILogable protoLog;

        /// <summary>
        /// 日志记录级别
        /// </summary>
        protected ILogRecordLevel logRecordLevel;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="protoLog">原生日志</param>
        /// <param name="logRecordLevel">日志记录级别</param>
        public IntegrationLogProvider(ILogable protoLog = null, ILogRecordLevel logRecordLevel = null)
        {
            if (protoLog == null)
            {
                this.protoLog = LogTool.DefaultLog;
            }
            else
            {
                this.protoLog = protoLog;
            }
            if (logRecordLevel == null)
            {
                this.logRecordLevel = new DefaultLogRecordLevel();
            }
            else
            {
                this.logRecordLevel = logRecordLevel;
            }
        }

        /// <summary>
        /// 创建日志
        /// </summary>
        /// <param name="categoryName">分类名称</param>
        /// <returns>日志</returns>
        public ILogger CreateLogger(string categoryName)
        {
            if (protoLog == null)
            {
                protoLog = App.GetServiceFromInstance<ILogable>();
            }
            if (logRecordLevel == null)
            {
                logRecordLevel = App.GetServiceFromInstance<ILogRecordLevel>();
            }

            return new IntegrationLog(categoryName, protoLog, logRecordLevel);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
        }
    }
}
