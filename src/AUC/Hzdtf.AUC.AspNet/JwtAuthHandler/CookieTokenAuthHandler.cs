using Hzdtf.Logger.Contract;
using Hzdtf.Utility.Utils;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hzdtf.AUC.AspNet.JwtAuthHandler
{
    /// <summary>
    /// Cookie票据授权处理
    /// @ 黄振东
    /// </summary>
    public class CookieTokenAuthHandler : JwtTokenAuthHandlerBase
    {
        /// <summary>
        /// 获取票据
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns>票据</returns>
        public override string GetToken(object context)
        {
            if (context == null)
            {
                LogTool.DefaultLog.WranAsync("context為null", null, "CookieTokenAuthHandler", null, "GetToken");
                return null;
            }
            if (context is HttpContext)
            {
                var httpContext = context as HttpContext;
                if (httpContext.Request == null)
                {
                    LogTool.DefaultLog.WranAsync("context.Request為null", null, "CookieTokenAuthHandler", null, "GetToken");
                    return null;
                }
                if (httpContext.Request.Cookies == null)
                {
                    LogTool.DefaultLog.WranAsync("context.Request.Cookies為null", null, "CookieTokenAuthHandler", null, "GetToken");
                    return null;
                }

                string token = null;
                try
                {
                    if (httpContext.Request.Cookies.TryGetValue(AuthUtil.COOKIE_AUTH_KEY, out token))
                    {
                        return token;
                    }
                }
                catch (Exception ex)
                {
                    LogTool.DefaultLog.WranAsync("获取Cookie发生异常，忽略", ex, "CookieTokenAuthHandler", null, "GetToken");
                    return null;
                }
            }
            else
            {
                LogTool.DefaultLog.WranAsync("context不是HttpContext类型", null, "CookieTokenAuthHandler", null, "GetToken");
                return null;
            }

            return null;
        }
    }
}
