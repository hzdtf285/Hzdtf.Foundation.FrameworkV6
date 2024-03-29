﻿using Hzdtf.Utility.Attr;
using Hzdtf.Utility.Conversion;
using Hzdtf.Utility.ObjectInnerConvert.Attr;
using Hzdtf.Utility.ObjectInnerConvert;
using MessagePack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Hzdtf.Utility.Model
{
    /// <summary>
    /// 基本用户信息
    /// @ 黄振东
    /// </summary>
    /// <typeparam name="IdT">ID类型</typeparam>
    [MessagePackObject]
    public class BasicUserInfo<IdT> : CodeNameInfo<IdT>
    {
        /// <summary>
        /// 登录ID_名称
        /// </summary>
        public const string LoginId_Name = "LoginId";

        /// <summary>
        /// 登录ID
        /// </summary>
        [JsonProperty("loginId")]
        [DisplayName("登录ID")]
        [MaxLength(20)]
        [Required]
        [Display(Name = "登录ID", Order = 1, AutoGenerateField = true)]
        [MessagePack.Key("loginId")]
        public string LoginId
        {
            get;
            set;
        }

        /// <summary>
        /// 密码_名称
        /// </summary>
        public const string Password_Name = "password";

        /// <summary>
        /// 密码
        /// </summary>
        [JsonProperty("password")]
        [DisplayName("密码")]
        [Required]
        [MaxLength(20)]
        [Display(Name = "密码", Order = 2, AutoGenerateField = false)]
        [MessagePack.Key("password")]
        public string Password
        {
            get;
            set;
        }
        
        /// <summary>
        /// 启用_名称
        /// </summary>
        public const string Enabled_Name = "Enabled";

        /// <summary>
        /// 启用
        /// </summary>
        [JsonProperty("enabled")]
        [DisplayValueConvert(typeof(BoolValueToTextConvert), typeof(BoolTextToValueConvert))]
        [Display(Name = "启用", Order = 50, AutoGenerateField = true)]
        [MessagePack.Key("enabled")]
        public bool Enabled
        {
            get;
            set;
        } = true;

        /// <summary>
        /// 系统内置_名称
        /// </summary>
        public const string SystemInlay_Name = "SystemInlay";

        /// <summary>
        /// 系统内置
        /// </summary>
        [JsonProperty("systemInlay")]
        [DisplayValueConvert(typeof(BoolValueToTextConvert), typeof(BoolTextToValueConvert))]
        [Display(Name = "系统内置", Order = 4, AutoGenerateField = false)]
        [MessagePack.Key("systemInlay")]
        public bool SystemInlay
        {
            get;
            set;
        }

        /// <summary>
        /// 登录时间_名称
        /// </summary>
        public const string LoginDateTime_Name = "LoginDateTime";

        /// <summary>
        /// 登录时间
        /// </summary>
        [JsonProperty("loginDateTime")]
        [DisplayValueConvert(typeof(DateTimeValueToTextConvert))]
        [Display(Name = "登录时间", Order = 5, AutoGenerateField = false)]
        [MessagePack.Key("loginDateTime")]
        [TypeConvertValue(ConvertType = TypeConvertEnum.LocalTime)]
        public DateTime? LoginDateTime
        {
            get;
            set;
        }

        /// <summary>
        /// 登录IP_名称
        /// </summary>
        public const string LoginClientIP_Name = "LoginClientIP";

        /// <summary>
        /// 登录IP
        /// </summary>
        [JsonProperty("loginClientIP")]
        [MessagePack.Key("loginClientIP")]
        [MaxLength(15)]

        [DisplayName("登录IP")]
        [Display(Name = "登录IP", Order = 6, AutoGenerateField = true)]
        public string LoginClientIP
        {
            get;
            set;
        }

        /// <summary>
        /// 登录次数_名称
        /// </summary>
        public const string Logins_Name = "Logins";

        /// <summary>
        /// 登录次数
        /// </summary>
        [JsonProperty("logins")]
        [MessagePack.Key("logins")]
        [DisplayName("登录次数")]
        [Display(Name = "登录次数", Order = 7, AutoGenerateField = true)]
        public int Logins
        {
            get;
            set;
        }

        /// <summary>
        /// 退出登录时间_名称
        /// </summary>
        public const string LogoutTime_Name = "LogoutTime";

        /// <summary>
        /// 退出登录时间
        /// </summary>
        [JsonProperty("logoutTime")]
        [DisplayValueConvert(typeof(DateTimeValueToTextConvert))]
        [Display(Name = "退出登录时间", Order = 8, AutoGenerateField = false)]
        [MessagePack.Key("logoutTime")]
        [TypeConvertValue(ConvertType = TypeConvertEnum.LocalTime)]
        public DateTime? LogoutTime
        {
            get;
            set;
        }

        /// <summary>
        /// 租赁ID_名称
        /// 一般情况没有，特殊情况下有用，如SAAS中的租赁
        /// </summary>
        public const string TenantId_Name = "tenantId";

        /// <summary>
        /// 租赁ID
        /// 一般情况没有，特殊情况下有用，如SAAS中的租赁
        /// </summary>
        [JsonProperty("tenantId")]
        [Display(Name = "租赁ID", Order = 999, AutoGenerateField = false)]
        [MessagePack.Key("tenantId")]
        public IdT TenantId
        {
            get;
            set;
        }

        /// <summary>
        /// 租赁ID字符串_名称
        /// </summary>
        public const string TenantIdString_Name = "TenantIDString";

        /// <summary>
        /// 租赁ID字符串，如果ID类型为长整型，则在JS前端使用此属性为字符串类型，因为JS中长整型会丢失精度
        /// </summary>
        [JsonProperty("tenantIdString")]
        [Display(AutoGenerateField = false)]
        [MessagePack.Key("tenantIdString")]
        public string TenantIDString
        {
            get => TenantId.ToString();
        }
    }

    /// <summary>
    /// 基本用户信息
    /// @ 黄振东
    /// </summary>
    [MessagePackObject]
    public class BasicUserInfo : BasicUserInfo<int>
    {
    }
}
