using FoxUC.Utility.Cache;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FoxUC.Persistence.Autofac
{
    /// <summary>
    /// 事务执行前方法缓存
    /// @ 网狐
    /// </summary>
    public class TransactionBeforeMethodCache : SingleTypeLocalMemoryBase<string, MethodInfo>
    {
        #region 属性与字段

        /// <summary>
        /// 字典缓存
        /// </summary>
        private static readonly IDictionary<string, MethodInfo> dicCache = new ConcurrentDictionary<string, MethodInfo>();

        #endregion

        #region 需要子类重写的方法

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <returns>缓存</returns>
        protected override IDictionary<string, MethodInfo> GetCache() => dicCache;

        #endregion
    }
}