﻿using Hzdtf.Utility.Attr.ParamAttr;
using Hzdtf.Utility.Enums;
using Hzdtf.Utility.Model;
using Hzdtf.Utility.Model.Return;
using Hzdtf.Utility.Utils;
using Hzdtf.Workflow.Model;
using Hzdtf.Workflow.Service.Contract;
using Hzdtf.Workflow.Service.Contract.Engine;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;

namespace Hzdtf.Workflow.Service.Impl
{
    /// <summary>
    /// 工作流定义服务
    /// @ 黄振东
    /// </summary>
    public partial class WorkflowDefineService : IWorkflowConfigReader
    {
        #region 属性与字段

        /// <summary>
        /// 流程服务
        /// </summary>
        public IFlowService FlowService
        {
            get;
            set;
        }

        /// <summary>
        /// 表单服务
        /// </summary>
        public IFormService FormService
        {
            get;
            set;
        }

        /// <summary>
        /// 流程关卡服务
        /// </summary>
        public IFlowCensorshipService FlowCensorshipService
        {
            get;
            set;
        }

        /// <summary>
        /// 标准关卡服务
        /// </summary>
        public IStandardCensorshipService StandardCensorshipService
        {
            get;
            set;
        }

        /// <summary>
        /// 送件流程路线服务
        /// </summary>
        public ISendFlowRouteService SendFlowRouteService
        {
            get;
            set;
        }

        /// <summary>
        /// 退件流程路线服务
        /// </summary>
        public IReturnFlowRouteService ReturnFlowRouteService
        {
            get;
            set;
        }

        #endregion

        /// <summary>
        /// 根据编码查询工作流定义信息
        /// </summary>
        /// <param name="code">编码</param>
        /// <param name="connectionId">连接ID</param>
        /// <param name="comData">通用数据</param>
        /// <returns>返回信息</returns>
        public virtual ReturnInfo<WorkflowDefineInfo> FindByCode([DisplayName2("编码"), Required] string code, CommonUseData comData = null, string connectionId = null)
        {
            return ExecReturnFuncAndConnectionId<WorkflowDefineInfo>((reInfo, connId) =>
            {
                return persistence.SelectByCode(code, connId);
            }, null, connectionId, AccessMode.SLAVE);
        }

        #region IWorkflowConfigReader 接口

        /// <summary>
        /// 根据工作流定义ID读取工作流定义信息的所有配置
        /// </summary>
        /// <param name="workflowDefineId">工作流定义ID</param>
        /// <param name="connectionId">连接ID</param>
        /// <param name="comData">通用数据</param>
        /// <returns>返回信息</returns>
        public virtual ReturnInfo<WorkflowDefineInfo> ReaderAllConfig([DisplayName2("工作流定义ID"), Id] int workflowDefineId, CommonUseData comData = null, string connectionId = null)
        {
            return ExecReturnFuncAndConnectionId<WorkflowDefineInfo>((reInfo, connId) =>
            {
                WorkflowDefineInfo workflowDefine = persistence.Select(workflowDefineId, connectionId: connId, comData: comData);
                BasicReturnInfo basicReturn = ReaderOtherConfig(workflowDefine, connectionId: connId, comData: comData);
                if (basicReturn.Failure())
                {
                    reInfo.FromBasic(basicReturn);

                    return null;
                }

                return workflowDefine;
            }, null, connectionId, AccessMode.SLAVE);
        }

