using Hzdtf.Utility.Pool;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hzdtf.Utility.AspNet.Extensions.GRpc
{
    /// <summary>
    /// GRpc客户端池配置
    /// @ 黄振东
    /// </summary>
    public class GRpcClientPoolOptions
    {
        /// <summary>
        /// 池配置
        /// </summary>
        public PoolConfigInfo<string, GrpcChannelOptions> PoolConfig
        {
            get;
            set;
        }

        /// <summary>
        /// 服务池类型，对应接口类型：IGRpcUnityServicePool
        /// </summary>
        public Type ServicePoolType
        {
            get;
            set;
        }

        /// <summary>
        /// 池，对应接口类型：IGRpcChannelPool
        /// </summary>
        public Type PoolType
        {
            get;
            set;
        }

        /// <summary>
        /// 服务生成器，对应接口类型：IUnityServicesBuilder
        /// </summary>
        public Type ServiceBuilderType
        {
            get;
            set;
        }

        /// <summary>
        /// 服务提供者，对应接口类型：IServicesProvider
        /// </summary>
        public Type ProviderType
        {
            get;
            set;
        }

        /// <summary>
        /// 统计服务配置，对应接口类型：IUnityServicesOptions
        /// </summary>
        public Type ServicesOptionsType
        {
            get;
            set;
        }
    }
}
