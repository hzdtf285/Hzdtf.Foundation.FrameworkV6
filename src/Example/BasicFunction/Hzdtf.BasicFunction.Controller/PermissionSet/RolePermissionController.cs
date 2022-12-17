using Hzdtf.BasicController;
using Hzdtf.BasicFunction.Model;
using Hzdtf.BasicFunction.Model.Expand.Menu;
using Hzdtf.BasicFunction.Service.Contract;
using Hzdtf.Utility.Attr;
using Hzdtf.Utility.Model;
using Hzdtf.Utility.Model.Return;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Text;
using Hzdtf.Logger.Contract;
using Hzdtf.Utility.Factory;
using Hzdtf.Utility.Localization;
using Hzdtf.Utility.Model.Page;
using Hzdtf.Utility.RoutePermission;

namespace Hzdtf.BasicFunction.Controller.PermissionSet
{
    /// <summary>
    /// 角色权限控制器
    /// @ 黄振东
    /// </summary>
    [Inject]
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [RoutePermission("RolePermission")]
    public class RolePermissionController : PagingControllerBase<int, RoleInfo, IRoleService, PageInfo, KeywordFilterInfo>
    {
        /// <summary>
        /// 菜单服务
        /// </summary>
        protected readonly IMenuService menuService;

        /// <summary>
        /// 角色菜单功能服务
        /// </summary>
        protected readonly IRoleMenuFunctionService roleMenuFunctionService;

        /// <summary>
        /// 用户服务
        /// </summary>
        protected readonly IUserService userService;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="log">日志</param>
        /// <param name="service">服务</param>
        /// <param name="localize">本地化</param>
        /// <param name="comUseDataFactory">通用数据工厂</param>
        /// <param name="pagingParseFilter">分页解析筛选</param>
        /// <param name="pagingReturnConvert">分页返回转换</param>
        /// <param name="menuService">菜单服务</param>
        /// <param name="roleMenuFunctionService">角色菜单功能服务</param>
        /// <param name="userService">用户服务</param>
        public RolePermissionController(ILogable log = null, IRoleService service = null, ILocalization localize = null, ISimpleFactory<HttpContext, CommonUseData> comUseDataFactory = null,
            IPagingParseFilter pagingParseFilter = null, IPagingReturnConvert pagingReturnConvert = null,
            IMenuService menuService = null, IRoleMenuFunctionService roleMenuFunctionService = null, IUserService userService = null)
            : base(log, service, localize, comUseDataFactory, pagingParseFilter, pagingReturnConvert)
        {
            this.menuService = menuService;
            this.roleMenuFunctionService = roleMenuFunctionService;
            this.userService = userService;
        }


        /// <summary>
        /// 获取菜单树列表（包含菜单及所拥有的功能列表）
        /// </summary>
        /// <returns>返回信息</returns>
        [HttpGet("MenuTrees")]
        public virtual IList<MenuTreeInfo> MenuTrees() => menuService.QueryMenuTrees(comUseDataFactory.Create(HttpContext)).Data;

        /// <summary>
        /// 获取角色拥有的功能菜单信息列表
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <returns>返回信息</returns>
        [HttpGet("HaveMenuFunctions")]
        public virtual ReturnInfo<IList<MenuFunctionInfo>> HaveMenuFunctions(int roleId) => roleMenuFunctionService.QueryMenuFunctionsByRoleId(roleId, comUseDataFactory.Create(HttpContext));

        /// <summary>
        /// 保存权限
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <param name="menuFunctionIds">菜单功能ID列表</param>
        /// <returns>返回信息</returns>
        [HttpPut("SavePermission")]
        public virtual ReturnInfo<bool> SavePermission(int roleId, IList<int> menuFunctionIds) => roleMenuFunctionService.SaveRoleMenuFunctions(roleId, menuFunctionIds, comUseDataFactory.Create(HttpContext));
        
        /// <summary>
        /// 填充页面数据，包含当前用户所拥有的权限功能列表
        /// </summary>
        /// <param name="returnInfo">返回信息</param>
        /// <param name="comData">通用数据</param>
        protected override void FillPageData(ReturnInfo<PageInfo> returnInfo, CommonUseData comData = null)
        {
            var re = userService.QueryPageData<PageInfo>("RolePermission", () =>
            {
                return returnInfo.Data;
            }, comData: comData);
            returnInfo.FromBasic(re);
        }

        /// <summary>
        /// 创建页面数据
        /// </summary>
        /// <param name="comData">通用数据</param>
        /// <returns>页面数据</returns>
        protected override PageInfo CreatePageData(CommonUseData comData = null) => new PageInfo();
    }
}
