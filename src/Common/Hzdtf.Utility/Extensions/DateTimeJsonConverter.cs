using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Newtonsoft.Json
{
    /// <summary>
    /// 日期时间JSON转换
    /// @ 黄振东
    /// </summary>
    public class DateTimeJsonConverter : JsonConverter
    {
        /// <summary>
        /// 是否能转换
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <returns>是否能转换</returns>
        public override bool CanConvert(Type objectType)
        {
            return typeof(DateTime).IsAssignableFrom(objectType) || typeof(DateTime?).IsAssignableFrom(objectType);
        }

        /// <summary>
        /// 读取JSON
        /// </summary>
        /// <param name="reader">JSON读取</param>
        /// <param name="objectType">对象类型</param>
        /// <param name="existingValue">存在值</param>
        /// <param name="serializer">JSON序列化</param>
        /// <returns>读取到的值</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null)
            {
                return null;
            }

            if (reader.ValueType == typeof(string))
            {
                var str = reader.Value.ToString();
                if (string.IsNullOrWhiteSpace(str))
                {
                    if (objectType == typeof(DateTime))
                    {
                        return DateTime.MinValue;
                    }
                    else
                    {
                        return null;
                    }
                }

                return str.ToCstDateTime();
            }

            var dateTime = (DateTime)reader.Value;
            if (dateTime.Kind == DateTimeKind.Utc)
            {
                return Instant.FromDateTimeUtc(dateTime).GetCstDateTime();
            }
            
            return dateTime;
        }

        /// <summary>
        /// 写入JSON
        /// </summary>
        /// <param name="writer">JSON写入</param>
        /// <param name="value">值</param>
        /// <param name="serializer">JSON序列化</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                return;
            }

            writer.WriteValue(((DateTime)value).ToFullFixedDateTime());
        }
    }
}
