﻿using Hzdtf.Utility.Attr;
using Hzdtf.Utility.Utils;
using MessagePack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Hzdtf.Utility.Model
{
    /// <summary>
    /// 简单信息
    /// @ 黄振东
    /// </summary>
    /// <typeparam name="IdT">ID类型</typeparam>
    [MessagePackObject]
    public class SimpleInfo<IdT> : ICloneable
    {
        /// <summary>
        /// ID_名称
        /// </summary>
        public const string Id_Name = "Id";

        /// <summary>
        /// ID
        /// </summary>
        private IdT id;

        /// <summary>
        /// ID
        /// </summary>
        [JsonProperty("id")]
        [Display(AutoGenerateField = false)]
        [MessagePack.Key("id")]
        public IdT Id
        {
            get => IdGetValue();
            set => IdSetValue(value);
        }

        /// <summary>
        /// ID字符串_名称
        /// </summary>
        public const string IdString_Name = "IdString";

        /// <summary>
        /// ID字符串，如果ID类型为长整型，则在JS前端使用此属性为字符串类型，因为JS中长整型会丢失精度
        /// </summary>
        [JsonProperty("idString")]
        [Display(AutoGenerateField = false)]
        [MessagePack.Key("idString")]
        public string IdString
        {
            get => Id.ToString();
        }

        /// <summary>
        /// 浅拷贝
        /// </summary>
        /// <returns>拷贝后的对象</returns>
        public object Clone()
        {
            return this.MemberwiseClone();
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns>字符串</returns>
        public override string ToString() => this.ToJsonString();

        /// <summary>
        /// ID设置值
        /// </summary>
        /// <param name="id">ID</param>
        protected virtual void IdSetValue(IdT id) => this.id = id;

        /// <summary>
        /// ID获取值
        /// </summary>
        /// <returns>ID</returns>
        protected virtual IdT IdGetValue() => this.id;
    }

    /// <summary>
    /// 简单信息
    /// @ 黄振东
    /// </summary>
    [MessagePackObject]
    public class SimpleInfo : SimpleInfo<int>
    {
    }

    /// <summary>
    /// 简单租赁信息
    /// </summary>
    /// <typeparam name="IdT">ID类型</typeparam>
    public class SimpleTenantInfo<IdT> : SimpleInfo<IdT>
    {
        /// <summary>
        /// 租赁ID_名称
        /// </summary>
        public const string TenantID_Name = "TenantId";

        /// <summary>
        /// 租赁ID
        /// </summary>
        [JsonProperty("tenantId")]
        [Display(Name = "租赁ID", Order = 10, AutoGenerateField = false)]
        [MessagePack.Key("tenantId")]
        public IdT TenantId
        {
            get;
            set;
        }
    }
}
