using Hzdtf.Utility.Model;
using Newtonsoft.Json;
using MessagePack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Hzdtf.Workflow.Model
{
    /// <summary>
    /// 标准关卡信息
    /// @ 黄振东
    /// </summary>
    public partial class StandardCensorshipInfo : PersonTimeInfo<int>
    {
﻿        /// <summary>
        /// 编码_名称
        /// </summary>
		public const string Code_Name = "Code";

		/// <summary>
        /// 编码
        /// </summary>
        [JsonProperty("code")]
        [MessagePack.Key("code")]
        [Required]
        [MaxLength(20)]

        [DisplayName("编码")]
        [Display(Name = "编码", Order = 2, AutoGenerateField = true)]
        public string Code
        {
            get;
            set;
        }

﻿        /// <summary>
        /// 名称_名称
        /// </summary>
		public const string Name_Name = "Name";

		/// <summary>
        /// 名称
        /// </summary>
        [JsonProperty("name")]
        [MessagePack.Key("name")]
        [Required]
        [MaxLength(20)]

        [DisplayName("名称")]
        [Display(Name = "名称", Order = 3, AutoGenerateField = true)]
        public string Name
        {
            get;
            set;
        }


    }
}
