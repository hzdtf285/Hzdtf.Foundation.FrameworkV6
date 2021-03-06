using Hzdtf.Utility.Model;
using Hzdtf.Utility.Model.Return;
using Hzdtf.Workflow.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Hzdtf.Workflow.Service.Contract
{
    /// <summary>
    /// 工作流定义服务
    /// @ 黄振东
    /// </summary>
    public partial interface IWorkflowDefineService
    {
        /// <summary>
        /// 根据编码查询工作流定义信息
        /// </summary>
        /// <param name="code">编码</param>
        /// <param name="connectionId">连接ID</param>
        /// <param name="comData">通用数据</param>
        /// <returns>返回信息</returns>
        ReturnInfo<WorkflowDefineInfo> FindByCode(string code, CommonUseData comData = null, string connectionId = null);
    }
}
