﻿using Hzdtf.BasicFunction.Model;
using Hzdtf.BasicFunction.Model.Expand.User;
using Hzdtf.Utility.Attr;
using Hzdtf.Utility.Enums;
using Hzdtf.Utility.Model.Return;
using Hzdtf.Utility.Safety;
using System;
using System.Collections.Generic;
using System.Text;
using Hzdtf.Utility.Utils;
using Hzdtf.Utility.Model;
using Hzdtf.BasicFunction.Persistence.Contract;
using Hzdtf.BasicFunction.Model.Expand.Menu;
using Hzdtf.BasicFunction.Service.Contract.User;
using Hzdtf.BasicFunction.Service.Contract;
using Hzdtf.Utility.Attr.ParamAttr;
using System.ComponentModel.DataAnnotations;
using Hzdtf.Utility.AutoMapperExtensions;
using Hzdtf.AUC.Contract.User;
using Hzdtf.Utility;

namespace Hzdtf.BasicFunction.Service.Impl
{
    /// <summary>
    /// 用户服务
    /// @ 黄振东
    /// </summary>
    public partial class UserService : IUserVali<int, UserInfo>, IUserMenuService
    {
        #region 属性与字段

        /// <summary>
        /// 菜单持久化
        /// </summary>
        public IMenuPersistence MenuPersistence
        {
            get;
            set;
        }

        /// <summary>
        /// 用户角色服务
        /// </summary>
        public IUserRoleService UserRoleService
        {
            get;
            set;
        }

        /// <summary>
        /// 角色菜单功能持久化
        /// </summary>
        public IRoleMenuFunctionPersistence RoleMenuFunctionPersistence
        {
            get;
            set;
        }

        /// <summary>
        /// 用户菜单功能持久化
        /// </summary>
        public IUserMenuFunctionPersistence UserMenuFunctionPersistence
        {
            get;
            set;
        }

        #endregion

        #region IUserService 接口

        /// <summary>
        /// 根据登录ID修改密码
        /// </summary>
        /// <param name="currUserModifyPassword">当前用户修改密码</param>
        /// <param name="connectionId">连接ID</param>
        /// <param name="comData">通用数据</param>
        /// <returns>返回信息</returns>
        public virtual ReturnInfo<bool> ModifyPasswordByLoginId([Model] CurrUserModifyPasswordInfo currUserModifyPassword, CommonUseData comData = null, string connectionId = null)
        {
            return ExecReturnFuncAndConnectionId<bool>((reInfo, connId) =>
            {
                UserInfo user = persistence.SelectByLoginIdAndPassword(currUserModifyPassword.LoginId, MD5Util.Encryption16(currUserModifyPassword.OldPassword), connId);
                if (user == null)
                {
                    reInfo.SetFailureMsg("旧密码不对");
                    return false;
                }

                user.Password = MD5Util.Encryption16(currUserModifyPassword.NewPassword);
                user.SetModifyInfo(UserTool<int>.GetCurrUser(comData));

                bool result = persistence.UpdatePasswordById(user, connId) > 0;
                if (result)
                {
                    reInfo.SetSuccessMsg("修改成功，请记住新密码！");
                }

                return result;
            }, null, connectionId);
        }

        /// <summary>
        /// 重置用户密码
        /// </summary>
        /// <param name="modifyPassword">修改密码</param>
        /// <param name="connectionId">连接ID</param>
        /// <param name="comData">通用数据</param>
        /// <returns>返回信息</returns>
        public virtual ReturnInfo<bool> ResetUserPassword([Model] ModifyPasswordInfo modifyPassword, CommonUseData comData = null, string connectionId = null)
        {
            return ExecReturnFuncAndConnectionId<bool>((reInfo, connId) =>
            {
                UserInfo user = new UserInfo()
                {
                    Id = modifyPassword.Id,
                    Password = MD5Util.Encryption16(modifyPassword.NewPassword)
                };
                user.SetModifyInfo(UserTool<int>.GetCurrUser(comData));

                bool result = persistence.UpdatePasswordById(user, connId) > 0;
                if (result)
                {
                    reInfo.SetSuccessMsg("修改成功，请记住新密码！");
                }

                return result;
            }, null, connectionId);
        }

