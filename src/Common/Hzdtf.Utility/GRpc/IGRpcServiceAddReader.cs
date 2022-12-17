using Hzdtf.Utility.Data;
using Hzdtf.Utility.Model;
using Grpc.Net.Client.Balancer;
using System.Net;

namespace Hzdtf.Utility.GRpc
{
    /// <summary>
    /// GRpc服务地址读取接口
    /// @ 黄振东
    /// </summary>
    public interface IGRpcServiceAddReader : IReaderDic<string, BalancerAddress[]>
    {
    }
}
