using Grpc.Net.Client;
using Hzdtf.Utility;
using Hzdtf.Utility.Enums;
using Hzdtf.Utility.Factory;
using Hzdtf.Utility.Model;
using Hzdtf.Utility.Utils;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Http
{
    /// <summary>
    /// http上下文扩展类
    /// @ 黄振东
    /// </summary>
    public static class HttpContextExtensions
    {
        /// <summary>
        /// 获取上下文键
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns>上下文键</returns>
        public static string GetContextKey(this HttpContext context)
        {
            if (context.Connection == null)
            {
                return context.GetHashCode().ToString();
            }

            return $"{context.Connection.Id}_{context.GetHashCode()}";
        }

        /// <summary>
        /// 创建通用数据
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="factory">通用数据工厂</param>
        /// <param name="key">键</param>
        /// <param name="menuCode">菜单编码</param>
        /// <param name="functionCodes">功能编码数组</param>
        /// <returns>通用数据</returns>
        public static CommonUseData CreateCommonUseData(this HttpContext context, ISimpleFactory<HttpContext, CommonUseData> factory, string key = null, string menuCode = null, params string[] functionCodes)
        {
            if (factory != null)
            {
                var result = factory.Create(context);
                if (result != null)
                {
                    result.Key = key;
                    result.MenuCode = menuCode;
                    result.FunctionCodes = functionCodes;

                    return result;
                }
            }

            var comData = context.CreateBasicCommonUseData();
            if (string.IsNullOrWhiteSpace(comData.EventId))
            {
                comData.EventId = StringUtil.NewShortGuid();
            }

            return comData;
        }

        /// <summary>
        /// 创建基本的通用数据
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="key">键</param>
        /// <param name="menuCode">菜单编码</param>
        /// <param name="authToken">授权票据</param>
        /// <param name="functionCodes">功能编码数组</param>
        /// <returns>基本的通用数据</returns>
        public static CommonUseData CreateBasicCommonUseData(this HttpContext context, string key = null, string menuCode = null, IAuthToken authToken = null, params string[] functionCodes)
        {
            var result = new CommonUseData()
            {
                Key = key,
                MenuCode = menuCode,
                FunctionCodes = functionCodes
            };
            if (context != null && context.Request != null)
            {
                result.Path = context.Request.Path.Value.ToLower();
                var routeValue = context.Request.RouteValues;
                var routes = routeValue.GetControllerAction();
                if (routes != null && routes.Length > 1)
                {
                    result.Controller = routes[0];
                    result.Action = routes[1];
                }

                result.Token = authToken == null ? context.Request.GetBearerOriginTokenFromHeader() : authToken.GetToken();

                if (context.Request.Headers != null && context.Request.Headers.ContainsKey(App.EVENT_ID_KEY))
                {
                    result.EventId = context.Request.Headers[App.EVENT_ID_KEY];
                }

                result.CommMode = GRpcChannelUtil.IsRequestGRpc(context.Request.ContentType) ? CommunicationMode.GRPC : CommunicationMode.HTTP;
            }

            return result;
        }
    }
}
