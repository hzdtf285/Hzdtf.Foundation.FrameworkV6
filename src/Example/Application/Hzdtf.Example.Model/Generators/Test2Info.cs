using Hzdtf.Utility.Model;
using Newtonsoft.Json;
using MessagePack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Hzdtf.Example.Model
{
    /// <summary>
    /// Test2Info信息
    /// </summary>
    public partial class Test2Info : PersonTimeInfo<int>
    {
﻿        /// <summary>
        /// OrderNo_名称
        /// </summary>
		public const string OrderNo_Name = "OrderNo";

		/// <summary>
        /// OrderNo
        /// </summary>
        [JsonProperty("orderNo")]
        [MessagePack.Key("orderNo")]
        [Required]

        [DisplayName("OrderNo")]
        [Display(Name = "OrderNo", Order = 2, AutoGenerateField = true)]
        public long OrderNo
        {
            get;
            set;
        }
    }
}
