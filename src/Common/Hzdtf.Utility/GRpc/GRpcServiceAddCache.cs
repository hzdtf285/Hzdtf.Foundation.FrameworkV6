using Hzdtf.Utility.Cache;
using System.Net;
using Hzdtf.Utility.Utils;
using Grpc.Net.Client.Balancer;
using System.Collections.Generic;
using Hzdtf.Utility.HostConfig;
using System;
using System.Collections.Concurrent;

namespace Hzdtf.Utility.GRpc
{
    /// <summary>
    /// GRpc服务地址缓存
    /// @ 黄振东
    /// </summary>
    public class GRpcServiceAddCache : SingleTypeLocalMemoryBase<string, BalancerAddress[]>, IGRpcServiceAddReader
    {
        /// <summary>
        /// 缓存键
        /// </summary>
        private static readonly IDictionary<string, BalancerAddress[]> dicCaches = new ConcurrentDictionary<string, BalancerAddress[]>();

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="hostConfigReader">主机配置读取，如果为空，默认为HostConfigJsonFile</param>
        public GRpcServiceAddCache(IHostConfigReader hostConfigReader = null) 
        {
            if (hostConfigReader == null)
            {
                hostConfigReader = new HostConfigJsonFile();
            }

            var kvs = hostConfigReader.Reader();
            if (kvs.IsNullOrCount0())
            {
                return;
            }

            foreach (var kv in kvs)
            {
                var ps = new BalancerAddress[kv.Value.Length];
                for (var i = 0; i < ps.Length; i++)
                {
                    ps[i] = new BalancerAddress(kv.Value[i].Key, kv.Value[i].Value);
                }

                try
                {
                    GetCache().Add(kv.Key, ps);
                }
                catch (ArgumentException) // 忽略添加相同的键异常，为了预防密集的线程过来
                {
                    System.Console.WriteLine($"{this.GetType().Name}.发生相同添加相同的key异常(程序忽略),key:{kv.Key}.value:{ps}");
                }
            }
        }

        /// <summary>
        /// 读取
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>值</returns>
        public BalancerAddress[] Reader(string key)
        {
            return Get(key);
        }

        /// <summary>
        /// 获取缓存对象
        /// </summary>
        /// <returns>缓存对象</returns>
        protected override IDictionary<string, BalancerAddress[]> GetCache()
        {
            return dicCaches;
        }
    }
}
