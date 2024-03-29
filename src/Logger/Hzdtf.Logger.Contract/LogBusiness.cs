﻿using Hzdtf.Utility;
using Hzdtf.Utility.Attr;
using Hzdtf.Utility.Localization;
using Hzdtf.Utility.Model.Return;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hzdtf.Logger.Contract
{
    /// <summary>
    /// 日志业务
    /// @ 黄振东
    /// </summary>
    [Inject]
    public class LogBusiness : ILogBusiness
    {
        /// <summary>
        /// 日志
        /// </summary>
        protected readonly ILogable log;

        /// <summary>
        /// 本地化
        /// </summary>
        protected readonly ILocalization localize;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="log">日志</param>
        /// <param name="localize">本地化</param>
        public LogBusiness(ILogable log = null, ILocalization localize = null)
        {
            if (log == null)
            {
                this.log = LogTool.DefaultLog;
            }
            else
            {
                this.log = log;
            }

            this.localize = localize;
        }

        /// <summary>
        /// 捕获日志执行
        /// 当发生异常时，会记录异常日志，同时触发异常回调
        /// </summary>
        /// <param name="action">执行核心</param>
        /// <param name="exceptionCallback">异常回调</param>
        /// <param name="eventId">事件ID</param>
        /// <param name="logTags">日志标签</param>
        /// <returns>返回类型</returns>
        public void TryLogExec(Action action, Action<BasicReturnInfo> exceptionCallback = null, string eventId = null, params string[] logTags)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                log.ErrorAsync(ex.Message, ex, source: "LogBusiness", eventId: eventId, tags: logTags);

                var re = new BasicReturnInfo();
                var msg = localize != null ? localize.Get(CommonCodeDefine.OPER_FAILURE_KEY, "操作失败") : "操作失败";
                re.SetFailureMsg("操作失败", ex.Message, ex);

                exceptionCallback(re);
            }
        }

        /// <summary>
        /// 捕获日志执行
        /// 当发生异常时，会记录异常日志，同时触发异常回调
        /// </summary>
        /// <typeparam name="ReturnT">返回类型</typeparam>
        /// <param name="func">执行核心</param>
        /// <param name="exceptionCallback">异常回调</param>
        /// <param name="eventId">事件ID</param>
        /// <param name="logTags">日志标签</param>
        /// <returns>返回类型</returns>
        public ReturnT TryLogExec<ReturnT>(Func<ReturnT> func, Func<BasicReturnInfo, ReturnT> exceptionCallback = null, string eventId = null, params string[] logTags)
        {
            try
            {
                return func();
            }
            catch (Exception ex)
            {
                log.ErrorAsync(ex.Message, ex, source: "LogBusiness", eventId: eventId, tags: logTags);

                var re = new BasicReturnInfo();
                var msg = localize != null ? localize.Get(CommonCodeDefine.OPER_FAILURE_KEY, "操作失败") : "操作失败";
                re.SetFailureMsg("操作失败", ex.Message, ex);

                return exceptionCallback(re);
            }
        }
    }
}
