using Hzdtf.Utility.Attr;
using Grpc.Net.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using Hzdtf.Utility.Pool;
using Hzdtf.Utility.Data;
using Hzdtf.Utility.Utils;

namespace Hzdtf.Utility.GRpc.Pool
{
    /// <summary>
    /// GRpc渠道池
    /// </summary>
    [Inject]
    public class GRpcChannelPool : ResourcePoolBase<string, GrpcChannel, GrpcChannelOptions>, IGRpcChannelPool
    {
        /// <summary>
        /// 缓存字典
        /// </summary>
        private static readonly ConcurrentDictionary<string, List<ResourceStatusInfo<GrpcChannel>>> dicCache = new ConcurrentDictionary<string, List<ResourceStatusInfo<GrpcChannel>>>();

        /// <summary>
        /// 同步键缓存
        /// </summary>
        private static readonly ConcurrentDictionary<string, object> syncKeyCache = new ConcurrentDictionary<string, object>();

        /// <summary>
        /// 等待资源队列
        /// </summary>
        private static readonly ConcurrentDictionary<string, ConcurrentQueue<string>> waitResourseQueues = new ConcurrentDictionary<string, ConcurrentQueue<string>>();

        /// <summary>
        /// 忽略等待资源ID缓存
        /// </summary>
        private static readonly IList<string> ignoreWaitResourceIds = new List<string>();

        /// <summary>
        /// 同步忽略等待资源ID缓存
        /// </summary>
        private static readonly object syncIgnoreWaitResourceIds = new object();

        /// <summary>
        /// 同步同步资源状态字典
        /// </summary>
        private static readonly ConcurrentDictionary<ResourceStatusInfo<GrpcChannel>, object> syncResourceStatus = new ConcurrentDictionary<ResourceStatusInfo<GrpcChannel>, object>();

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="getConfig">获取配置</param>
        public GRpcChannelPool(IGetObject<PoolConfigInfo<string, GrpcChannelOptions>> getConfig = null)
            : base(getConfig)
        { }

        /// <summary>
        /// 创建资源值
        /// </summary>
        /// <param name="key">资源键</param>
        /// <returns>资源值</returns>
        protected override GrpcChannel Create(string key)
        {
            if (!Config.ConcreateResourceOptiones.IsNullOrCount0() && Config.ConcreateResourceOptiones.ContainsKey(key))
            {
                return GrpcChannel.ForAddress(key, Config.ConcreateResourceOptiones[key]);
            }
            else
            {
                if (Config.GlobalConcreateResourceOptions == null)
                {
                    return GrpcChannel.ForAddress(key);
                }
                else
                {
                    return GrpcChannel.ForAddress(key, Config.GlobalConcreateResourceOptions);
                }
            }        
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <returns>缓存</returns>
        protected override ConcurrentDictionary<string, List<ResourceStatusInfo<GrpcChannel>>> GetCache() => dicCache;

        /// <summary>
        /// 获取同步缓存键
        /// </summary>
        /// <param name="key">资源键</param>
        /// <returns>同步缓存键</returns>
        protected override object GetSyncCacheKey(string key)
        {
            return syncKeyCache.GetOrAdd(key, x =>
            {
                return new object();
            });
        }

        /// <summary>
        /// 释放资源值
        /// </summary>
        /// <param name="resourceValue">资源值</param>
        protected override void Release(GrpcChannel resourceValue)
        {
            try
            {
                resourceValue.ShutdownAsync();
                resourceValue.Dispose();
            }
            catch { }
        }

        /// <summary>
        /// 获取等待资源队列
        /// key：资源键
        /// value：资源值ID
        /// </summary>
        /// <returns>等待资源队列</returns>
        protected override ConcurrentDictionary<string, ConcurrentQueue<string>> GetWaitResourceQueues() => waitResourseQueues;

        /// <summary>
        /// 获取忽略等待资源ID缓存
        /// </summary>
        /// <returns>忽略等待资源ID缓存</returns>
        protected override IList<string> GetIgnoreWaitResourceIdCache() => ignoreWaitResourceIds;

        /// <summary>
        /// 获取忽略同步等待资源ID缓存
        /// </summary>
        /// <returns>忽略同步等待资源ID缓存</returns>
        protected override object GetSyncIgnoreWaitResourceIdCache() => syncIgnoreWaitResourceIds;

        /// <summary>
        /// 获取同步资源状态
        /// </summary>
        /// <param name="rs">资源状态</param>
        /// <returns>同步资源状态</returns>
        protected override object GetSyncResourceStatus(ResourceStatusInfo<GrpcChannel> rs)
        {
            return syncResourceStatus.GetOrAdd(rs, x =>
            {
                return new object();
            });
        }
    }
}
