using Hzdtf.Utility.Model;
using Hzdtf.Utility.Model.Return;
using Hzdtf.Workflow.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hzdtf.Workflow.Service.Impl
{
    /// <summary>
    /// 工作流辅助类
    /// @ 黄振东
    /// </summary>
    public static class WorkflowUtil
    {
        /// <summary>
        /// 判断当前用户能否审核
        /// </summary>
        /// <param name="workflowHandle">工作流处理</param>
        /// <param name="comData">通用数据</param>
        /// <returns>当前用户能否审核</returns>
        public static BasicReturnInfo CanCurrUserAudit(WorkflowHandleInfo workflowHandle, CommonUseData comData = null)
        {
            BasicReturnInfo returnInfo = new BasicReturnInfo();
            var user = UserTool<int>.GetCurrUser(comData);
            if (user == null)
            {
                returnInfo.SetFailureMsg("您还未登录，请先登录系统");

                return returnInfo;
            }

            if (workflowHandle == null)
            {
                returnInfo.SetFailureMsg("找不到处理信息");

                return returnInfo;
            }

            if (workflowHandle.HandlerId != user.Id)
            {
                returnInfo.SetFailureMsg("Sorry,您不是此流程的处理者,无权限审核");

                return returnInfo;
            }

            if (workflowHandle.HandleStatus == HandleStatusEnum.EFFICACYED)
            {
                returnInfo.SetFailureMsg("Sorry,您的处理信息已无效");

                return returnInfo;
            }
            if (workflowHandle.HandleStatus == HandleStatusEnum.SENDED || workflowHandle.HandleStatus == HandleStatusEnum.RETURNED)
            {
                returnInfo.SetFailureMsg("Sorry,您的处理信息已处理，无需重复处理");

                return returnInfo;
            }

            return returnInfo;
        }
    }
}
