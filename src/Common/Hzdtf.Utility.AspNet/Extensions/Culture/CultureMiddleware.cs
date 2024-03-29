﻿using Hzdtf.Utility.Localization;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Hzdtf.Utility.Utils;

namespace Hzdtf.Utility.AspNet.Extensions.Culture
{
    /// <summary>
    /// 文化中间件
    /// @ 黄振东
    /// </summary>
    public class CultureMiddleware
    {
        /// <summary>
        /// 下一个中间件处理委托
        /// </summary>
        private readonly RequestDelegate next;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="next">下一个中间件处理委托</param>
        public CultureMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="context">http上下文</param>
        /// <returns>任务</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            var routeValue = context.Request.RouteValues;
            var routes = routeValue.GetControllerAction();
            if (routes.IsNullOrLength0())
            {
                await next(context);
                return;
            }

            // 使用上下文里的文化
            string culture = context.GetCurrentCulture();
            if (string.IsNullOrWhiteSpace(culture))
            {
                // 使用子类的文化
                culture = context.GetCurrentCulture();
            }            
            if (string.IsNullOrWhiteSpace(culture))
            {
                // 使用默认的文化
                culture = App.DefaultCulture;
            }
            if (string.IsNullOrWhiteSpace(culture))
            {
                await next(context);

                return;
            }

            LocalizationUtil.SetCurrentCulture(culture);

            await next(context);
        }

        /// <summary>
        /// 获取默认的语言文化
        /// </summary>
        /// <returns>默认的语言文化</returns>
        protected virtual string GetDefaultCulture() => null;
    }
}
