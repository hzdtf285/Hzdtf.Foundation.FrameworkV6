﻿using Hzdtf.Utility.Model;
using Hzdtf.Utility.Model.Return;
using Hzdtf.Workflow.Model;
using Hzdtf.Workflow.Service.Contract;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hzdtf.Workflow.Service.Impl
{
    /// <summary>
    /// 退件流程路线服务
    /// @ 黄振东
    /// </summary>
    public partial class ReturnFlowRouteService : IReturnFlowRouteServiceAsync
    {
        /// <summary>
        /// 异步根据流程关卡ID查询退件流程路线列表
        /// </summary>
        /// <param name="flowCensorshipId">流程关卡ID</param>
        /// <param name="connectionId">连接ID</param>
        /// <param name="comData">通用数据</param>
        /// <returns>任务</returns>
        public virtual async Task<ReturnInfo<IList<ReturnFlowRouteInfo>>> QueryByFlowCensorshipIdAsync(int flowCensorshipId, CommonUseData comData = null, string connectionId = null)
        {
            return await Task.Run<ReturnInfo<IList<ReturnFlowRouteInfo>>>(() =>
            {
                return QueryByFlowCensorshipId(flowCensorshipId, connectionId : connectionId, comData: comData);
            });
        }

        /// <summary>
        /// 异步根据流程关卡ID数组查询退件流程路线列表
        /// </summary>
        /// <param name="flowCensorshipIds">流程关卡ID</param>
        /// <param name="connectionId">连接ID</param>>
        /// <param name="comData">通用数据</param>
        /// <returns>任务</returns>
        public virtual async Task<ReturnInfo<IList<ReturnFlowRouteInfo>>> QueryByFlowCensorshipIdsAsync(int[] flowCensorshipIds, CommonUseData comData = null, string connectionId = null)
        {
            return await Task.Run<ReturnInfo<IList<ReturnFlowRouteInfo>>>(() =>
            {
                return QueryByFlowCensorshipIds(flowCensorshipIds, connectionId : connectionId, comData: comData);
            });
        }
    }
}
