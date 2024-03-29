﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Hzdtf.Utility.Json
{
    /// <summary>
    /// 日期时间本地Json转换
    /// @ 黄振东
    /// </summary>
    public class DateTimeLocalJsonConvert : JsonConverter<DateTime>
    {
        /// <summary>
        /// 读取日期时间
        /// </summary>
        /// <param name="reader">读取</param>
        /// <param name="typeToConvert">类型转换</param>
        /// <param name="options">JSON配置选项</param>
        /// <returns>日期时间</returns>
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.GetString().ToCstDateTime();
        }

        /// <summary>
        /// 写入日期时间
        /// </summary>
        /// <param name="writer">写入</param>
        /// <param name="value">值</param>
        /// <param name="options">JSON配置选项</param>
        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToFullFixedDateTime());
        }
    }
}
