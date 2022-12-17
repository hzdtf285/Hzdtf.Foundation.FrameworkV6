using Hzdtf.Logger.Contract;
using Hzdtf.Service.Contract;
using Hzdtf.Utility;
using Hzdtf.Utility.Factory;
using Hzdtf.Utility.Localization;
using Hzdtf.Utility.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace Hzdtf.BasicController
{
    /// <summary>
    /// 基本控制器基类
    /// </summary>
    /// <typeparam name="IdT">Id类型</typeparam>
    /// <typeparam name="ModelT">模型类型</typeparam>
    /// <typeparam name="ServiceT">服务类型</typeparam>
    public abstract class BasicControllerBase<IdT, ModelT, ServiceT> : ControllerBase
        where ModelT : SimpleInfo<IdT>
        where ServiceT : IService<IdT, ModelT>
    {
        /// <summary>
        /// 日志
        /// </summary>
        protected readonly ILogable log;

        /// <summary>
        /// 配置
        /// </summary>
        protected IConfiguration config
        {
            get => App.CurrConfig;
        }

        /// <summary>
        /// 服务
        /// </summary>
        protected readonly ServiceT service;

        /// <summary>
        /// 本地化
        /// </summary>
        protected readonly ILocalization localize;

        /// <summary>
        /// 通用数据工厂
        /// </summary>
        protected readonly ISimpleFactory<HttpContext, CommonUseData> comUseDataFactory;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="log">日志</param>
        /// <param name="service">配置</param>
        /// <param name="localize">服务</param>
        /// <param name="comUseDataFactory">本地化</param>
        public BasicControllerBase(ILogable log = null, ServiceT service = default(ServiceT), ILocalization localize = null, ISimpleFactory<HttpContext, CommonUseData> comUseDataFactory = null)
        {
            if (log == null)
            {
                log = LogTool.DefaultLog;
            }
            this.log = log;
            this.service = service;
            this.localize = localize;
            this.comUseDataFactory = comUseDataFactory;
        }
    }
}
