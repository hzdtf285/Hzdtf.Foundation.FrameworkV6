using Hzdtf.Utility.Enums;
using MessagePack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Hzdtf.Utility.Utils;
using Hzdtf.Utility.ObjectInnerConvert.Attr;
using Hzdtf.Utility.ObjectInnerConvert;

namespace Hzdtf.Utility.Model
{
    /// <summary>
    /// 筛选信息
    /// @ 黄振东
    /// </summary>
    [MessagePackObject]
    public class FilterInfo
    {
        /// <summary>
        /// 开始创建时间
        /// </summary>
        [JsonProperty("startCreateTime")]
        [MessagePack.Key("startCreateTime")]
        [TypeConvertValue(ConvertType = TypeConvertEnum.LocalTime)]
        public DateTime? StartCreateTime
        {
            get;
            set;
        }

        /// <summary>
        /// 结束创建时间
        /// </summary>
        [JsonProperty("endCreateTime")]
        [MessagePack.Key("endCreateTime")]
        [TypeConvertValue(ConvertType = TypeConvertEnum.LocalTime)]
        public DateTime? EndCreateTime
        {
            get;
            set;
        }

        /// <summary>
        /// 排序
        /// </summary>
        [JsonProperty("sort")]
        [MessagePack.Key("sort")]
        public SortType Sort
        {
            get;
            set;
        }

        /// <summary>
        /// 排名名称
        /// </summary>
        private string sortName;

        /// <summary>
        /// 排序名称
        /// </summary>
        [JsonProperty("sortName")]
        [MessagePack.Key("sortName")]
        public string SortName
        {
            get => sortName;
            set
            {
                if (string.IsNullOrWhiteSpace(value) || LimitSortNames == null || LimitSortNames.Contains(value, true))
                {
                    sortName = value;
                }
                else
                {
                    throw new NotSupportedException($"不支持排名名称[{value}]");
                }
            }
        }

        /// <summary>
        /// 限定排序名称数组。不区分大小写。如果有限定范围，请重写此属性
        /// </summary>
        [IgnoreMember, JsonIgnore]
        protected virtual string[] LimitSortNames
        {
            get;
        }
    }
}