        /// <summary>
        /// 根据菜单编码和功能编码判断当前用户是否有权限
        /// </summary>
        /// <param name="menuCode">菜单编码</param>
        /// <param name="functionCode">功能编码</param>
        /// <param name="connectionId">连接ID</param>
        /// <param name="comData">通用数据</param>
        /// <returns>返回信息</returns>
        public virtual ReturnInfo<bool> IsCurrUserPermission(string menuCode, string functionCode, CommonUseData comData = null, string connectionId = null)
        {
            return IsCurrUserPermission(menuCode, new string[] { functionCode }, comData, connectionId);
        }

        /// <summary>
        /// 根据菜单编码和功能编码集合判断当前用户是否有权限
        /// </summary>
        /// <param name="menuCode">菜单编码</param>
        /// <param name="functionCodes">功能编码集合</param>
        /// <param name="connectionId">连接ID</param>
        /// <param name="comData">通用数据</param>
        /// <returns>返回信息</returns>
        public virtual ReturnInfo<bool> IsCurrUserPermission(string menuCode, string[] functionCodes, CommonUseData comData = null, string connectionId = null)
        {
            var user = UserTool<int>.GetCurrUser(comData);
            return IsPermission(user.Id, menuCode, functionCodes, connectionId: connectionId, comData: comData);
        }

        /// <summary>
        /// 根据用户ID、菜单编码和功能编码集合判断用户是否有权限
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="menuCode">菜单编码</param>
        /// <param name="functionCodes">功能编码集合</param>
        /// <param name="connectionId">连接ID</param>
        /// <param name="comData">通用数据</param>
        /// <returns>返回信息</returns>
        public virtual ReturnInfo<bool> IsPermission([DisplayName2("用户ID"), Id] int userId, [DisplayName2("菜单编码"), Required] string menuCode,
            [DisplayName2("功能编码集合"), ArrayNotEmpty] string[] functionCodes, CommonUseData comData = null, string connectionId = null)
        {
            if ("Role".Equals(Config["User:PermissionBenchmark"]))
            {
                return ExecReturnFuncAndConnectionId<bool>((reInfo, connId) =>
                {
                    // 查找该用户所属角色
                    ReturnInfo<IList<RoleInfo>> reRoleInfo = UserRoleService.OwnRolesByUserId(userId, comData, connId);
                    if (reRoleInfo.Failure())
                    {
                        reInfo.FromBasic(reRoleInfo);
                        return false;
                    }
                    if (reRoleInfo.Data.IsNullOrCount0())
                    {
                        return false;
                    }
                    int[] roleIds = new int[reRoleInfo.Data.Count];
                    for (var i = 0; i < roleIds.Length; i++)
                    {
                        roleIds[i] = reRoleInfo.Data[i].Id;
                    }

                    if (RoleMenuFunctionPersistence.CountByMenuCodeAndFunctionCodesAndRoleIds(menuCode, functionCodes, roleIds, connId) > 0)
                    {
                        return true;
                    }
                    else
                    {
                        reInfo.SetCodeMsg(CommonCodeDefine.NOT_PERMISSION, "Sorry,您没有访问此功能权限！");
                        return false;
                    }
                }, null, connectionId, AccessMode.SLAVE);                    
            }
            else
            {
                return ExecReturnFunc<bool>((reInfo) =>
                {
                    if (UserMenuFunctionPersistence.CountByMenuCodeAndFunctionCodesAndUserId(menuCode, functionCodes, userId, connectionId) > 0)
                    {
                        return true;
                    }
                    else
                    {
                        reInfo.SetCodeMsg(CommonCodeDefine.NOT_PERMISSION, "Sorry,您没有访问此功能权限！");
                        return false;
                    }
                });
            }
        }

