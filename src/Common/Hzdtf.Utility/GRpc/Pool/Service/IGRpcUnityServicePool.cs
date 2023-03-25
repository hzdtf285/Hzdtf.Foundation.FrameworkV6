using Hzdtf.Utility.Pool.Service;
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
    public interface IGRpcUnityServicePool : IUnityServicePool<string, GrpcChannel>
    {
    }
}
