using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.ResponseCompression;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 压缩扩展类
    /// @ 黄振东
    /// </summary>
    public static class CompressionExtensions
    {
        /// <summary>
        /// 添加压缩
        /// </summary>
        /// <param name="services">服务收藏</param>
        /// <param name="enableForHttps">是否启动https</param>
        /// <returns>服务收藏</returns>
        public static IServiceCollection AddCompression(this IServiceCollection services, bool enableForHttps = true)
        {
            services.Configure<BrotliCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Optimal;
            }).Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Optimal;
            }).AddResponseCompression(options =>
            {
                options.EnableForHttps = enableForHttps;
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
                {
                    "text/html; charset=utf-8",
                    "application/xhtml+xml",
                    "application/atom+xml",
                });
            });

            return services;
        }
    }
}