        /// <summary>
        /// 根据菜单编码查询当前用户所拥有的功能信息列表
        /// </summary>
        /// <param name="menuCode">菜单编码</param>
        /// <param name="connectionId">连接ID</param>
        /// <param name="comData">通用数据</param>
        /// <returns>返回信息</returns>
        public virtual ReturnInfo<IList<FunctionInfo>> QueryCurrUserOwnFunctionsByMenuCode([DisplayName2("菜单编码"), Required] string menuCode, CommonUseData comData = null, string connectionId = null)
        {
            var user = UserTool<int>.GetCurrUser(comData);
            return QueryUserOwnFunctionsByMenuCode(user.Id, menuCode, connectionId: connectionId, comData: comData);
        }

        /// <summary>
        /// 根据用户ID和菜单编码查询用户所拥有的功能信息列表
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="menuCode">菜单编码</param>
        /// <param name="connectionId">连接ID</param>
        /// <param name="comData">通用数据</param>
        /// <returns>返回信息</returns>
        public virtual ReturnInfo<IList<FunctionInfo>> QueryUserOwnFunctionsByMenuCode([DisplayName2("用户ID"), Id] int userId, [DisplayName2("菜单编码"), Required] string menuCode, CommonUseData comData = null, string connectionId = null)
        {
            if ("Role".Equals(Config["User:PermissionBenchmark"]))
            {
                return ExecReturnFuncAndConnectionId<IList<FunctionInfo>>((reInfo, connId) =>
                {
                    // 查找该用户所属角色
                    ReturnInfo<IList<RoleInfo>> reRoleInfo = UserRoleService.OwnRolesByUserId(userId, connectionId: connId, comData: comData);
                    if (reRoleInfo.Failure())
                    {
                        reInfo.FromBasic(reRoleInfo);
                        return null;
                    }
                    if (reRoleInfo.Data.IsNullOrCount0())
                    {
                        return null;
                    }
                    int[] roleIds = new int[reRoleInfo.Data.Count];
                    for (var i = 0; i < roleIds.Length; i++)
                    {
                        roleIds[i] = reRoleInfo.Data[i].Id;
                    }

                    return RoleMenuFunctionPersistence.SelectFunctionsByMenuCodeAndRoleIds(menuCode, roleIds, connId);
                }, null, connectionId, AccessMode.SLAVE);
            }
            else
            {
                return ExecReturnFunc<IList<FunctionInfo>>((reInfo) =>
                {
                    return UserMenuFunctionPersistence.SelectFunctionsByMenuCodeAndUserId(menuCode, userId, connectionId);
                });
            }
        }

        /// <summary>
        /// 判断当前用户是否是系统管理组
        /// </summary>
        /// <param name="connectionId">连接ID</param>
        /// <param name="comData">通用数据</param>
        /// <returns>返回信息</returns>
        public virtual ReturnInfo<bool> IsCurrUserAdministrators(CommonUseData comData = null, string connectionId = null)
        {
            var user = UserTool<int>.GetCurrUser(comData);
            return IsUserAdministrators(user.Id, connectionId: connectionId, comData: comData);
        }

        /// <summary>
        /// 判断用户是否是系统管理组
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="connectionId">连接ID</param>
        /// <param name="comData">通用数据</param>
        /// <returns>返回信息</returns>
        public virtual ReturnInfo<bool> IsUserAdministrators([DisplayName2("用户ID"), Id] int userId, CommonUseData comData = null, string connectionId = null)
        {
            ReturnInfo<bool> returnInfo = new ReturnInfo<bool>();
            IList<RoleInfo> roles = UserRolePersistence.SelectRolesByUserId(userId, connectionId);
            if (roles.IsNullOrCount0())
            {
                return returnInfo;
            }

            foreach (RoleInfo r in roles)
            {
                if ("Administrators".Equals(r.Code))
                {
                    returnInfo.Data = true;

                    return returnInfo;
                }
            }

            return returnInfo;
        }

        /// <summary>
        /// 根据筛选条件查询用户列表
        /// </summary>
        /// <param name="filter">筛选</param>
        /// <param name="connectionId">连接ID</param>
        /// <param name="comData">通用数据</param>
        /// <returns>返回信息</returns>
        public virtual ReturnInfo<IList<UserInfo>> QueryByFilter(UserFilterInfo filter, CommonUseData comData = null, string connectionId = null)
        {
            return ExecReturnFunc<IList<UserInfo>>((reInfo) =>
            {
                return persistence.SelectByFilter(filter, connectionId);
            });
        }

