﻿using Hzdtf.Utility.Attr.ParamAttr;
using Hzdtf.Utility.Enums;
using Hzdtf.Utility.Model;
using Hzdtf.Utility.Model.Return;
using Hzdtf.Workflow.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hzdtf.Workflow.Service.Impl
{
    /// <summary>
    /// 送件流程路线服务
    /// @ 黄振东
    /// </summary>
    public partial class SendFlowRouteService
    {
        /// <summary>
        /// 根据流程关卡ID查询送件流程路线列表
        /// </summary>
        /// <param name="flowCensorshipId">流程关卡ID</param>
        /// <param name="connectionId">连接ID</param>
        /// <param name="comData">通用数据</param>
        /// <returns>返回信息</returns>
        public virtual ReturnInfo<IList<SendFlowRouteInfo>> QueryByFlowCensorshipId([DisplayName2("流程关卡ID"), Id] int flowCensorshipId, CommonUseData comData = null, string connectionId = null)
        {
            return ExecReturnFuncAndConnectionId<IList<SendFlowRouteInfo>>((reInfo, connId) =>
            {
                return persistence.SelectByFlowCensorshipId(flowCensorshipId, connId);
            }, null, connectionId, AccessMode.SLAVE);
        }

        /// <summary>
        /// 根据流程关卡ID数组查询送件流程路线列表
        /// </summary>
        /// <param name="flowCensorshipIds">流程关卡ID</param>
        /// <param name="connectionId">连接ID</param>
        /// <param name="comData">通用数据</param>
        /// <returns>返回信息</returns>
        public virtual ReturnInfo<IList<SendFlowRouteInfo>> QueryByFlowCensorshipIds(int[] flowCensorshipIds, CommonUseData comData = null, string connectionId = null)
        {
            return ExecReturnFuncAndConnectionId<IList<SendFlowRouteInfo>>((reInfo, connId) =>
            {
                return persistence.SelectByFlowCensorshipIds(flowCensorshipIds, connId);
            }, null, connectionId, AccessMode.SLAVE);
        }
    }
}
