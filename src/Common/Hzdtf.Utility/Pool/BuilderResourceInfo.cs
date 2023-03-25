using Hzdtf.Utility.RemoteService.Builder;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hzdtf.Utility.Pool
{
    /// <summary>
    /// 生成资源信息
    /// @ 黄振东
    /// </summary>
    /// <typeparam name="ResourceValueT">资源值类型</typeparam>
    [MessagePackObject]
    public class BuilderResourceInfo<ResourceValueT> : BuilderInfo
        where ResourceValueT : class
    {
        /// <summary>
        /// 值
        /// </summary>
        [MessagePack.Key("value")]
        public ResourceValueT Value
        {
            get;
            set;
        }
    }
}
