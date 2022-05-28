using Hzdtf.Utility.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Hzdtf.Persistence.Autofac
{
    /// <summary>
    /// 事务执行前方法缓存
    /// @ 黄振东
    /// </summary>
    public class TransactionBeforeMethodCache : SingleTypeLocalMemoryBase<string, MethodInfo>
    {
        #region 属性与字段

        /// <summary>
        /// 字典缓存
        /// </summary>
        private static readonly IDictionary<string, MethodInfo> dicCache = new Dictionary<string, MethodInfo>(1);

        /// <summary>
        /// 同步字典缓存
        /// </summary>
        private static readonly object syncDicCache = new object();

        #endregion

        #region 需要子类重写的方法

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <returns>缓存</returns>
        protected override IDictionary<string, MethodInfo> GetCache() => dicCache;

        /// <summary>
        /// 获取同步的缓存对象，是为了线程安全
        /// </summary>
        /// <returns>同步的缓存对象</returns>
        protected override object GetSyncCache() => syncDicCache;

        #endregion
    }
}
