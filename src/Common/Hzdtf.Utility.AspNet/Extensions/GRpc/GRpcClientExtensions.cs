using Hzdtf.Utility;
using Hzdtf.Utility.GRpc;
using Grpc.Core;
using Grpc.Net.Client.Balancer;
using Grpc.Net.Client.Configuration;
using Grpc.Net.ClientFactory;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// GRpc客户端扩展类
    /// @ 黄振东
    /// </summary>
    public static class GRpcClientExtensions
    {
        /// <summary>
        /// 添加GRpc默认地址解析（静态）
        /// 地址解析：从App.Instance获取IGRpcServiceAddReader
        /// </summary>
        /// <param name="services">服务收藏</param>
        /// <returns>服务收藏</returns>
        public static IServiceCollection AddGrpcDefaultResolver(this IServiceCollection services)
        {
            var resolverFactory = new StaticResolverFactory(addr =>
            {
                var hostReader = App.Instance.GetService<IGRpcServiceAddReader>();
                return hostReader.Reader(addr.LocalPath);
            });
            services.AddSingleton<ResolverFactory>(resolverFactory);
            services.AddSingleton<IGRpcServiceAddReader, GRpcServiceAddCache>();

            return services;
        }

        /// <summary>
        /// 添加GRpc客户端并且设置默认负载均衡策略（轮询）
        /// </summary>
        /// <typeparam name="GRpcClientT">GRpc客户端类型</typeparam>
        /// <param name="serviceName">服务名</param>
        /// <param name="services">服务收藏</param>
        /// <param name="options">配置回调</param>
        /// <returns>服务收藏</returns>
        public static IServiceCollection AddGrpcClientAndDefaultBalancing<GRpcClientT>(this IServiceCollection services, string serviceName, Action<GrpcClientFactoryOptions> options = null)
           where GRpcClientT : ClientBase<GRpcClientT>
        {
            if (string.IsNullOrWhiteSpace(serviceName))
            {
                throw new ArgumentNullException("服务名不能为空");
            }
            services.AddGrpcClient<GRpcClientT>(o =>
            {
                o.Address = new Uri($"static:{serviceName}");
                o.ChannelOptionsActions.Add(op =>
                {
                    op.Credentials = ChannelCredentials.Insecure;
                    op.ServiceConfig = new ServiceConfig { LoadBalancingConfigs = { new RoundRobinConfig() } };
                });
                if (options != null)
                {
                    options(o);
                }
            });

            return services;
        }
    }
}
