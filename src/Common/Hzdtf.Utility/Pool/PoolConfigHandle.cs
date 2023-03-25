using Hzdtf.Utility.Attr;
using Hzdtf.Utility.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Hzdtf.Utility.Pool
{
    /// <summary>
    /// 池配置操作
    /// @ 黄振东
    /// </summary>
    /// <typeparam name="ResourceKeyT">资源键类型</typeparam>
    /// <typeparam name="ConcreateReourseOptionsT">具体资源配置类型</typeparam>
    [Inject]
    public class PoolConfigHandle<ResourceKeyT, ConcreateReourseOptionsT> : IGetObject<PoolConfigInfo<ResourceKeyT, ConcreateReourseOptionsT>>, ISetObject<PoolConfigInfo<ResourceKeyT, ConcreateReourseOptionsT>>
    {
        /// <summary>
        /// 配置信息
        /// </summary>
        private PoolConfigInfo<ResourceKeyT, ConcreateReourseOptionsT> config;

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <returns>对象</returns>
        public PoolConfigInfo<ResourceKeyT, ConcreateReourseOptionsT> Get() => config;

        /// <summary>
        /// 设置对象
        /// </summary>
        /// <param name="configInfo">对象</param>
        public void Set(PoolConfigInfo<ResourceKeyT, ConcreateReourseOptionsT> configInfo) => config = configInfo;
    }
}
