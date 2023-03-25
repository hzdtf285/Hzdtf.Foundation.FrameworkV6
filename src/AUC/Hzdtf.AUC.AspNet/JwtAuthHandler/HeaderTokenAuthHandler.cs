using Hzdtf.Utility.Utils;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;

namespace Hzdtf.AUC.AspNet.JwtAuthHandler
{
    /// <summary>
    /// Header票据授权处理
    /// @ 黄振东
    /// </summary>
    public class HeaderTokenAuthHandler : JwtTokenAuthHandlerBase
    {
        /// <summary>
        /// 获取票据
        /// </summary>
        /// <returns>票据</returns>
        public override string GetToken(object context)
        {
            if (context is HttpContext)
            {
                var httpContext = context as HttpContext;
                return AuthUtil.GetBearerOriginalToken(httpContext.Request.Headers[AuthUtil.AUTH_KEY]);
            }
            else if (context is Metadata)
            {
                var metadata = context as Metadata;
                return AuthUtil.GetBearerOriginalToken(metadata.GetValue(AuthUtil.AUTH_KEY));
            }
            else
            {
                return null;
            }
        }
    }
}