        /// <summary>
        /// 查询页面数据
        /// </summary>
        /// <typeparam name="PageInfoT">页面信息类型</typeparam>
        /// <param name="menuCode">菜单编码</param>
        /// <param name="createPage">创建页面数据回调</param>
        /// <param name="appendPageData">追加页面数据回调</param>
        /// <param name="comData">通用数据</param>
        /// <returns></returns>
        public virtual ReturnInfo<PageInfoT> QueryPageData<PageInfoT>(string menuCode, Func<PageInfoT> createPage, Action<ReturnInfo<PageInfoT>> appendPageData = null, CommonUseData comData = null)
            where PageInfoT : PageInfo<int>
        {
            var returnInfo = new ReturnInfo<PageInfoT>();
            returnInfo.Data = createPage();
            if (string.IsNullOrWhiteSpace(menuCode))
            {
                if (appendPageData != null)
                {
                    appendPageData(returnInfo);
                }
                return returnInfo;
            }

            var reFunInfo = QueryCurrUserOwnFunctionsByMenuCode(menuCode, comData: comData);
            if (reFunInfo.Success())
            {
                if (reFunInfo.Data.IsNullOrCount0())
                {
                    return returnInfo;
                }

                returnInfo.Data.Functions = new List<CodeNameInfo<int>>(reFunInfo.Data.Count);
                foreach (var f in reFunInfo.Data)
                {
                    returnInfo.Data.Functions.Add(new CodeNameInfo<int>()
                    {
                        Code = f.Code,
                        Name = f.Name
                    });
                }

                if (appendPageData != null)
                {
                    appendPageData(returnInfo);
                }
            }
            else
            {
                returnInfo.FromBasic(reFunInfo);
            }

            return returnInfo;
        }

        #endregion

        #region IUserVali<UserInfo> 接口

        /// <summary>
        /// 验证
        /// </summary>
        /// <param name="user">用户</param>
        /// <param name="password">密码</param>
        /// <param name="comData">通用数据</param>
        /// <returns>返回信息</returns>
        [ProcTrackLog(IgnoreParamValues = true)]
        public virtual ReturnInfo<UserInfo> Vali([DisplayName2("用户"), Required] string user, [DisplayName2("密码"), Required] string password, CommonUseData comData = null)
        {
            ReturnInfo<UserInfo> returnInfo = new ReturnInfo<UserInfo>();

            return ExecReturnFuncAndConnectionId<UserInfo>((reInfo, connId) =>
            {
                UserInfo result = persistence.SelectByLoginIdAndPassword(user, MD5Util.Encryption16(password), connId);
                if (result == null)
                {
                    reInfo.SetFailureMsg("登录ID或密码不对");
                    return null;
                }

                if (result.Enabled)
                {
                    returnInfo.Data = result;
                    reInfo.SetSuccessMsg("登录成功");

                    result.Memo = result.Password = result.Modifier = result.Creater = null;

                    log.InfoAsync($"登录ID:{result.LoginId},用户编号:{result.Code},名称:{result.Name} 成功授权",
                        null, typeof(UserService).Name, eventId: comData.GetEventId());
                }
                else
                {
                    reInfo.SetFailureMsg("Sorry.您的登录ID未启用,不能登录此系统");
                }

                return result;
            }, returnInfo, accessMode: AccessMode.SLAVE);
        }

        #endregion

        #region IUserMenuService 接口

