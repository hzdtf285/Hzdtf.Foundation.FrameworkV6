using Hzdtf.Utility.Attr;
using Hzdtf.Utility.Pool.Service;
using Hzdtf.Utility.RemoteService.Builder;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hzdtf.Utility.GRpc.Pool.Service
{
    /// <summary>
    /// GRpc统计服务池
    /// @ 黄振东
    /// </summary>
    [Inject]
    public class GRpcUnityServicePool : UnityServicePool<GrpcChannel, GrpcChannelOptions>, IGRpcUnityServicePool
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="resourcePool">资源池</param>
        /// <param name="servicesBuilder">统计服务生成器</param>
        public GRpcUnityServicePool(IGRpcChannelPool resourcePool, IUnityServicesBuilder servicesBuilder)
            : base(resourcePool, servicesBuilder)
        {
        }
    }
}
