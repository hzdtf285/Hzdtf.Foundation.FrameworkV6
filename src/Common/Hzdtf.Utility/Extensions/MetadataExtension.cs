using Hzdtf.Utility.Enums;
using Hzdtf.Utility.Model;
using Hzdtf.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Hzdtf.Utility.Utils;

namespace Grpc.Core
{
    /// <summary>
    /// Medata扩展类
    /// @ 黄振东
    /// </summary>
    public static class MetadataExtension
    {
        /// <summary>
        /// 创建基本的通用数据
        /// </summary>
        /// <param name="metadata">metadata</param>
        /// <param name="key">键</param>
        /// <param name="menuCode">菜单编码</param>
        /// <param name="authToken">授权票据</param>
        /// <param name="functionCodes">功能编码数组</param>
        /// <returns>基本的通用数据</returns>
        public static CommonUseData CreateBasicCommonUseData(this Metadata metadata, string key = null, string menuCode = null, IAuthToken authToken = null, params string[] functionCodes)
        {
            CommonUseData result = null;
            if (metadata.IsNullOrCount0())
            {
                result = new CommonUseData()
                {
                    Key = key,
                    MenuCode = menuCode,
                    FunctionCodes = functionCodes
                };
            }
            else
            {
                var token = authToken == null ? metadata.GetValue(AuthUtil.AUTH_KEY) : authToken.GetToken(metadata);
                result = new CommonUseData(controller: metadata.GetValue("Controller"), action: metadata.GetValue("Action"), path: metadata.GetValue("Path"),
                    commMode: CommunicationMode.GRPC, clientRemoteIp: metadata.GetValue(App.CLIENT_REMOTE_IP_HEAD_KEY), token: token)
                {
                    Key = key,
                    MenuCode = menuCode,
                    FunctionCodes = functionCodes,
                };
                result.EventId = metadata.GetValue(App.EVENT_ID_KEY);

                if (!string.IsNullOrWhiteSpace(result.MenuCode))
                {
                    result.MenuCode = metadata.GetValue("MenuCode");
                }
                if (!result.FunctionCodes.IsNullOrLength0())
                {
                    var funCodeStr = metadata.GetValue("FunctionCodes");
                    if (!string.IsNullOrWhiteSpace(funCodeStr))
                    {
                        result.FunctionCodes = funCodeStr.ToJsonObject<string[]>();
                    }
                }
                if (!string.IsNullOrWhiteSpace(result.MenuCode))
                {
                    result.Key = metadata.GetValue("Key");
                }
            }

            return result;
        }

        /// <summary>
        /// 根据键获取值
        /// </summary>
        /// <param name="metadata">Metadata</param>
        /// <param name="key">键</param>
        /// <returns>值</returns>
        public static string GetValue(this Metadata metadata, string key)
        {
            if (string.IsNullOrWhiteSpace(key) || metadata.IsNullOrCount0())
            {
                return null;
            }

            foreach (var item in metadata)
            {
                if (key.Equals(item.Key))
                {
                    return item.Value;
                }
            }

            return null;
        }
    }
}