        /// <summary>
        /// 根据用户ID获取能访问的菜单列表
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="connectionId">连接ID</param>
        /// <param name="comData">通用数据</param>
        /// <returns>返回信息</returns>
        public virtual ReturnInfo<UserMenuInfo> CanAccessMenus([DisplayName2("用户ID"), Id] int userId, CommonUseData comData = null, string connectionId = null)
        {
            return ExecReturnFuncAndConnectionId<UserMenuInfo>((reInfo, connId) =>
            {
                ReturnInfo<UserInfo> protoReturnInfo = Find(userId, connectionId: connId, comData: comData);
                if (protoReturnInfo.Failure())
                {
                    reInfo.FromBasic(protoReturnInfo);
                    return null;
                }
                if (protoReturnInfo.Data == null)
                {
                    reInfo.SetFailureMsg("找不到该用户");
                    return null;
                }
                protoReturnInfo.Data.Password = null;

                // 查找该用户所属角色
                ReturnInfo<IList<RoleInfo>> reRoleInfo = UserRoleService.OwnRolesByUserId(userId, connectionId: connId, comData: comData);
                if (reRoleInfo.Failure())
                {
                    reInfo.FromBasic(reRoleInfo);
                    return null;
                }
                protoReturnInfo.Data.OwnRoles = reRoleInfo.Data;                

                var result = AutoMapperUtil.Mapper.Map<UserInfo, UserMenuInfo>(protoReturnInfo.Data);

                // 查找能拥有的权限菜单
                IList<MenuInfo> menus = null;
                if ("Role".Equals(Config["User:PermissionBenchmark"]))
                {
                    if (!reRoleInfo.Data.IsNullOrCount0())
                    {
                        int[] roleIds = new int[reRoleInfo.Data.Count];
                        for (var i = 0; i < roleIds.Length; i++)
                        {
                            roleIds[i] = reRoleInfo.Data[i].Id;
                        }
                        menus = MenuPersistence.SelectByRoleIds(roleIds, connId);
                    }
                }
                else
                {
                    menus = MenuPersistence.SelectByUserId(userId, connId);
                }
                
                if (menus.IsNullOrCount0())
                {
                    return result;
                }

                // 如果没有查询权限则需要移除
                IList<MenuInfo> tempMenus = new List<MenuInfo>();
                foreach (MenuInfo m in menus)
                {
                    if (m.Functions.IsNullOrCount0())
                    {
                        continue;
                    }

                    foreach (FunctionInfo funInfo in m.Functions)
                    {
                        if (FunCodeDefine.QUERY_CODE.Equals(funInfo.Code))
                        {
                            tempMenus.Add(m);
                            break;
                        }
                    }
                }

                result.Menus = tempMenus.ToOrigAndSort();

                return result;
            }, connectionId: connectionId, accessMode: AccessMode.SLAVE);
        }

        #endregion

        #region 重写父类的方法

