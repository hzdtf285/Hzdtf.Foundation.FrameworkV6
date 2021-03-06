using Hzdtf.BasicFunction.Service.Contract;
using Hzdtf.Utility.Attr;
using Hzdtf.Utility.Model;
using Hzdtf.Utility.Model.Return;
using Hzdtf.Workflow.Model.Expand;
using Hzdtf.Workflow.Service.Impl.Engine;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hzdtf.BasicFunction.Workflow
{
    /// <summary>
    /// 工作流初始序列服务
    /// @ 黄振东
    /// </summary>
    [Inject]
    public class WorkflowInitSequenceService : WorkflowFormService
    {
        /// <summary>
        /// 序列服务
        /// </summary>
        public ISequenceService SequenceService
        {
            get;
            set;
        }

        /// <summary>
        /// 生成申请单号
        /// </summary>
        /// <typeparam name="FormT">表单类型</typeparam>
        /// <param name="flowInit">流程初始化</param>
        /// <param name="returnInfo">返回信息</param>
        /// <param name="comData">通用数据</param>
        /// <returns>申请单号</returns>
        protected override string BuilderApplyNo<FormT>(FlowInitInfo<FormT> flowInit, ReturnInfo<WorkflowBasicInfo> returnInfo, CommonUseData comData = null)
        {
            var buildNoReturnInfo = SequenceService.BuildNo(flowInit.WorkflowCode, comData: comData);
            if (buildNoReturnInfo.Failure())
            {
                returnInfo.FromBasic(buildNoReturnInfo);

                return null;
            }

            return buildNoReturnInfo.Data;
        }
    }
}
