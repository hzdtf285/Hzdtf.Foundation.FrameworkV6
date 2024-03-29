﻿using Hzdtf.BasicController;
using Hzdtf.BasicFunction.Service.Contract;
using Hzdtf.Utility.Attr;
using Hzdtf.Utility.Model;
using Hzdtf.Utility.Model.Page;
using Hzdtf.Utility.Model.Return;
using Hzdtf.Workflow.Model;
using Hzdtf.Workflow.Model.Expand.Filter;
using Hzdtf.Workflow.Service.Contract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Hzdtf.Logger.Contract;
using Hzdtf.Utility.Factory;
using Hzdtf.Utility.Localization;
using Hzdtf.Utility.RoutePermission;

namespace Hzdtf.Workflow.Controller
{
    /// <summary>
    /// 我申请的流程控制器
    /// @ 黄振东
    /// </summary>
    [Inject]
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [RoutePermission("MyApplyFlow")]
    public partial class MyApplyFlowController : PagingControllerBase<int, WorkflowInfo, IWorkflowService, DateRangePageInfo, ApplyFlowFilterInfo>
    {
        /// <summary>
        /// 用户服务
        /// </summary>
        protected readonly IUserService userService;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="log">日志</param>
        /// <param name="service">服务</param>
        /// <param name="localize">本地化</param>
        /// <param name="comUseDataFactory">通用数据工厂</param>
        /// <param name="pagingParseFilter">分页解析筛选</param>
        /// <param name="pagingReturnConvert">分页返回转换</param>
        /// <param name="userService">用户服务</param>
        public MyApplyFlowController(ILogable log = null, IWorkflowService service = null, ILocalization localize = null, ISimpleFactory<HttpContext, CommonUseData> comUseDataFactory = null,
            IPagingParseFilter pagingParseFilter = null, IPagingReturnConvert pagingReturnConvert = null,
            IUserService userService = null)
            : base(log, service, localize, comUseDataFactory, pagingParseFilter, pagingReturnConvert)
        {
            this.userService = userService;
        }

        /// <summary>
        /// 从服务里查询分页
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="filter">筛选</param>
        /// <param name="comData">通用数据</param>
        /// <returns>返回信息</returns>
        protected override ReturnInfo<PagingInfo<WorkflowInfo>> QueryPageFromService(int pageIndex, int pageSize, ApplyFlowFilterInfo filter, CommonUseData comData = null)
        {
            return service.QueryCurrUserApplyFlowPage(pageIndex, pageSize, filter, comData);
        }

        /// <summary>
        /// 获取流程明细信息
        /// </summary>
        /// <param name="workflowId">工作流ID</param>
        /// <returns>返回信息</returns>
        [HttpGet("GetFlowDetail/{workflowId}")]
        public virtual ReturnInfo<WorkflowInfo> GetFlowDetail(int workflowId) => service.FindCurrUserApplyDetail(workflowId, HttpContext.CreateCommonUseData(comUseDataFactory, menuCode: "MyApplyFlow", functionCodes: FunCodeDefine.QUERY_CODE));

        /// <summary>
        /// 填充页面数据，包含当前用户所拥有的权限功能列表
        /// </summary>
        /// <param name="returnInfo">返回信息</param>
        /// <param name="comData">通用数据</param>
        protected override void FillPageData(ReturnInfo<DateRangePageInfo> returnInfo, CommonUseData comData = null)
        {
            var re = userService.QueryPageData<DateRangePageInfo>("MyApplyFlow", () =>
            {
                return returnInfo.Data;
            }, comData: comData);
            returnInfo.FromBasic(re);
        }

        /// <summary>
        /// 创建页面数据
        /// </summary>
        /// <param name="comData">通用数据</param>
        /// <returns>页面数据</returns>
        protected override DateRangePageInfo CreatePageData(CommonUseData comData = null) => new DateRangePageInfo();
    }
}