        /// <summary>
        /// 添加模型前
        /// </summary>
        /// <param name="returnInfo">返回信息</param>
        /// <param name="model">模型</param>
        /// <param name="connectionId">连接ID</param>
        /// <param name="comData">通用数据</param>
        protected override void BeforeAdd(ReturnInfo<bool> returnInfo, UserInfo model, ref string connectionId, CommonUseData comData = null)
        {
            bool idClose = false;
            if (string.IsNullOrWhiteSpace(connectionId))
            {
                idClose = true;
                connectionId = persistence.NewConnectionId();
            }
            try
            {
                UserInfo exists = persistence.SelectByLoginIdOrCodeOrName(model.LoginId, model.Code, model.Name, connectionId);
                ValiExistsParam(returnInfo, model, exists);

                if (returnInfo.Success())
                {
                    model.Password = MD5Util.Encryption16(model.Password);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            finally
            {
                if (idClose && returnInfo.Failure())
                {
                    persistence.Release(connectionId);
                }
            }
        }

        /// <summary>
        /// 根据ID查找模型后
        /// </summary>
        /// <param name="returnInfo">返回信息</param>
        /// <param name="id">ID</param>
        /// <param name="connectionId">连接ID</param>
        /// <param name="comData">通用数据</param>
        protected override void AfterFind(ReturnInfo<UserInfo> returnInfo, int id, ref string connectionId, CommonUseData comData = null)
        {
            if (returnInfo.Success() && returnInfo.Data != null)
            {
                returnInfo.Data.Password = null;
            }
        }

        /// <summary>
        /// 根据ID修改模型前
        /// </summary>
        /// <param name="returnInfo">返回信息</param>
        /// <param name="model">模型</param>
        /// <param name="connectionId">连接ID</param>
        /// <param name="comData">通用数据</param>
        protected override void BeforeModifyById(ReturnInfo<bool> returnInfo, UserInfo model, ref string connectionId, CommonUseData comData = null)
        {
            bool idClose = false;
            if (string.IsNullOrWhiteSpace(connectionId))
            {
                idClose = true;
                connectionId = persistence.NewConnectionId();
            }
            try
            {
                UserInfo exists = persistence.SelectByLoginIdOrCodeOrNameNotId(model.Id, model.LoginId, model.Code, model.Name, connectionId);                
                ValiExistsParam(returnInfo, model, exists);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            finally
            {
                if (idClose && returnInfo.Failure())
                {
                    persistence.Release(connectionId);
                }
            }
        }

        /// <summary>
        /// 添加模型列表前
        /// </summary>
        /// <param name="returnInfo">返回信息</param>
        /// <param name="models">模型列表</param>        
        /// <param name="connectionId">连接ID</param>
        /// <param name="comData">通用数据</param>
        protected override void BeforeAdd(ReturnInfo<bool> returnInfo, IList<UserInfo> models, ref string connectionId, CommonUseData comData = null)
        {
            for (var i = 0; i < models.Count; i++)
            {
                BeforeAdd(returnInfo, models[i], ref connectionId);
                if (returnInfo.Failure())
                {
                    returnInfo.SetFailureMsg($"第{i + 1}行:{returnInfo.Msg}");
                    return;
                }
            }
        }

        /// <summary>
        /// 根据ID移除模型前
        /// </summary>
        /// <param name="returnInfo">返回信息</param>
        /// <param name="id">ID</param>
        /// <param name="connectionId">连接ID</param>
        /// <param name="comData">通用数据</param>
        /// <returns>返回信息</returns>
        protected override void BeforeRemoveById(ReturnInfo<bool> returnInfo, int id, ref string connectionId, CommonUseData comData = null)
        {
            ValiCanRemove(returnInfo, persistence.Select(id, connectionId: connectionId, comData: comData));
        }

        /// <summary>
        /// 根据ID集合移除模型前
        /// </summary>
        /// <param name="returnInfo">返回信息</param>
        /// <param name="ids">ID集合</param>
        /// <param name="connectionId">连接ID</param>
        /// <param name="comData">通用数据</param>
        /// <returns>返回信息</returns>
        protected override void BeforeRemoveByIds(ReturnInfo<bool> returnInfo, int[] ids, ref string connectionId, CommonUseData comData = null)
        {
            IList<UserInfo> users = persistence.Select(ids, connectionId: connectionId, comData: comData);
            if (users.IsNullOrCount0())
            {
                return;
            }

            foreach (UserInfo user in users)
            {
                ValiCanRemove(returnInfo, user);
                if (returnInfo.Failure())
                {
                    return;
                }
            }
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 验证存在的参数
        /// </summary>
        /// <param name="returnInfo">返回信息</param>
        /// <param name="model">模型</param>
        /// <param name="exists">存在的模型</param>
        private void ValiExistsParam(ReturnInfo<bool> returnInfo, UserInfo model, UserInfo exists)
        {
            if (exists == null)
            {
                return;
            }

            if (string.Compare(model.LoginId, exists.LoginId, true) == 0)
            {
                returnInfo.SetFailureMsg($"登录ID:{model.LoginId}已存在");
                return;
            }
            if (string.Compare(model.Code, exists.Code, true) == 0)
            {
                returnInfo.SetFailureMsg($"编码:{model.Code}已存在");
                return;
            }
            if (string.Compare(model.Name, exists.Name, true) == 0)
            {
                returnInfo.SetFailureMsg($"名称:{model.Name}已存在");
                return;
            }
        }

        /// <summary>
        /// 验证是否能移除
        /// </summary>
        /// <param name="returnInfo">返回信息</param>
        /// <param name="user">用户</param>
        private void ValiCanRemove(ReturnInfo<bool> returnInfo, UserInfo user)
        {
            if (user == null)
            {
                return;
            }

            if (user.SystemInlay)
            {
                returnInfo.SetFailureMsg($"用户[{user.Name}]是系统内置不能删除");
                return;
            }

            if (user.SystemHide)
            {
                returnInfo.SetFailureMsg($"用户[{user.Name}]是系统隐藏不能删除");
                return;
            }
        }

        #endregion
    }
}
