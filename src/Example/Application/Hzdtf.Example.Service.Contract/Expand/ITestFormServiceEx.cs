using Hzdtf.Example.Model;
using Hzdtf.Utility.Model;
using Hzdtf.Utility.Model.Return;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hzdtf.Example.Service.Contract
{
    /// <summary>
    /// 测试表单服务接口
    /// @ 黄振东
    /// </summary>
    public partial interface ITestFormService
    {
        /// <summary>
        /// 根据流程ID修改流程状态
        /// </summary>
        /// <param name="testForm">测试表单</param>
        /// <param name="connectionId">连接ID</param>
        /// <param name="comData">通用数据</param>
        /// <returns>返回信息</returns>
        ReturnInfo<bool> ModifyFlowStatusByWorkflowId(TestFormInfo testForm, CommonUseData comData = null, string connectionId = null);
    }
}
