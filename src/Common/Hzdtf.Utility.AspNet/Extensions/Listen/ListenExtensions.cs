using Hzdtf.Utility.Listen;
using Hzdtf.Utility.Safety;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Hzdtf.Utility.Listen.ListenConfig;

namespace Microsoft.AspNetCore.Server.Kestrel.Core
{
    /// <summary>
    /// 监听扩展类
    /// @ 黄振东
    /// </summary>
    public static class ListenExtensions
    {
        /// <summary>
        /// 配置监听
        /// </summary>
        /// <param name="options">配置</param>
        /// <param name="configure">回调监听配置</param>
        /// <returns>配置</returns>
        public static KestrelServerOptions ConfigListen(this KestrelServerOptions options, Action<Listen, ListenOptions> configure = null)
        {
            var config = ListenConfigHelper.Reader();
            foreach (var c in config.Listens)
            {
                var pro = Enum.Parse<HttpProtocols>(c.Protocols);
                options.ListenAnyIP(c.Port, lisOptions =>
                {
                    lisOptions.Protocols = pro;

                    if (c.Https != null && !string.IsNullOrWhiteSpace(c.Https.FileName))
                    {
                        if (string.IsNullOrWhiteSpace(c.Https.Password))
                        {
                            lisOptions.UseHttps(c.Https.FileName);
                        }
                        else
                        {
                            var pwd = c.Https.PasswordEncrypt ? DESUtil.Decrypt(c.Https.Password) : c.Https.Password;
                            lisOptions.UseHttps(c.Https.FileName, pwd);
                        }
                    }

                    if (configure != null)
                    {
                        configure(c, lisOptions);
                    }
                });

            }

            return options;
        }
    }
}
