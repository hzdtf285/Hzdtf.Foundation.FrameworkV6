using Hzdtf.Utility;
using Hzdtf.Utility.GRpc;
using Grpc.Core;
using Grpc.Net.Client.Balancer;
using Grpc.Net.ClientFactory;
using System;
using Grpc.Net.Client;
using Hzdtf.Utility.GRpc.Pool.Service;
using Hzdtf.Utility.GRpc.Pool;
using Hzdtf.Utility.RemoteService.Builder;
using Hzdtf.Utility.AspNet.Extensions.GRpc;
using Hzdtf.Utility.Data;
using Hzdtf.Utility.Pool;
using Hzdtf.Utility.RemoteService.Provider;
using Hzdtf.Utility.RemoteService.Options;

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
        /// <param name="clientName">客户端名称。如果存在多个grpc客户端类名相同，则需要指定不能的客户端名称区分</param>
        /// <param name="callbackChannelOptions">回调GRpc渠道选项配置</param>
        /// <param name="credentials">证书</param>
        /// <returns>服务收藏</returns>
        public static IServiceCollection AddGrpcClientAndDefaultBalancing<GRpcClientT>(this IServiceCollection services, string serviceName, Action<GrpcClientFactoryOptions> options = null, string clientName = null, Action<GrpcChannelOptions> callbackChannelOptions = null,
            ChannelCredentials credentials = null)
           where GRpcClientT : ClientBase<GRpcClientT>
        {
            if (string.IsNullOrWhiteSpace(serviceName))
            {
                throw new ArgumentNullException("服务名不能为空");
            }

            var fun = (GrpcClientFactoryOptions o) =>
            {
                o.SetRobinBalancingAndStaticResolver(serviceName, callbackChannelOptions, credentials);
                if (options != null)
                {
                    options(o);
                }
            };
            if (string.IsNullOrWhiteSpace(clientName))
            {
                services.AddGrpcClient<GRpcClientT>(fun);
            }
            else
            {
                services.AddGrpcClient<GRpcClientT>(clientName, fun);
            }

            return services;
        }

        /// <summary>
        /// 添加GRpc客户端池
        /// </summary>
        /// <param name="services">服务收藏</param>
        /// <param name="options">grpc客户端池配置</param>
        /// <returns>服务收藏</returns>
        public static IServiceCollection AddGRpcClientTool(this IServiceCollection services, Action<GRpcClientPoolOptions> options = null)
        {
            var config = new GRpcClientPoolOptions();
            if (options != null)
            {
                options(config);
            }
            if (config.PoolConfig != null)
            {
                var handle = new PoolConfigHandle<string, GrpcChannelOptions>();
                handle.Set(config.PoolConfig);

                services.AddSingleton<IGetObject<PoolConfigInfo<string, GrpcChannelOptions>>>(handle);
                services.AddSingleton<ISetObject<PoolConfigInfo<string, GrpcChannelOptions>>>(handle);
            }
            if (config.ServiceBuilderType == null)
            {
                services.AddSingleton<IUnityServicesBuilder, UnityServicesBuilder>();
            }
            else
            {
                services.AddSingleton(typeof(IUnityServicesBuilder), config.ServiceBuilderType);
            }
            if (config.PoolType == null)
            {
                services.AddSingleton<IGRpcChannelPool, GRpcChannelPool>();
            }
            else
            {
                services.AddSingleton(typeof(IGRpcChannelPool), config.PoolType);
            }
            if (config.ServicePoolType == null)
            {
                services.AddSingleton<IGRpcUnityServicePool, GRpcUnityServicePool>();
            }
            else
            {
                services.AddSingleton(typeof(IGRpcUnityServicePool), config.ServicePoolType);
            }
            if (config.ProviderType == null)
            {
                services.AddSingleton<IServicesProvider, HostConfigServiceProvider>();
            }
            else
            {
                services.AddSingleton(typeof(IServicesProvider), config.ProviderType);
            }
            if (config.ServicesOptionsType == null)
            {
                services.AddSingleton<IUnityServicesOptions, UnityServicesOptionsCache>();
            }
            else
            {
                services.AddSingleton(typeof(IUnityServicesOptions), config.ServicesOptionsType);
            }

            return services;
        }
    }
}
