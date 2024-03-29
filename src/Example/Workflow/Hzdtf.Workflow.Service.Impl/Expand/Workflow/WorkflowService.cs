﻿using Hzdtf.Utility.Attr.ParamAttr;
using Hzdtf.Utility.Model;
using Hzdtf.Utility.Model.Page;
using Hzdtf.Utility.Model.Return;
using Hzdtf.Workflow.Model;
using Hzdtf.Workflow.Model.Expand.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Hzdtf.Utility.Utils;
using Hzdtf.Workflow.Service.Contract.Engine;
using Hzdtf.Utility.Enums;
using Hzdtf.Utility.Attr;

namespace Hzdtf.Workflow.Service.Impl
{
    /// <summary>
    /// 工作流服务
    /// @ 黄振东
    /// </summary>
    public partial class WorkflowService
    {
        /// <summary>
        /// 工作流配置读取
        /// </summary>
        public IWorkflowConfigReader WorkflowConfigReader
        {
            get;
            set;
        }

        /// <summary>
        /// 表单数据读取工厂
        /// </summary>
        public IFormDataReaderFactory FormDataReaderFactory
        {
            get;
            set;
        }

        /// <summary>
        /// 根据申请单号判断是否存在
        /// </summary>
        /// <param name="applyNo">申请单号</param>
        /// <param name="connectionId">连接ID</param>
        /// <returns>申请单号判断是否存在</returns>
        /// <param name="comData">通用数据</param>
        public virtual ReturnInfo<bool> ExistsByApplyNo([DisplayName2("申请单号"), Required] string applyNo, CommonUseData comData = null, string connectionId = null)
        {
            return ExecReturnFuncAndConnectionId<bool>((reInfo, connId) =>
            {
                return persistence.CountByApplyNo(applyNo, connId) > 0;
            }, null, connectionId, AccessMode.SLAVE);
        }

        /// <summary>
        /// 查询当前用户的待办的工作流列表并分页
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="filter">筛选</param>
        /// <param name="connectionId">连接ID</param>
        /// <param name="comData">通用数据</param>
        /// <returns>返回信息</returns>
        public virtual ReturnInfo<PagingInfo<WorkflowInfo>> QueryCurrUserWaitHandlePage(int pageIndex, int pageSize, WaitHandleFilterInfo filter, CommonUseData comData = null, string connectionId = null)
        {
            if (filter == null)
            {
                filter = new WaitHandleFilterInfo();
            }
            var user = UserTool<int>.GetCurrUser(comData);
            filter.HandlerId = user.Id;
            filter.EndCreateTime = filter.EndCreateTime.AddThisDayLastTime();

            return ExecReturnFuncAndConnectionId<PagingInfo<WorkflowInfo>>((reInfo, connId) =>
            {
                return persistence.SelectWaitHandlePage(pageIndex, pageSize, filter, connId);
            }, null, connectionId, AccessMode.SLAVE);
        }

        /// <summary>
        /// 查询当前用户的已审核的工作流列表并分页
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="filter">筛选</param>
        /// <param name="connectionId">连接ID</param>
        /// <param name="comData">通用数据</param>
        /// <returns>返回信息</returns>
        public virtual ReturnInfo<PagingInfo<WorkflowInfo>> QueryCurrUserAuditedFlowPage(int pageIndex, int pageSize, AuditFlowFilterInfo filter, CommonUseData comData = null, string connectionId = null)
        {
            if (filter == null)
            {
                filter = new AuditFlowFilterInfo();
            }
            var user = UserTool<int>.GetCurrUser(comData);
            filter.HandlerId = user.Id;
            filter.EndCreateTime = filter.EndCreateTime.AddThisDayLastTime();

            return ExecReturnFuncAndConnectionId<PagingInfo<WorkflowInfo>>((reInfo, connId) =>
            {
                return persistence.SelectAuditedHandlePage(pageIndex, pageSize, filter, connId);
            }, null, connectionId, AccessMode.SLAVE);
        }

        /// <summary>
        /// 查询当前用户的申请的工作流列表并分页
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="filter">筛选</param>
        /// <param name="connectionId">连接ID</param>
        /// <param name="comData">通用数据</param>
        /// <returns>返回信息</returns>
        public virtual ReturnInfo<PagingInfo<WorkflowInfo>> QueryCurrUserApplyFlowPage(int pageIndex, int pageSize, ApplyFlowFilterInfo filter, CommonUseData comData = null, string connectionId = null)
        {
            if (filter == null)
            {
                filter = new ApplyFlowFilterInfo();
            }
            var user = UserTool<int>.GetCurrUser(comData);
            filter.HandlerId = user.Id;
            filter.EndCreateTime = filter.EndCreateTime.AddThisDayLastTime();

            return ExecReturnFuncAndConnectionId<PagingInfo<WorkflowInfo>>((reInfo, connId) =>
            {
                return persistence.SelectApplyFlowPage(pageIndex, pageSize, filter, connId);
            }, null, connectionId, AccessMode.SLAVE);
        }

