using MessagePack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Hzdtf.Utility.Model
{
    /// <summary>
    /// 带有时间的信息
    /// @ 黄振东
    /// </summary>
    /// <typeparam name="IdT">ID类型</typeparam>
    [MessagePackObject]
    public class TimeInfo<IdT> : SimpleInfo<IdT>
    {
        /// <summary>
        /// 创建时间_名称
        /// </summary>
        public const string CreateDateTime_Name = "CreateDateTime";

        /// <summary>
        /// 创建时间
        /// </summary>
        [JsonProperty("createDateTime")]
        [Display(AutoGenerateField = false)]
        [MessagePack.Key("createDateTime")]
        public DateTime CreateDateTime
        {
            get;
            set;
        }

        /// <summary>
        /// 修改时间_名称
        /// </summary>
        public const string ModifyDateTime_Name = "ModifyDateTime";

        /// <summary>
        /// 修改时间
        /// </summary>
        [JsonProperty("modifyDateTime")]
        [Display(AutoGenerateField = false)]
        [MessagePack.Key("modifyDateTime")]
        public DateTime ModifyDateTime
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 带有时间的信息
    /// @ 黄振东
    /// </summary>
    [MessagePackObject]
    public class TimeInfo : TimeInfo<int>
    {
    }
}
