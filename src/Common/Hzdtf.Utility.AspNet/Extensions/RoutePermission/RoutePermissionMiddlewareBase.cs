﻿using Hzdtf.Utility.RoutePermission;
using Hzdtf.Utility.Model.Return;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Hzdtf.Utility.Utils;
using System.Linq;
using static Hzdtf.Utility.RoutePermission.RoutePermissionInfo;
using Microsoft.AspNetCore.Routing;
using Hzdtf.Utility.Localization;
using Hzdtf.Utility.TheOperation;

namespace Hzdtf.Utility.AspNet.Extensions.RoutePermission
{
    /// <summary>
    /// 路由权限中间件基类
    /// @ 黄振东
    /// </summary>
    public abstract class RoutePermissionMiddlewareBase
    {
        /// <summary>
        /// 下一个中间件处理委托
        /// </summary>
        private readonly RequestDelegate next;

        /// <summary>
        /// 读取API权限配置
        /// </summary>
        protected readonly IRoutePermissionReader reader;

        /// <summary>
        /// API权限选项配置
        /// </summary>
        protected readonly RoutePermissionOptions options;

        /// <summary>
        /// 本地化
        /// </summary>
        protected readonly ILocalization localize;

        /// <summary>
        /// 本次操作
        /// </summary>
        protected readonly ITheOperation theOperation;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="next">下一个中间件处理委托</param>
        /// <param name="options">路由权限选项配置</param>
        /// <param name="reader">读取API权限配置</param>
        /// <param name="localize">本地化</param>
        /// <param name="theOperation">本次操作</param>
        public RoutePermissionMiddlewareBase(RequestDelegate next, IOptions<RoutePermissionOptions> options,
            IRoutePermissionReader reader, ILocalization localize,
            ITheOperation theOperation = null)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("读API权限配置不能为空");
            }

            this.next = next;
            this.options = options.Value;
            this.reader = reader;
            this.localize = localize;
            this.theOperation = theOperation;
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="context">http上下文</param>
        /// <returns>任务</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value.ToLower();
            if (path.StartsWith(options.PfxApiPath))
            {
                var routeValue = context.Request.RouteValues;
                var routes = routeValue.GetControllerAction();
                if (routes.IsNullOrLength0())
                {
                    await next(context);
                    return;
                }

                var routePermisses = reader.Reader();
                if (routePermisses.IsNullOrLength0())
                {
                    await next(context);
                    return;
                }

                var controllerConfig = routePermisses.Where(p => p.Controller == routes[0]).FirstOrDefault();
                if (controllerConfig == null)
                {
                    await next(context);
                    return;
                }
                if (controllerConfig.Disabled)
                {
                    var tempReturn = new BasicReturnInfo();
                    tempReturn.SetFailureMsg(localize.Get(CommonCodeDefine.DISABLED_ACCESS_CULTURE_KEY, "此功能已禁用"));
                    await WriteContent(context, tempReturn);

                    return;
                }
                if (controllerConfig.Actions.IsNullOrLength0())
                {
                    await next(context);
                    return;
                }

                var actionConfig = controllerConfig.Actions.Where(p => p.Action == routes[1]).FirstOrDefault();
                if (actionConfig == null)
                {
                    await next(context);
                    return;
                }
                if (actionConfig.Disabled)
                {
                    var tempReturn = new BasicReturnInfo();
                    tempReturn.SetFailureMsg(localize.Get(CommonCodeDefine.DISABLED_ACCESS_CULTURE_KEY, "此功能已禁用"));
                    await WriteContent(context, tempReturn);

                    return;
                }

                var basicReturn = new BasicReturnInfo();
                var isPer = IsHavePermission(context, controllerConfig, actionConfig, basicReturn);
                if (basicReturn.Failure())
                {
                    await WriteContent(context, basicReturn);
                    return;
                }
                if (isPer)
                {
                    await next(context);
                }
                else
                {
                    await NotPermissionHandle(context);
                }
            }
            else
            {
                await next(context);
            }
        }

        /// <summary>
        /// 判断是否拥有权限
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="controller">控制器信息</param>
        /// <param name="action">动作信息</param>
        /// <param name="basicReturn">基本返回</param>
        /// <returns>获取是否拥有权限</returns>
        protected abstract bool IsHavePermission(HttpContext context, RoutePermissionInfo controller, ActionInfo action, BasicReturnInfo basicReturn);

        /// <summary>
        /// 无权限处理
        /// </summary>
        /// <param name="context">http上下文</param>
        /// <returns>任务</returns>
        protected virtual async Task NotPermissionHandle(HttpContext context)
        {
            var basicReturn = new BasicReturnInfo();
            basicReturn.SetCodeMsg(CommonCodeDefine.NOT_PERMISSION, localize.Get(CommonCodeDefine.NOT_PERMISSION_CULTURE_KEY, "对不起，您没有权限"));

            await WriteContent(context, basicReturn);
        }

        /// <summary>
        /// 写入内容
        /// </summary>
        /// <param name="context">http上下文</param>
        /// <param name="basicReturn">基本返回</param>
        /// <returns>任务</returns>
        protected async Task WriteContent(HttpContext context, BasicReturnInfo basicReturn)
        {
            context.Response.ContentType = "application/json;charset=UTF-8";
            await context.Response.WriteAsync(basicReturn.ToJsonString());
        }
    }

    /// <summary>
    /// 路由权限选项配置
    /// @ 黄振东
    /// </summary>
    public class RoutePermissionOptions
    {
        /// <summary>
        /// Api路径前辍，默认是/api/
        /// </summary>
        public string PfxApiPath
        {
            get;
            set;
        } = "/api/";

        /// <summary>
        /// 配置读取
        /// 可以设置Hzdtf.Utility.RoutePermission.RoutePermissionJson、Hzdtf.Utility.RoutePermission.RoutePermissionAssembly
        /// 默认为RoutePermissionJson
        /// </summary>
        public IRoutePermissionConfigReader ConfigReader
        {
            get;
            set;
        }
    }
}
