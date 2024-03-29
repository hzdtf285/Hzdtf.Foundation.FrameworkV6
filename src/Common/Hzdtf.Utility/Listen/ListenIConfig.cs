﻿using Hzdtf.Utility.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hzdtf.Utility.Listen
{
    /// <summary>
    /// 监听配置
    /// @ 黄振东
    /// </summary>
    public class ListenConfig
    {
        /// <summary>
        /// 监听数组
        /// </summary>
        public Listen[] Listens { get; set; }

        /// <summary>
        /// 监听
        /// @ 黄振东
        /// </summary>
        public class Listen
        {
            /// <summary>
            /// 主机
            /// </summary>
            public string Host { get; set; }

            /// <summary>
            /// 端口
            /// </summary>
            public int Port { get; set; }

            /// <summary>
            /// 协议
            /// </summary>
            public string Protocols { get; set; }

            /// <summary>
            /// https
            /// </summary>
            public Https Https { get; set; }

            /// <summary>
            /// 获取方案
            /// </summary>
            /// <returns>方案</returns>
            public string GetSheme()
            {
                return Https == null ? Uri.UriSchemeHttp : Uri.UriSchemeHttps;
            }

            /// <summary>
            /// 转换为URL
            /// </summary>
            /// <returns>URL</returns>
            public string ToUrl()
            {
                var host = Host ?? NetworkUtil.LocalIP;
                return GetSheme() + "://" + host + ":" + Port;
            }
        }

        /// <summary>
        /// https
        /// @ 黄振东
        /// </summary>
        public class Https
        {
            /// <summary>
            /// 文件名
            /// </summary>
            public string FileName { get; set; }

            /// <summary>
            /// 密码
            /// </summary>
            public string Password { get; set; }

            /// <summary>
            /// 密码是否加密，默认为否
            /// </summary>
            public bool PasswordEncrypt { get; set; }
        }
    }
}
