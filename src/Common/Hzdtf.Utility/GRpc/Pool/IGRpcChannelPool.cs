using Hzdtf.Utility.Pool;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hzdtf.Utility.GRpc.Pool
{
    /// <summary>
    /// GRpc渠道池接口
    /// </summary>
    public interface IGRpcChannelPool : IResourcePool<string, GrpcChannel, GrpcChannelOptions>
    {
    }
}
