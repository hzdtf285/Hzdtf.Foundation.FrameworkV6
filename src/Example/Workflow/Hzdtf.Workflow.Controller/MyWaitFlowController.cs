using Hzdtf.BasicController;
using Hzdtf.BasicFunction.Service.Contract;
using Hzdtf.Logger.Contract;
using Hzdtf.Utility.Attr;
using Hzdtf.Utility.Factory;
using Hzdtf.Utility.Localization;
using Hzdtf.Utility.Model;
using Hzdtf.Utility.Model.Page;
using Hzdtf.Utility.Model.Return;
using Hzdtf.Utility.RoutePermission;
using Hzdtf.Workflow.Model;
using Hzdtf.Workflow.Model.Expand;
using Hzdtf.Workflow.Model.Expand.Filter;
using Hzdtf.Workflow.Service.Contract;
using Hzdtf.Workflow.Service.Contract.Engine;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hzdtf.Workflow.Controller
{
    /// <summary>
    /// 我的待办流程控制器
    /// @ 黄振东
    /// </summary>
    [Inject]
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [RoutePermission("MyWaitFlow")]
    public partial class MyWaitFlowController : PagingControllerBase<int, WorkflowInfo, IWorkflowService, DateRangePageInfo, WaitHandleFilterInfo>
    {
        /// <summary>
        /// 用户服务
        /// </summary>
        protected readonly IUserService userService;

        /// <summary>
        /// 工作流处理服务
        /// </summary>
        protected readonly IWorkflowHandleService workflowHandleService;

        /// <summary>
        /// 工作流审核
        /// </summary>
        protected readonly IWorkflowAudit workFlowAudit;

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
        /// <param name="workflowHandleService">工作流处理服务</param>
        /// <param name="workFlowAudit">工作流审核</param>
        public MyWaitFlowController(ILogable log = null, IWorkflowService service = null, ILocalization localize = null, ISimpleFactory<HttpContext, CommonUseData> comUseDataFactory = null,
            IPagingParseFilter pagingParseFilter = null, IPagingReturnConvert pagingReturnConvert = null,
            IUserService userService = null, IWorkflowHandleService workflowHandleService = null, IWorkflowAudit workFlowAudit = null)
            : base(log, service, localize, comUseDataFactory, pagingParseFilter, pagingReturnConvert)
        {
            this.userService = userService;
            this.workflowHandleService = workflowHandleService;
            this.workFlowAudit = workFlowAudit;
        }

        /// <summary>
        /// 从服务里查询分页
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="filter">筛选</param>
        /// <param name="comData">通用数据</param>
        /// <returns>返回信息任务</returns>
        protected override ReturnInfo<PagingInfo<WorkflowInfo>> QueryPageFromService(int pageIndex, int pageSize, WaitHandleFilterInfo filter, CommonUseData comData = null)
        {
            return service.QueryCurrUserWaitHandlePage(pageIndex, pageSize, filter, comData);
        }

        /// <summary>
        /// 根据处理ID修改为已读
        /// </summary>
        /// <param name="handleId">处理ID</param>
        /// <returns>返回信息</returns>
        [HttpPut("ModifyReadedByHandleId/{handleId}")]
        public virtual ReturnInfo<bool> ModifyReadedByHandleId(int handleId) => workflowHandleService.ModifyToReadedById(handleId, comUseDataFactory.Create(HttpContext));

        /// <summary>
        /// 获取审核明细信息
        /// </summary>
        /// <param name="workflowId">工作流ID</param>
        /// <param name="handleId">处理ID</param>
        /// <returns>返回信息</returns>
        [HttpGet("GetAuditDetail/{workflowId}/{handleId}")]
        public virtual ReturnInfo<WorkflowInfo> GetAuditDetail(int workflowId, int handleId) => service.FindAuditDetail(workflowId, handleId, comUseDataFactory.Create(HttpContext));

        /// <summary>
        /// 执行审核
        /// </summary>
        /// <param name="flowAudit">流程审核信息</param>
        /// <returns>返回信息</returns>
        [HttpPost("ExecAudit")]
        public virtual ReturnInfo<bool> ExecAudit(FlowAuditInfo flowAudit) => workFlowAudit.Execute(flowAudit.ToFlowIn(), comUseDataFactory.Create(HttpContext));

        /// <summary>
        /// 获取流程明细信息
        /// </summary>
        /// <param name="workflowId">工作流ID</param>
        /// <param name="handleId">处理ID</param>
        /// <returns>返回信息</returns>
        [HttpGet("GetFlowDetail/{workflowId}/{handleId}")]
        public virtual ReturnInfo<WorkflowInfo> GetFlowDetail(int workflowId, int handleId) => service.FindWaitDetail(workflowId, handleId, comUseDataFactory.Create(HttpContext));

        /// <summary>
        /// 填充页面数据，包含当前用户所拥有的权限功能列表
        /// </summary>
        /// <param name="returnInfo">返回信息</param>
        /// <param name="comData">通用数据</param>
        protected override void FillPageData(ReturnInfo<DateRangePageInfo> returnInfo, CommonUseData comData = null)
        {
            var re = userService.QueryPageData<DateRangePageInfo>("MyWaitFlow", () =>
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
