﻿using Hzdtf.Autofac.Extensions;
using Hzdtf.BasicFunction.Service.Contract;
using Hzdtf.Utility.Model.Return;
using Hzdtf.Workflow.Service.Contract;
using System;
using System.Collections.Generic;
using System.Text;
using Hzdtf.Utility.Utils;
using Hzdtf.Utility.Model;
using Hzdtf.Utility.Model.Identitys;
using Hzdtf.Utility;
using NPOI.SS.Formula.Functions;

namespace Hzdtf.BasicFunction.Workflow
{
    /// <summary>
    /// 用户工作流辅助类
    /// @ 黄振东
    /// </summary>
    public static class UserWorkflowUtil
    {
        /// <summary>
        /// 初始化用户处理验证
        /// 注册用户删除前判断是否有处理的事件
        /// </summary>
        public static void InitValiUserHandleVali()
        {
            IUserService userService = App.GetServiceFromInstance<IUserService>();
            userService.RemoveByIding += UserService_RemoveByIding;
            userService.RemoveByIdsing += UserService_RemoveByIdsing;
        }

        /// <summary>
        /// 根据用户ID移除前事件
        /// </summary>
        /// <param name="arg1">返回信息</param>
        /// <param name="arg2">用户ID</param>
        /// <param name="comData">通用数据</param>
        /// <param name="arg3">连接ID</param>
        private static void UserService_RemoveByIdsing(ReturnInfo<bool> arg1, int[] arg2, CommonUseData comData = null, string arg3 = null)
        {
            IWorkflowHandleService workflowHandleService = App.GetServiceFromInstance<IWorkflowHandleService>();
            ReturnInfo<bool[]> handleReturnInfo = workflowHandleService.ExistsAuditAndUnhandleByHandleIds(arg2, comData, arg3);
            if (handleReturnInfo.Failure())
            {
                arg1.FromBasic(handleReturnInfo);
                return;
            }

            if (handleReturnInfo.Data.IsNullOrLength0())
            {
                return;
            }

            for (var i = 0; i < handleReturnInfo.Data.Length; i++)
            {
                if (handleReturnInfo.Data[i])
                {
                    arg1.SetFailureMsg($"第{i + 1}行：用户尚有未处理的审核流程，故不能移除");
                    return;
                }
            }
        }

        /// <summary>
        /// 根据用户ID移除前事件
        /// </summary>
        /// <param name="arg1">返回信息</param>
        /// <param name="arg2">用户ID</param>
        /// <param name="comData">通用数据</param>
        /// <param name="arg3">连接ID</param>
        private static void UserService_RemoveByIding(ReturnInfo<bool> arg1, int arg2, CommonUseData comData = null, string arg3 = null)
        {
            IWorkflowHandleService workflowHandleService = App.GetServiceFromInstance<IWorkflowHandleService>();
            ReturnInfo<bool> handleReturnInfo = workflowHandleService.ExistsAuditAndUnhandleByHandleId(arg2, comData, arg3);
            if (handleReturnInfo.Failure())
            {
                arg1.FromBasic(handleReturnInfo);
                return;
            }

            if (handleReturnInfo.Data)
            {
                arg1.SetFailureMsg("用户尚有未处理的审核流程，故不能移除");
            }
        }
    }
}
