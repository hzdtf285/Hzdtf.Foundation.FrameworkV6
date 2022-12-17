using Hzdtf.Utility.Attr;
using Hzdtf.Utility.Cache;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hzdtf.Utility.RequestResource
{
    /// <summary>
    /// 请求资源缓存
    /// @ 黄振东
    /// </summary>
    [Inject]
    public class RequestResourceCache : SingleTypeLocalMemoryBase<string, string>, IRequestResource
    {
        #region 属性与字段

        /// <summary>
        /// 字典缓存
        /// </summary>
        private static readonly IDictionary<string, string> dicCache = new ConcurrentDictionary<string, string>();

        #endregion

        #region 需要子类重写的方法

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <returns>缓存</returns>
        protected override IDictionary<string, string> GetCache() => dicCache;

        #endregion
    }
}
