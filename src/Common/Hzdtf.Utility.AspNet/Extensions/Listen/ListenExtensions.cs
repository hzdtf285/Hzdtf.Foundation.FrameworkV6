using Hzdtf.Utility.Listen;
using Hzdtf.Utility.Safety;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
            return ConfigListen(options, out _, configure);
        }

        /// <summary>
        /// 配置监听
        /// </summary>
        /// <param name="options">配置</param>
        /// <param name="config">监听配置</param>
        /// <param name="configure">回调监听配置</param>
        /// <returns>配置</returns>
        public static KestrelServerOptions ConfigListen(this KestrelServerOptions options, out ListenConfig config, Action<Listen, ListenOptions> configure = null)
        {
            config = ListenConfigHelper.Reader();
            foreach (var c in config.Listens)
            {
                if (string.IsNullOrWhiteSpace(c.Host))
                {
                    options.ListenAnyIP(c.Port, lisOptions =>
                    {
                        ListenConfig(c, lisOptions, configure);
                    });
                }
                else
                {
                    options.Listen(IPAddress.Parse(c.Host), c.Port, lisOptions =>
                    {
                        ListenConfig(c, lisOptions, configure);
                    });
                }
            }

            return options;
        }

        /// <summary>
        /// 监听配置
        /// </summary>
        /// <param name="listen">监听</param>
        /// <param name="lisOptions">监听配置</param>
        /// <param name="configure">回调监听配置</param>
        private static void ListenConfig(Listen listen, ListenOptions lisOptions, Action<Listen, ListenOptions> configure = null)
        {
            var pro = Enum.Parse<HttpProtocols>(listen.Protocols);
            lisOptions.Protocols = pro;

            if (listen.Https != null && !string.IsNullOrWhiteSpace(listen.Https.FileName))
            {
                if (string.IsNullOrWhiteSpace(listen.Https.Password))
                {
                    lisOptions.UseHttps(listen.Https.FileName);
                }
                else
                {
                    var pwd = listen.Https.PasswordEncrypt ? DESUtil.Decrypt(listen.Https.Password) : listen.Https.Password;
                    lisOptions.UseHttps(listen.Https.FileName, pwd);
                }
            }

            if (configure != null)
            {
                configure(listen, lisOptions);
            }
        }
    }
}
