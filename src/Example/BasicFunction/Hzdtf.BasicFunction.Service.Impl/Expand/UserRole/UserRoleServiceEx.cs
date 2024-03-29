﻿using Hzdtf.BasicFunction.Model;
using Hzdtf.Utility.Attr;
using Hzdtf.Utility.Attr.ParamAttr;
using Hzdtf.Utility.Model.Return;
using System;
using System.Collections.Generic;
using System.Text;
using Hzdtf.Utility.Utils;
using System.ComponentModel.DataAnnotations;
using Hzdtf.Utility.Model;

namespace Hzdtf.BasicFunction.Service.Impl
{
    /// <summary>
    /// 用户角色服务
    /// @ 黄振东
    /// </summary>
    public partial class UserRoleService
    {
        /// <summary>
        /// 根据用户ID查询拥有的角色列表
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="connectionId">连接ID</param>
        /// <param name="comData">通用数据</param>
        /// <returns>返回信息</returns>
        public virtual ReturnInfo<IList<RoleInfo>> OwnRolesByUserId([DisplayName2("用户ID"), Id] int userId, CommonUseData comData = null, string connectionId = null)
        {
            return ExecReturnFunc<IList<RoleInfo>>((reInfo) =>
            {
                return persistence.SelectRolesByUserId(userId, connectionId);
            });
        }

        /// <summary>
        /// 根据角色ID查询拥有的用户列表
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <param name="connectionId">连接ID</param>
        /// <param name="comData">通用数据</param>
        /// <returns>返回信息</returns>
        public virtual ReturnInfo<IList<UserInfo>> OwnUsersByRoleId([DisplayName2("角色ID"), Id] int roleId, CommonUseData comData = null, string connectionId = null)
        {
            return ExecReturnFunc<IList<UserInfo>>((reInfo) =>
            {
                IList<UserInfo> users = persistence.SelectUsersByRoleId(roleId, connectionId);
                if (users.IsNullOrCount0())
                {
                    return users;
                }

                foreach (var u in users)
                {
                    u.Password = null;
                }

                return users;
            });
        }

        /// <summary>
        /// 根据角色编码查询拥有的用户列表
        /// </summary>
        /// <param name="roleCode">角色编码</param>
        /// <param name="connectionId">连接ID</param>
        /// <param name="comData">通用数据</param>
        /// <returns>返回信息</returns>
        public virtual ReturnInfo<IList<UserInfo>> OwnUsersByRoleCode([DisplayName2("角色编码"), Required] string roleCode, CommonUseData comData = null, string connectionId = null)
        {
            return ExecReturnFunc<IList<UserInfo>>((reInfo) =>
            {
                IList<UserInfo> users = persistence.SelectUsersByRoleCode(roleCode, connectionId);
                if (users.IsNullOrCount0())
                {
                    return users;
                }

                foreach (var u in users)
                {
                    u.Password = null;
                }

                return users;
            });
        }
    }
}