        /// <summary>
        /// 根据工作流编码读取工作流定义信息的所有配置
        /// </summary>
        /// <param name="workflowCode">工作流编码</param>
        /// <param name="connectionId">连接ID</param>
        /// <param name="comData">通用数据</param>
        /// <returns>返回信息</returns>
        public virtual ReturnInfo<WorkflowDefineInfo> ReaderAllConfig([DisplayName2("工作流编码"), Required] string workflowCode, CommonUseData comData = null, string connectionId = null)
        {
            return ExecReturnFuncAndConnectionId<WorkflowDefineInfo>((reInfo, connId) =>
            {
                WorkflowDefineInfo workflowDefine = persistence.SelectByCode(workflowCode, connId);
                BasicReturnInfo basicReturn = ReaderOtherConfig(workflowDefine, connectionId: connId, comData: comData);
                if (basicReturn.Failure())
                {
                    reInfo.FromBasic(basicReturn);

                    return null;
                }

                return workflowDefine;
            }, null, connectionId, AccessMode.SLAVE);
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 读取其他配置
        /// </summary>
        /// <param name="workflowDefine">工作流定义</param>
        /// <param name="connectionId">连接ID</param>
        /// <param name="comData">通用数据</param>
        private BasicReturnInfo ReaderOtherConfig(WorkflowDefineInfo workflowDefine, string connectionId, CommonUseData comData = null)
        {
            BasicReturnInfo basicReturn = new BasicReturnInfo();
            if (workflowDefine == null)
            {
                basicReturn.SetFailureMsg("找不到工作流定义信息");
                return basicReturn;
            }

            #region 查找流程/表单/流程关卡

            ReturnInfo<FlowInfo> reFlow = FlowService.Find(workflowDefine.FlowId, connectionId : connectionId, comData: comData);
            if (reFlow.Failure())
            {
                basicReturn.FromBasic(reFlow);
                return basicReturn;
            }
            if (reFlow.Data == null)
            {
                basicReturn.SetFailureMsg("找不到工作流的流程信息");
                return basicReturn;
            }

            ReturnInfo<FormInfo> reForm = FormService.Find(workflowDefine.FormId, connectionId : connectionId, comData: comData);

            workflowDefine.Flow = reFlow.Data;

            if (reForm.Failure())
            {
                basicReturn.FromBasic(reForm);
                return basicReturn;
            }
            if (reForm.Data == null)
            {
                basicReturn.SetFailureMsg("找不到工作流的表单信息");
                return basicReturn;
            }
            workflowDefine.Form = reForm.Data;

            ReturnInfo<IList<FlowCensorshipInfo>> reFlowCensorships = FlowCensorshipService.QueryByFlowId(workflowDefine.FlowId, connectionId : connectionId, comData: comData);
            if (reFlowCensorships.Failure())
            {
                basicReturn.FromBasic(reFlowCensorships);
                return basicReturn;
            }
            if (reFlowCensorships.Data.IsNullOrCount0())
            {
                basicReturn.SetFailureMsg("找不到工作流的流程关卡信息");
                return basicReturn;
            }
            workflowDefine.Flow.FlowCensorships = reFlowCensorships.Data.ToArray();

            // 构造流程关卡ID数组
            int[] flowCensorshipIds = new int[workflowDefine.Flow.FlowCensorships.Length];
            IList<int> stFlowCensorshipIds = new List<int>();
            for (var i = 0; i < flowCensorshipIds.Length; i++)
            {
                var f = workflowDefine.Flow.FlowCensorships[i];
                flowCensorshipIds[i] = f.Id;

                if (f.OwnerCensorshipType == CensorshipTypeEnum.STANDARD)
                {
                    stFlowCensorshipIds.Add(f.OwnerCensorshipId);
                }
            }
            #endregion

            #region 查找送件/退件路线/标准关卡

            IList<StandardCensorshipInfo> standardCensorships = null;
            // 标准关卡
            if (!stFlowCensorshipIds.IsNullOrCount0())
            {
                ReturnInfo<IList<StandardCensorshipInfo>> reStand = StandardCensorshipService.Find(stFlowCensorshipIds.ToArray(), connectionId : connectionId, comData: comData);                
                if (reStand.Failure())
                {
                    basicReturn.FromBasic(reStand);
                    return basicReturn;
                }
                if (reStand.Data.IsNullOrCount0())
                {
                    basicReturn.SetFailureMsg("找不到标准关卡信息");
                    return basicReturn;
                }

                standardCensorships = reStand.Data;
            }

            ReturnInfo<IList<SendFlowRouteInfo>> reSend = SendFlowRouteService.QueryByFlowCensorshipIds(flowCensorshipIds, connectionId : connectionId, comData: comData);
            if (reSend.Failure())
            {
                basicReturn.FromBasic(reSend);
                return basicReturn;
            }
            if (reSend.Data.IsNullOrCount0())
            {
                basicReturn.SetFailureMsg("找不到工作流的送件路线信息");
            }

            ReturnInfo<IList<ReturnFlowRouteInfo>> reReturn = ReturnFlowRouteService.QueryByFlowCensorshipIds(flowCensorshipIds, connectionId : connectionId, comData: comData);
            if (reReturn.Failure())
            {
                basicReturn.FromBasic(reReturn);
                return basicReturn;
            }

            foreach (var f in workflowDefine.Flow.FlowCensorships)
            {
                switch (f.OwnerCensorshipType)
                {
                    case CensorshipTypeEnum.STANDARD:
                        IList<StandardCensorshipInfo> stdList = new List<StandardCensorshipInfo>();
                        foreach (var s in standardCensorships)
                        {
                            if (f.OwnerCensorshipId == s.Id)
                            {
                                stdList.Add(s);
                            }
                        }

                        if (stdList.Count == 0)
                        {
                            basicReturn.SetFailureMsg($"找不到归属ID为{f.OwnerCensorshipId}的标准关卡信息");
                            return basicReturn;
                        }

                        f.StandardCensorships = stdList.ToArray();

                        break;
                }

                IList<SendFlowRouteInfo> sendRotes = new List<SendFlowRouteInfo>();
                foreach (var send in reSend.Data)
                {
                    if (f.Id == send.FlowCensorshipId)
                    {
                        sendRotes.Add(send);
                    }
                }

                if (!sendRotes.IsNullOrCount0())
                {
                    f.SendFlowRoutes = sendRotes.ToArray();
                }

                if (!reReturn.Data.IsNullOrCount0())
                {
                    IList<ReturnFlowRouteInfo> returnRotes = new List<ReturnFlowRouteInfo>();
                    foreach (var re in reReturn.Data)
                    {
                        if (f.Id == re.FlowCensorshipId)
                        {
                            returnRotes.Add(re);
                        }
                    }

                    if (!returnRotes.IsNullOrCount0())
                    {
                        f.ReturnFlowRoutes = returnRotes.ToArray();
                    }
                }
            }

            #endregion

            return basicReturn;
        }

        #endregion
    }
}