        /// <summary>
        /// 根据ID查找工作流信息且包含处理列表和所有配置信息
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="connectionId">连接ID</param>
        /// <param name="comData">通用数据</param>
        /// <returns>返回信息</returns>
        public virtual ReturnInfo<WorkflowInfo> FindContainHandlesAndAllConfigs([DisplayName2("ID"), Id] int id, CommonUseData comData = null, string connectionId = null)
        {
            return ExecReturnFuncAndConnectionId<WorkflowInfo>((reInfo, connId) =>
            {
                WorkflowInfo workflow = persistence.SelectContainHandles(id, connId);
                if (workflow == null)
                {
                    reInfo.SetFailureMsg("找不到此工作流信息");

                    return null;
                }
                if (workflow.Handles.IsNullOrCount0())
                {
                    reInfo.SetFailureMsg("找不到此工作流处理信息");

                    return null;
                }

                ReturnInfo<WorkflowDefineInfo> reDefine = WorkflowConfigReader.ReaderAllConfig(workflow.WorkflowDefineId, connectionId: connId, comData: comData);
                if (reDefine.Failure())
                {
                    reInfo.FromBasic(reDefine);

                    return null;
                }
                if (reDefine.Data == null)
                {
                    reInfo.SetFailureMsg("找不到此工作流定义信息");

                    return null;
                }

                workflow.WorkflowDefine = reDefine.Data;

                return workflow;
            }, null, connectionId, AccessMode.SLAVE);
        }

        /// <summary>
        /// 根据ID查找审核明细信息
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="handleId">处理ID</param>
        /// <param name="connectionId">连接ID</param>
        /// <param name="comData">通用数据</param>
        /// <returns>返回信息</returns>
        public virtual ReturnInfo<WorkflowInfo> FindAuditDetail([DisplayName2("ID"), Id] int id, [DisplayName2("处理ID"), Id] int handleId, CommonUseData comData = null, string connectionId = null)
        {
            ReturnInfo<WorkflowInfo> returnInfo = FindContainHandlesAndAllConfigs(id, connectionId : connectionId, comData: comData);
            if (returnInfo.Failure())
            {
                return returnInfo;
            }

            WorkflowHandleInfo currHandle = null;
            foreach (var h in returnInfo.Data.Handles)
            {
                if (h.Id == handleId)
                {
                    currHandle = h;
                    break;
                }
            }

            BasicReturnInfo basicReturn = WorkflowUtil.CanCurrUserAudit(currHandle, comData);
            if (basicReturn.Failure())
            {
                returnInfo.FromBasic(basicReturn);
            }

            return returnInfo;
        }

        /// <summary>
        /// 根据ID查找待审核明细信息
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="handleId">处理ID</param>
        /// <param name="connectionId">连接ID</param>
        /// <param name="comData">通用数据</param>
        /// <returns>返回信息</returns>
        public virtual ReturnInfo<WorkflowInfo> FindWaitDetail([DisplayName2("ID"), Id] int id, [DisplayName2("处理ID"), Id] int handleId, CommonUseData comData = null, string connectionId = null)
        {
            ReturnInfo<WorkflowInfo> returnInfo = FindContainHandlesAndAllConfigs(id, connectionId : connectionId, comData: comData);
            if (returnInfo.Failure())
            {
                return returnInfo;
            }

            WorkflowHandleInfo currHandle = null;
            foreach (var h in returnInfo.Data.Handles)
            {
                if (h.Id == handleId)
                {
                    currHandle = h;
                    break;
                }
            }
            if (currHandle.HandleStatus == HandleStatusEnum.EFFICACYED)
            {
                returnInfo.SetFailureMsg("您还未处理的流程已失效");
                return returnInfo;
            }

            return returnInfo;
        }

        /// <summary>
        /// 根据ID查找已审核明细信息
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="handleId">处理ID</param>
        /// <param name="connectionId">连接ID</param>
        /// <param name="comData">通用数据</param>
        /// <returns>返回信息</returns>
        public virtual ReturnInfo<WorkflowInfo> FindAuditedDetail([DisplayName2("ID"), Id] int id, [DisplayName2("处理ID"), Id] int handleId, CommonUseData comData = null, string connectionId = null)
        {
            ReturnInfo<WorkflowInfo> returnInfo = FindContainHandlesAndAllConfigs(id, connectionId : connectionId, comData: comData);
            if (returnInfo.Failure())
            {
                return returnInfo;
            }

            WorkflowHandleInfo currHandle = null;
            foreach (var h in returnInfo.Data.Handles)
            {
                if (h.Id == handleId)
                {
                    currHandle = h;
                    break;
                }
            }

            if (currHandle.HandleStatus == HandleStatusEnum.UN_HANDLE)
            {
                returnInfo.SetFailureMsg("您还未处理该流程");
                return returnInfo;
            }
            if (currHandle.HandleStatus == HandleStatusEnum.EFFICACYED)
            {
                returnInfo.SetFailureMsg("您还未处理的流程已失效");
                return returnInfo;
            }

            return returnInfo;
        }

        /// <summary>
        /// 根据ID查找当前用户申请明细信息
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="connectionId">连接ID</param>
        /// <param name="comData">通用数据</param>
        /// <returns>返回信息</returns>
        public virtual ReturnInfo<WorkflowInfo> FindCurrUserApplyDetail([DisplayName2("ID"), Id] int id, CommonUseData comData = null, string connectionId = null)
        {
            ReturnInfo<WorkflowInfo> returnInfo = FindContainHandlesAndAllConfigs(id, connectionId : connectionId, comData: comData);
            if (returnInfo.Failure())
            {
                return returnInfo;
            }

            var user = UserTool<int>.GetCurrUser(comData);
            if (returnInfo.Data.CreaterId != user.Id)
            {
                returnInfo.SetFailureMsg("此工作流不是您申请的，无权限查看");

                return returnInfo;
            }

            return returnInfo;
        }
    }
}
