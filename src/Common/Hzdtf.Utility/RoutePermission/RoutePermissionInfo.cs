﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Hzdtf.Utility.RoutePermission
{
    /// <summary>
    /// 路由权限信息
    /// @ 黄振东
    /// </summary>
    public class RoutePermissionInfo
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id
        {
            get;
            set;
        }

        /// <summary>
        /// 控制
        /// </summary>
        public string Controller { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 是否禁用
        /// </summary>
        public bool Disabled { get; set; }

        /// <summary>
        /// 动作数组
        /// </summary>
        public ActionInfo[] Actions { get; set; }

        /// <summary>
        /// 扩展属性
        /// </summary>
        public IDictionary<string, string> Extend
        {
            get;
            set;
        }

        /// <summary>
        /// 动作信息
        /// @ 黄振东
        /// </summary>
        public class ActionInfo
        {
            /// <summary>
            /// ID
            /// </summary>
            public int Id
            {
                get;
                set;
            }

            /// <summary>
            /// 动作
            /// </summary>
            public string Action { get; set; }

            /// <summary>
            /// 编码数组
            /// </summary>
            public string[] Codes { get; set; }

            /// <summary>
            /// 是否禁用
            /// </summary>
            public bool Disabled { get; set; }

            /// <summary>
            /// 资源键
            /// </summary>
            public string ResourceKey { get; set; }

            /// <summary>
            /// 扩展属性
            /// </summary>
            public IDictionary<string, string> Extend
            {
                get;
                set;
            }
        }
    }
}
