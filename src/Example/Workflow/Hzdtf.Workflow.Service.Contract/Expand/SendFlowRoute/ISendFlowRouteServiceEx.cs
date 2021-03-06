using Hzdtf.Utility.Model;
using Hzdtf.Utility.Model.Return;
using Hzdtf.Workflow.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hzdtf.Workflow.Service.Contract
{
    /// <summary>
    /// 送件流程路线服务接口
    /// @ 黄振东
    /// </summary>
    public partial interface ISendFlowRouteService : ISendFlowRouteServiceAsync
    {
        /// <summary>
        /// 根据流程关卡ID查询送件流程路线列表
        /// </summary>
        /// <param name="flowCensorshipId">流程关卡ID</param>
        /// <param name="connectionId">连接ID</param>
        /// <param name="comData">通用数据</param>
        /// <returns>返回信息</returns>
        ReturnInfo<IList<SendFlowRouteInfo>> QueryByFlowCensorshipId(int flowCensorshipId, CommonUseData comData = null, string connectionId = null);

        /// <summary>
        /// 根据流程关卡ID数组查询送件流程路线列表
        /// </summary>
        /// <param name="flowCensorshipIds">流程关卡ID</param>
        /// <param name="connectionId">连接ID</param>
        /// <param name="comData">通用数据</param>
        /// <returns>返回信息</returns>
        ReturnInfo<IList<SendFlowRouteInfo>> QueryByFlowCensorshipIds(int[] flowCensorshipIds, CommonUseData comData = null, string connectionId = null);
    }
}
