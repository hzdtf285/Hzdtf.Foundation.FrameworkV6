using Hzdtf.Utility.Pool;
using Grpc.Net.Client;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hzdtf.Utility.GRpc.Pool
{
    /// <summary>
    /// 生成GRpc渠道信息
    /// @ 黄振东
    /// </summary>
    [MessagePackObject]
    public class BuilderGRpcChannelInfo : BuilderResourceInfo<GrpcChannel>
    {
    }
}
