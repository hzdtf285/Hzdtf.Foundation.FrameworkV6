using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// JSON序列化扩展类
    /// @ 黄振东
    /// </summary>
    public static class JsonSerializerExtensions
    {
        /// <summary>
        /// 添加默认的JSON
        /// </summary>
        /// <param name="builder">MVC生成器</param>
        /// <returns>MVC生成器</returns>
        public static IMvcBuilder AddDefaultNewtonsoftJson(this IMvcBuilder builder)
        {
            builder.AddNewtonsoftJson(o =>
            {
                o.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                o.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                o.SerializerSettings.Converters.Add(new DateTimeJsonConverter());
            });

            return builder;
        }
    }
}
