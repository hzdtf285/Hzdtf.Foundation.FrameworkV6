﻿using Hzdtf.Utility.Attr;
using Hzdtf.Utility.Data;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hzdtf.Utility.RemoteService.Provider
{
    /// <summary>
    /// 服务提供者聚合读取基类
    /// @ 黄振东
    /// </summary>
    public abstract class ServiceProviderAggReaderBase : IServicesProvider
    {
        /// <summary>
        /// 原生服务读取字典
        /// </summary>
        private readonly IDictionary<string, IReader<string[]>> dicProtoServicesReader = new ConcurrentDictionary<string, IReader<string[]>>();

        /// <summary>
        /// 获取到地址数组后事件
        /// </summary>
        public event Action<string, string, string[]> GetAddressesed;

        /// <summary>
        /// 异步根据服务名获取地址数组
        /// </summary>
        /// <param name="serviceName">服务名</param>
        /// <param name="tag">标签</param>
        /// <returns>地址数组任务</returns>
        public Task<string[]> GetAddresses(string serviceName, string tag = null)
        {
            var key = GetCacheKey(serviceName, tag);

            IReader<string[]> reader = null;
            if (dicProtoServicesReader.ContainsKey(key))
            {
                reader = dicProtoServicesReader[key];
            }
            else
            {
                reader = CreateReader(serviceName, tag);
                try
                {
                    dicProtoServicesReader.Add(key, reader);
                }
                catch (ArgumentException) { }
            }

            var adds = reader.Reader();
            if (GetAddressesed != null)
            {
                GetAddressesed(serviceName, tag, adds);
            }

            return Task<string[]>.FromResult(adds);
        }

        /// <summary>
        /// 获取缓存键
        /// </summary>
        /// <param name="serviceName">服务名</param>
        /// <param name="tag">标签</param>
        /// <returns>缓存键</returns>
        protected virtual string GetCacheKey(string serviceName, string tag = null)
            => $"{serviceName}_{tag}".ToLower();

        /// <summary>
        /// 创建读取
        /// </summary>
        /// <param name="serviceName">服务名</param>
        /// <param name="tag">标签</param>
        /// <returns>读取</returns>
        protected abstract IReader<string[]> CreateReader(string serviceName, string tag = null);

        /// <summary>
        /// 释放资源
        /// </summary>
        [ProcTrackLog(ExecProc = false)]
        public virtual void Dispose()
        {
            dicProtoServicesReader.Clear();
        }
    }
}
