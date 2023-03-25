using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hzdtf.Utility.RemoteService.Builder
{
    /// <summary>
    /// 生成信息
    /// @ 黄振东
    /// </summary>
    [MessagePackObject]
    public class BuilderInfo
    {
        /// <summary>
        /// 服务名
        /// </summary>
        [MessagePack.Key("serviceName")]
        public string ServiceName
        {
            get;
            set;
        }

        /// <summary>
        /// 路径
        /// </summary>
        [MessagePack.Key("path")]
        public string Path
        {
            get;
            set;
        }

        /// <summary>
        /// 标签
        /// </summary>
        [MessagePack.Key("tag")]
        public string Tag
        {
            get;
            set;
        }
    }
}
