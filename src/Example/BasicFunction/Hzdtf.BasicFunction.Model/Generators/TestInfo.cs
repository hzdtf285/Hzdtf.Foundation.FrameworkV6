using Hzdtf.Utility.Model;
using Newtonsoft.Json;
using MessagePack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Hzdtf.BasicFunction.Model
{
    /// <summary>
    /// TestInfo信息
    /// </summary>
    public partial class TestInfo : PersonTimeMerchantInfo<int>
    {
﻿        /// <summary>
        /// Name_名称
        /// </summary>
		public const string Name_Name = "Name";

		/// <summary>
        /// Name
        /// </summary>
        [JsonProperty("name")]
        [MessagePack.Key("name")]
        [MaxLength(20)]

        [DisplayName("Name")]
        [Display(Name = "Name", Order = 3, AutoGenerateField = true)]
        public string Name
        {
            get;
            set;
        }
    }
}
