﻿using Hzdtf.Example.Model;
using Hzdtf.Utility.Attr.ParamAttr;
using Hzdtf.Utility.Enums;
using Hzdtf.Utility.Model;
using Hzdtf.Utility.Model.Return;
using Hzdtf.Workflow.Model.Expand;
using Hzdtf.Workflow.Service.Contract.Engine;
using Hzdtf.Workflow.Service.Contract.Engine.Form;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hzdtf.Example.Service.Impl
{
    /// <summary>
    /// 测试表单服务
    /// @ 黄振东
    /// </summary>
    public partial class TestFormService : IFormDataReader<TestFormInfo>, IFormDataReader, IFormService<TestFormInfo>
    {
        /// <summary>
        /// 根据流程ID修改流程状态
        /// </summary>
        /// <param name="testForm">测试表单</param>
        /// <param name="connectionId">连接ID</param>
        /// <param name="comData">通用数据</param>
        /// <returns>返回信息</returns>
        public virtual ReturnInfo<bool> ModifyFlowStatusByWorkflowId(TestFormInfo testForm, CommonUseData comData = null, string connectionId = null)
        {
            return ExecReturnFuncAndConnectionId<bool>((reInfo, connId) =>
            {
                return persistence.UpdateFlowStatusByWorkflowId(testForm, connId) > 0;
            }, null, connectionId);
        }

        /// <summary>
        /// 根据工作流ID读取表单数据
        /// </summary>
        /// <param name="workflowId">工作流ID</param>
        /// <param name="connectionId">连接ID</param>
        /// <param name="comData">通用数据</param>
        /// <returns>返回信息</returns>
        ReturnInfo<TestFormInfo> IFormDataReader<TestFormInfo>.ReaderByWorkflowId([DisplayName2("工作流ID"), Id] int workflowId, CommonUseData comData = null, string connectionId = null)
        {
            return ExecReturnFuncAndConnectionId<TestFormInfo>((reInfo, connId) =>
            {
                return persistence.SelectByWorkflowId(workflowId, connId);
            }, null, connectionId, AccessMode.SLAVE);
        }

        /// <summary>
        /// 根据工作流ID读取表单数据
        /// </summary>
        /// <param name="workflowId">工作流ID</param>
        /// <param name="connectionId">连接ID</param>
        /// <param name="comData">通用数据</param>
        /// <returns>返回信息</returns>
        ReturnInfo<ConcreteFormInfo> IFormDataReader<ConcreteFormInfo>.ReaderByWorkflowId([DisplayName2("工作流ID"), Id] int workflowId, CommonUseData comData = null, string connectionId = null)
        {
            return ExecReturnFuncAndConnectionId<ConcreteFormInfo>((reInfo, connId) =>
            {
                return persistence.SelectByWorkflowId(workflowId, connId);
            }, null, connectionId, AccessMode.SLAVE);
        }

        /// <summary>
        /// 根据工作流ID移除
        /// </summary>
        /// <param name="workflowId">工作流ID</param>
        /// <param name="connectionId">连接ID</param>
        /// <param name="comData">通用数据</param>
        /// <returns>返回信息</returns>
        public virtual ReturnInfo<bool> RemoveByWorkflowId([DisplayName2("工作流ID"), Id] int workflowId, CommonUseData comData = null, string connectionId = null)
        {
            return ExecReturnFuncAndConnectionId<bool>((reInfo, connId) =>
            {
                return persistence.DeleteByWorkflowId(workflowId, connId) > 0;
            }, null, connectionId);
        }
    }
}
