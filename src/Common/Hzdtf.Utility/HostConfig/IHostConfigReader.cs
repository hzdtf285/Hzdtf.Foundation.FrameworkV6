using Hzdtf.Utility.Data;
using Hzdtf.Utility.Model;
using System.Collections.Generic;

namespace Hzdtf.Utility.HostConfig
{
    /// <summary>
    /// 主机配置读取接口
    /// @ 黄振东
    /// </summary>
    public interface IHostConfigReader : IReader<IDictionary<string, KeyValueInfo<string, int>[]>>
    {
    }
}
