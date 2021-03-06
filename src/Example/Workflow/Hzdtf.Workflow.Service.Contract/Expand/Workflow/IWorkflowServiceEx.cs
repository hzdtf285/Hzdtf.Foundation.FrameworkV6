using Hzdtf.Utility.Model;
using Hzdtf.Utility.Model.Page;
using Hzdtf.Utility.Model.Return;
using Hzdtf.Workflow.Model;
using Hzdtf.Workflow.Model.Expand.Filter;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hzdtf.Workflow.Service.Contract
{
    /// <summary>
    /// 工作流服务接口
    /// @ 黄振东
    /// </summary>
    public partial interface IWorkflowService
    {
        /// <summary>
        /// 根据申请单号判断是否存在
        /// </summary>
        /// <param name="applyNo">申请单号</param>
        /// <param name="connectionId">连接ID</param>
        /// <param name="comData">通用数据</param>
        /// <returns>申请单号判断是否存在</returns>
        ReturnInfo<bool> ExistsByApplyNo(string applyNo, CommonUseData comData = null, string connectionId = null);

        /// <summary>
        /// 查询当前用户的待办的工作流列表并分页
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="filter">筛选</param>
        /// <param name="connectionId">连接ID</param>
        /// <param name="comData">通用数据</param>
        /// <returns>返回信息</returns>
        ReturnInfo<PagingInfo<WorkflowInfo>> QueryCurrUserWaitHandlePage(int pageIndex, int pageSize, WaitHandleFilterInfo filter, CommonUseData comData = null, string connectionId = null);

        /// <summary>
        /// 查询当前用户的已审核的工作流列表并分页
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="filter">筛选</param>
        /// <param name="connectionId">连接ID</param>
        /// <param name="comData">通用数据</param>
        /// <returns>返回信息</returns>
        ReturnInfo<PagingInfo<WorkflowInfo>> QueryCurrUserAuditedFlowPage(int pageIndex, int pageSize, AuditFlowFilterInfo filter, CommonUseData comData = null, string connectionId = null);

        /// <summary>
        /// 查询当前用户的申请的工作流列表并分页
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="filter">筛选</param>
        /// <param name="connectionId">连接ID</param>
        /// <param name="comData">通用数据</param>
        /// <returns>返回信息</returns>
        ReturnInfo<PagingInfo<WorkflowInfo>> QueryCurrUserApplyFlowPage(int pageIndex, int pageSize, ApplyFlowFilterInfo filter, CommonUseData comData = null, string connectionId = null);

        /// <summary>
        /// 根据ID查找工作流信息且包含处理列表和所有配置信息
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="connectionId">连接ID</param>
        /// <param name="comData">通用数据</param>
        /// <returns>返回信息</returns>
        ReturnInfo<WorkflowInfo> FindContainHandlesAndAllConfigs(int id, CommonUseData comData = null, string connectionId = null);

        /// <summary>
        /// 根据ID查找当前用户申请明细信息
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="connectionId">连接ID</param>
        /// <param name="comData">通用数据</param>
        /// <returns>返回信息</returns>
        ReturnInfo<WorkflowInfo> FindCurrUserApplyDetail(int id, CommonUseData comData = null, string connectionId = null);

        /// <summary>
        /// 根据ID查找审核明细信息
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="handleId">处理ID</param>
        /// <param name="connectionId">连接ID</param>
        /// <param name="comData">通用数据</param>
        /// <returns>返回信息</returns>
        ReturnInfo<WorkflowInfo> FindAuditDetail(int id, int handleId, CommonUseData comData = null, string connectionId = null);

        /// <summary>
        /// 根据ID查找待审核明细信息
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="handleId">处理ID</param>
        /// <param name="connectionId">连接ID</param>
        /// <param name="comData">通用数据</param>
        /// <returns>返回信息</returns>
        ReturnInfo<WorkflowInfo> FindWaitDetail(int id, int handleId, CommonUseData comData = null, string connectionId = null);

        /// <summary>
        /// 根据ID查找已审核明细信息
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="handleId">处理ID</param>
        /// <param name="connectionId">连接ID</param>
        /// <param name="comData">通用数据</param>
        /// <returns>返回信息</returns>
        ReturnInfo<WorkflowInfo> FindAuditedDetail(int id, int handleId, CommonUseData comData = null, string connectionId = null);
    }
}
