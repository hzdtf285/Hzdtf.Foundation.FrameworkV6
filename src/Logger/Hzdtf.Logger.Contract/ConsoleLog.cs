using Hzdtf.Utility.Attr;
using Hzdtf.Utility.TheOperation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hzdtf.Logger.Contract
{
    /// <summary>
    /// 控制台日志
    /// @ 黄振东
    /// </summary>
    [Inject]
    public class ConsoleLog : ContentLogBase
    {
        #region 初始化

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="logRecordLevel">日志记录级别</param>
        /// <param name="theOperation">本地操作</param>
        public ConsoleLog(ILogRecordLevel logRecordLevel = null, ITheOperation theOperation = null)
            : base(logRecordLevel, theOperation)
        {
        }

        #endregion

        #region 重写父类的方法

        /// <summary>
        /// 将日志内容写入到存储设备里
        /// </summary>
        /// <param name="logContent">日志内容</param>
        /// <param name="level">等级</param>
        protected override void WriteStorage(string logContent, string level)
        {
            Console.WriteLine(logContent);
        }

        /// <summary>
        /// 分段分隔符
        /// </summary>
        /// <returns>分段分隔符</returns>
        protected override string SectionPartitionSymbol() => Environment.NewLine;

        #endregion
    }
}
