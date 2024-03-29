﻿using Hzdtf.BasicFunction.Model;
using Hzdtf.BasicFunction.Persistence.Contract;
using Hzdtf.Utility.Attr;
using Hzdtf.Utility.Model.Return;
using System;
using System.Collections.Generic;
using System.Text;
using Hzdtf.Utility.Utils;
using Hzdtf.Utility.Attr.ParamAttr;
using Hzdtf.Utility.Model;

namespace Hzdtf.BasicFunction.Service.Impl
{
    /// <summary>
    /// 角色菜单功能服务
    /// @ 黄振东
    /// </summary>
    public partial class RoleMenuFunctionService
    {
        /// <summary>
        /// 角色菜单功能持久化
        /// </summary>
        public IRoleMenuFunctionPersistence RoleMenuFunctionPersistence
        {
            get;
            set;
        }

        /// <summary>
        /// 根据角色ID查询菜单功能信息列表
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <param name="connectionId">连接ID</param>
        /// <param name="comData">通用数据</param>
        /// <returns>返回信息</returns>
        public virtual ReturnInfo<IList<MenuFunctionInfo>> QueryMenuFunctionsByRoleId([DisplayName2("角色ID"), Id] int roleId, CommonUseData comData = null, string connectionId = null)
        {
            return ExecReturnFunc<IList<MenuFunctionInfo>>((reInfo) =>
            {
                return RoleMenuFunctionPersistence.SelectMenuFunctionsByRoleId(roleId, connectionId);
            });
        }

        /// <summary>
        /// 保存角色拥有的菜单功能信息列表
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <param name="menuFunctionIds">菜单功能ID列表</param>
        /// <param name="connectionId">连接ID</param>
        /// <param name="comData">通用数据</param>
        /// <returns>返回信息</returns>
        public virtual ReturnInfo<bool> SaveRoleMenuFunctions([DisplayName2("角色ID"), Id] int roleId, IList<int> menuFunctionIds, CommonUseData comData = null, string connectionId = null)
        {
            IList<RoleMenuFunctionInfo> rmfs = new List<RoleMenuFunctionInfo>(menuFunctionIds.Count);
            foreach (var id in menuFunctionIds)
            {
                RoleMenuFunctionInfo rmf = new RoleMenuFunctionInfo()
                {
                    RoleId = roleId,
                    MenuFunctionId = id
                };
                rmf.SetCreateInfo(UserTool<int>.GetCurrUser(comData));

                rmfs.Add(rmf);
            }

            ReturnInfo<bool> returnInfo = new ReturnInfo<bool>();
            ExecSaveRoleMenuFunctions(returnInfo, roleId, rmfs, connectionId: connectionId, comData: comData);

            return returnInfo;
        }

        /// <summary>
        /// 执行保存保存角色拥有的菜单功能信息列表
        /// </summary>
        /// <param name="returnInfo">返回信息</param>
        /// <param name="roleId">角色ID</param>
        /// <param name="rmfs">角色菜单功能列表</param>
        /// <param name="connectionId">连接ID</param>
        /// <param name="comData">通用数据</param>
        [Transaction(ConnectionIdIndex = 4)]
        protected virtual void ExecSaveRoleMenuFunctions(ReturnInfo<bool> returnInfo, int roleId, IList<RoleMenuFunctionInfo> rmfs, CommonUseData comData = null, string connectionId = null)
        {
            persistence.DeleteByRoleId(roleId, connectionId);
            if (rmfs.IsNullOrCount0())
            {
                return;
            }

            returnInfo.Data = persistence.Insert(rmfs, connectionId: connectionId, comData: comData) > 0;
        }
    }
}
