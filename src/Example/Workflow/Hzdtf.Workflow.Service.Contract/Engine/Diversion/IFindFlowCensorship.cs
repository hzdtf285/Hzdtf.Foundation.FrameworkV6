using Hzdtf.Utility.Model;
using Hzdtf.Utility.Model.Return;
using Hzdtf.Workflow.Model.Expand.Diversion;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hzdtf.Workflow.Service.Contract.Engine.Diversion
{
    /// <summary>
    /// 查找流程关卡接口
    /// @ 黄振东
    /// </summary>
    public partial interface IFindFlowCensorship
    {
        /// <summary>
        /// 查找下一个处理信息
        /// </summary>
        /// <param name="findFlowCensorshipIn">查找流程关卡输入</param>
        /// <param name="connectionId">连接ID</param>
        /// <param name="comData">通用数据</param>
        /// <returns>返回信息</returns>
        ReturnInfo<FlowCensorshipOutInfo> NextHandler(FlowCensorshipInInfo findFlowCensorshipIn, CommonUseData comData = null, string connectionId = null);
    }
}
