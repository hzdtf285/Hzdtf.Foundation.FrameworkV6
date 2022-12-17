﻿using Hzdtf.BasicController;
using Hzdtf.BasicFunction.Model;
using Hzdtf.BasicFunction.Service.Contract;
using Hzdtf.Utility.Attr;
using Hzdtf.Utility.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Hzdtf.Utility.Utils;
using Hzdtf.Logger.Contract;
using Hzdtf.Utility.Factory;
using Hzdtf.Utility.Localization;
using Hzdtf.Utility.Model.Page;
using Hzdtf.Utility.RoutePermission;

namespace Hzdtf.BasicFunction.Controller
{
    /// <summary>
    /// 角色控制器
    /// @ 黄振东
    /// </summary>
    [Inject]
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [RoutePermission("Role")]
    public partial class RoleController : PagingControllerBase<int, RoleInfo, IRoleService, PageInfo, KeywordFilterInfo>
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="log">日志</param>
        /// <param name="service">服务</param>
        /// <param name="localize">本地化</param>
        /// <param name="comUseDataFactory">通用数据工厂</param>
        /// <param name="pagingParseFilter">分页解析筛选</param>
        /// <param name="pagingReturnConvert">分页返回转换</param>
        public RoleController(ILogable log = null, IRoleService service = null, ILocalization localize = null, ISimpleFactory<HttpContext, CommonUseData> comUseDataFactory = null,
            IPagingParseFilter pagingParseFilter = null, IPagingReturnConvert pagingReturnConvert = null)
            : base(log, service, localize, comUseDataFactory, pagingParseFilter, pagingReturnConvert)
        {
        }
    }
}
