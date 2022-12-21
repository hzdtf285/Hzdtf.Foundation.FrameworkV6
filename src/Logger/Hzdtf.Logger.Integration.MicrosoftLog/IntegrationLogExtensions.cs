using Hzdtf.Logger.Integration.MicrosoftLog;
using System;
using Hzdtf.Logger.Contract;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 集成日志扩展类
    /// @ 黄振东
    /// </summary>
    public static class IntegrationLogExtensions
    {
        /// <summary>
        /// 添加Hzdtf日志
        /// 默认原生日志为控制台日志（ConsoleLog）
        /// </summary>
        /// <param name="services">服务收藏</param>
        /// <param name="protoLogType">原生日志类型，必须是实现ILogable的接口实现类类型，默认是ConsoleLog实现类</param>
        /// <param name="options">回调选项，如果需要指定原生日志对象，则需要配置；否则使用默认的原生日志</param>
        /// <returns>服务收藏</returns>
        public static IServiceCollection AddHzdtfLog(this IServiceCollection services, Type protoLogType = null, Action<ILoggingBuilder, ILoggerProvider> options = null)
        {
            services.AddSingleton<ILoggerProvider, IntegrationLogProvider>();
            if (protoLogType == null)
            {
                protoLogType = typeof(ConsoleLog);
            }
            services.AddSingleton(typeof(ILogable), protoLogType);
            services.AddLogging(builder =>
            {
                var provider = new IntegrationLogProvider();
                builder.AddProvider(provider);

                if (options != null)
                {
                    options(builder, provider);
                }
            });


            return services;
        }
    }
}
