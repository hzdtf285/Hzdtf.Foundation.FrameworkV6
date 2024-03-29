﻿using Hzdtf.Service.Contract;
using Hzdtf.Utility.Model;
using Hzdtf.Utility.Model.Return;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Hzdtf.Logger.Contract;
using Hzdtf.Utility.Factory;
using Hzdtf.Utility.Localization;
using Hzdtf.Utility.RoutePermission;

namespace Hzdtf.BasicController
{
    /// <summary>
    /// 页面数据控制器基类
    /// @ 黄振东
    /// </summary>
    /// <typeparam name="IdT">Id类型</typeparam>
    /// <typeparam name="ModelT">模型类型</typeparam>
    /// <typeparam name="ServiceT">服务类型</typeparam>
    /// <typeparam name="PageInfoT">页面信息类型</typeparam>
    public class PageDataControllerBase<IdT, ModelT, ServiceT, PageInfoT> : BasicControllerBase<IdT, ModelT, ServiceT>
        where ModelT : SimpleInfo<IdT>
        where ServiceT : IService<IdT, ModelT>
        where PageInfoT : PageInfo<IdT>
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="log">日志</param>
        /// <param name="service">配置</param>
        /// <param name="localize">服务</param>
        /// <param name="comUseDataFactory">本地化</param>
        public PageDataControllerBase(ILogable log = null, ServiceT service = default(ServiceT), ILocalization localize = null, ISimpleFactory<HttpContext, CommonUseData> comUseDataFactory = null)
            : base(log, service, localize, comUseDataFactory)
        {
        }

        /// <summary>
        /// 获取页面数据，包含当前用户所拥有的权限功能列表
        /// </summary>
        /// <returns>返回信息</returns>
        [HttpGet("PageData")]
        [ActionPermission(new string[] { "Query" })]
        public virtual ReturnInfo<PageInfoT> PageData()
        {            
            var comData = comUseDataFactory.Create(HttpContext);
            var pageData = CreatePageData(comData);
            if (pageData == null)
            {
                return null;
            }
            else
            {
                var returnInfo = new ReturnInfo<PageInfoT>();
                returnInfo.Data = pageData;
                FillPageData(returnInfo, comData);

                return returnInfo;
            }
        }

        /// <summary>
        /// 填充页面数据，包含当前用户所拥有的权限功能列表
        /// </summary>
        /// <param name="returnInfo">返回信息</param>
        /// <param name="comData">通用数据</param>
        protected virtual void FillPageData(ReturnInfo<PageInfoT> returnInfo, CommonUseData comData = null) { }

        /// <summary>
        /// 创建页面数据
        /// </summary>
        /// <param name="comData">通用数据</param>
        /// <returns>页面数据</returns>
        protected virtual PageInfoT CreatePageData(CommonUseData comData = null) => null;
    }
}
