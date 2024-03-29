﻿using Hzdtf.Logger.Contract;
using Hzdtf.Utility;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hzdtf.Service.Impl
{
    /// <summary>
    /// 基本服务基类
    /// @ 黄振东
    /// </summary>
    public abstract class BasicServiceBase
    {
        #region 属性与字段

        /// <summary>
        /// 日志
        /// </summary>
        protected readonly ILogable log;

        /// <summary>
        /// 配置
        /// </summary>
        protected IConfiguration Config
        {
            get => App.CurrConfig;
        }

        #endregion

        #region 初始化

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="log">日志</param>
        public BasicServiceBase(ILogable log = null)
        {
            if (log == null)
            {
                log = LogTool.DefaultLog;
            }
            this.log = log;
        }

        #endregion
    }
}
