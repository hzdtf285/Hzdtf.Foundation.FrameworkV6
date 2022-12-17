using Hzdtf.Utility.Cache;
using Hzdtf.Utility.Data;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Hzdtf.Utility.Config.AssemblyConfig
{
    /// <summary>
    /// 程序集配置本地内存
    /// @ 黄振东
    /// </summary>
    public class AssemblyConfigLocalMember : SingleTypeLocalMemoryBase<bool, AssemblyConfigInfo>, IReader<AssemblyConfigInfo>
    {
        #region 属性与字段

        /// <summary>
        /// 字典缓存
        /// </summary>
        private static readonly IDictionary<bool, AssemblyConfigInfo> dicCache = new ConcurrentDictionary<bool, AssemblyConfigInfo>();

        /// <summary>
        /// 原生程序集配置读取
        /// </summary>
        public IReader<AssemblyConfigInfo> ProtoAssemblyConfigReader
        {
            get;
            set;
        }

        #endregion

        #region IReader<AssemblyConfigInfo> 接口

        /// <summary>
        /// 读取
        /// </summary>
        /// <returns>数据</returns>
        public new AssemblyConfigInfo Reader()
        {
            if (dicCache.ContainsKey(true))
            {
                return dicCache[true];
            }

            AssemblyConfigInfo assemblyConfigInfo = ProtoAssemblyConfigReader.Reader();
            if (assemblyConfigInfo == null)
            {
                return null;
            }

            Add(true, assemblyConfigInfo);

            return assemblyConfigInfo;
        }

        #endregion

        #region 需要子类重写的方法

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <returns>缓存</returns>
        protected override IDictionary<bool, AssemblyConfigInfo> GetCache() => dicCache;

        #endregion
    }
}
