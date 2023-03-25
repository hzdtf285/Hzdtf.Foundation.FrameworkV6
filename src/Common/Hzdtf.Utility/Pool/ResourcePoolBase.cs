using Hzdtf.Utility.AsyncCallbackWrap;
using Hzdtf.Utility.Data;
using Hzdtf.Utility.Utils;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hzdtf.Utility.Pool
{
    /// <summary>
    /// 资源池基类
    /// @ 黄振东
    /// </summary>
    /// <typeparam name="ResourceKeyT">资源键类型</typeparam>
    /// <typeparam name="ResourceValueT">资源值类型</typeparam>
    /// <typeparam name="ConcreateReourseOptionsT">具体资源配置类型</typeparam>
    public abstract class ResourcePoolBase<ResourceKeyT, ResourceValueT, ConcreateReourseOptionsT> : IResourcePool<ResourceKeyT, ResourceValueT, ConcreateReourseOptionsT>
        where ResourceValueT : class
    {
        /// <summary>
        /// 异步释放
        /// </summary>
        private readonly IAsyncReleaseString asyncRelease = new AsyncReleaseString();

        /// <summary>
        /// 配置
        /// </summary>
        private readonly PoolConfigInfo<ResourceKeyT, ConcreateReourseOptionsT> config;

        /// <summary>
        /// 定时器
        /// </summary>
        private Timer timer;

        /// <summary>
        /// 同步定时器
        /// </summary>
        private readonly object syncTimer = new object();

        /// <summary>
        /// 配置
        /// </summary>
        public PoolConfigInfo<ResourceKeyT, ConcreateReourseOptionsT> Config
        {
            get => config;
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="getConfig">获取配置</param>
        public ResourcePoolBase(IGetObject<PoolConfigInfo<ResourceKeyT, ConcreateReourseOptionsT>> getConfig = null)
        {
            if (getConfig != null)
            {
                config = getConfig.Get();
            }
            if (config == null)
            {
                config = new PoolConfigInfo<ResourceKeyT, ConcreateReourseOptionsT>();
            }
        }

        /// <summary>
        /// 根据键获取值
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>值</returns>
        public ResourceValueT Get(ResourceKeyT key)
        {
            InitTimer();
            lock (GetSyncCacheKey(key))
            {
                var dicCaches = GetCache();
                if (dicCaches.ContainsKey(key))
                {
                    var list = dicCaches.GetValue(key);
                    return GetOptimalResourceStatus(key, list).Resource;
                }
                else
                {
                    var rs = new ResourceStatusInfo<ResourceValueT>()
                    {
                        Resource = Create(key)
                    };
                    var list = new List<ResourceStatusInfo<ResourceValueT>>();
                    dicCaches.TryAdd(key, list);
                    list.Add(rs);

                    SetStartUse(rs);

                    return rs.Resource;
                }
            }
        }

        /// <summary>
        /// 回收，使用后需要执行回收
        /// </summary>
        /// <param name="value">资源值</param>
        public void Recycle(ResourceValueT value)
        {
            ResourceKeyT key;
            var rs = GetResourceStatus(value, out key);
            if (rs == null)
            {
                return;
            }
            
            lock (GetSyncResourceStatus(rs))
            {
                rs.UseingLength--;
                rs.EndUseTime = DateTimeExtensions.Now;
            }

            var waitReses = GetWaitResourceQueues();
            ConcurrentQueue<string> queue;
            if (waitReses.TryGetValue(key, out queue) && queue != null)
            {
                var ignoreWaitResIds = GetIgnoreWaitResourceIdCache();
                while (true)
                {
                    string id;
                    if (queue.TryDequeue(out id))
                    {
                        if (ignoreWaitResIds.Contains(id))
                        {
                            continue;
                        }

                        asyncRelease.Release(id);

                        break;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 根据资源键移除
        /// </summary>
        /// <param name="key">资源键</param>
        public void Remove(ResourceKeyT key)
        {
            var dicCache = GetCache();
            if (dicCache.IsNullOrCount0())
            {
                return;
            }
            
            if (dicCache.ContainsKey(key))
            {
                var list = dicCache[key];
                if (list.IsNullOrCount0())
                {
                    return;
                }

                lock (GetSyncCacheKey(key))
                {
                    foreach (var item in list)
                    {
                        Release(item.Resource);
                    }

                    dicCache.RemoveKey(key);
                }
            }
        }

        /// <summary>
        /// 执行，会自动回收
        /// </summary>
        /// <param name="key">资源键</param>
        /// <param name="action">回调</param>
        public void Exec(ResourceKeyT key, Action<ResourceValueT> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("回调不能为null");
            }
            var value = Get(key);
            try
            {
                action(value);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            finally
            {
                Recycle(value);
            }
        }

        /// <summary>
        /// 异步执行，会自动回收
        /// </summary>
        /// <param name="key">资源键</param>
        /// <param name="func">回调</param>
        public async Task ExecAsync(ResourceKeyT key, Func<ResourceValueT, Task> func)
        {
            if (func == null)
            {
                throw new ArgumentNullException("回调不能为null");
            }
            await Task.Run(() =>
            {
                Exec(key, value =>
                {
                    func(value);
                });
            });
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public void Close()
        {
            EachResourceStatus((k, v) =>
            {
                Release(v.Resource);
                return false;
            });
        }

        /// <summary>
        /// 从列表里获取最优的资源状态
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="list">列表</param>
        /// <returns>资源状态</returns>
        private ResourceStatusInfo<ResourceValueT> GetOptimalResourceStatus(ResourceKeyT key, List<ResourceStatusInfo<ResourceValueT>> list)
        {
            // 如果列表里为空，则直接创建使用
            if (list.Count == 0)
            {
                var rs1 = new ResourceStatusInfo<ResourceValueT>()
                {
                    Resource = Create(key)
                };
                list.Add(rs1);
                SetStartUse(rs1);

                return rs1;
            }

            // 从列表里获取可用的资源状态（排除掉超过最大使用数量，且按使用中数量从小排序）
            var canList = config.MaxSingleUseSize > 0 ? list.Where(p => p.UseingLength + 1 <= config.MaxSingleUseSize).ToList() : list;
            // 如果单个最大数量都满了，则看是否超过最大池，如果超过，则进入等待期，否则新创建
            var rs = FilterOptimalResourceStatus(canList);
            if (rs == null)
            {
                // 如果超过了最大池数量，则进入等待
                if (config.MaxPoolSize > 0 && list.Count >= config.MaxPoolSize)
                {
                    var waitReses = GetWaitResourceQueues();
                    var queue = waitReses.GetOrAdd(key, x =>
                    {
                        return new ConcurrentQueue<string>();
                    });
                    var id = StringUtil.NewShortGuid();
                    queue.Enqueue(id);

                    // 如果是超时，则需要加入忽略ID
                    if (!asyncRelease.Wait(id, TimeSpan.FromMilliseconds(config.TimeoutMillseconds)))
                    {
                        lock (GetSyncIgnoreWaitResourceIdCache())
                        {
                            GetIgnoreWaitResourceIdCache().Add(id);
                        }
                    }

                    rs = FilterOptimalResourceStatus(canList);
                    if (rs == null)
                    {
                        throw new Exception("获取资源已超时");
                    }
                }
                else
                {
                    rs = new ResourceStatusInfo<ResourceValueT>()
                    {
                        Resource = Create(key)
                    };
                    list.Add(rs);
                }
            }

            SetStartUse(rs);

            return rs;
        }

        /// <summary>
        /// 开始使用
        /// </summary>
        /// <param name="rs">资源状态</param>
        private void SetStartUse(ResourceStatusInfo<ResourceValueT> rs)
        {
            lock (GetSyncResourceStatus(rs))
            {
                rs.UseingLength++;
                rs.StartUseTime = DateTimeExtensions.Now;
            }
        }

        /// <summary>
        /// 筛选从最优的资源状态
        /// </summary>
        /// <param name="list">资源状态列表</param>
        /// <returns>资源状态</returns>
        protected virtual ResourceStatusInfo<ResourceValueT> FilterOptimalResourceStatus(List<ResourceStatusInfo<ResourceValueT>> list)
        {
            return list.OrderBy(x => x.UseingLength).OrderByDescending(x => x.EndUseTime).FirstOrDefault();
        }

        /// <summary>
        /// 根据资源值获取资源状态
        /// </summary>
        /// <param name="resourceValue">资源值</param>
        /// <param name="key">资源键</param>
        /// <returns>资源状态</returns>
        private ResourceStatusInfo<ResourceValueT> GetResourceStatus(ResourceValueT resourceValue, out ResourceKeyT key)
        {
            ResourceStatusInfo<ResourceValueT> rv = null;
            ResourceKeyT tempKey = default(ResourceKeyT);
            EachResourceStatus((k, v) =>
            {
                if (v.Resource == resourceValue)
                {
                    rv = v;
                    tempKey = k;
                    return true;  
                }
                else
                {
                    return false;
                }
            });
            key = tempKey;

            return rv;
        }

        /// <summary>
        /// 循环资源状态
        /// </summary>
        /// <param name="callback">回调</param>
        private void EachResourceStatus(Func<ResourceKeyT, ResourceStatusInfo<ResourceValueT>, bool> callback)
        {
            var dicCaches = GetCache();
            if (dicCaches.IsNullOrCount0())
            {
                return;
            }

            foreach (var item in dicCaches)
            {
                if (item.Value.IsNullOrCount0())
                {
                    continue;
                }

                foreach (var item2 in item.Value)
                {
                    if (callback(item.Key, item2))
                    {
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// 初始化定时器
        /// </summary>
        private void InitTimer()
        {
            if (timer != null || config.TimerCheckIntervalMillSeconds == 0)
            {
                return;
            }

            lock (syncTimer)
            {
                if (timer == null)
                {
                    timer = new Timer(st =>
                    {
                        CheckIdleResource();
                    }, null, 0, config.TimerCheckIntervalMillSeconds);
                }
            }
        }

        /// <summary>
        /// 检查空闲资源
        /// </summary>
        private void CheckIdleResource()
        {
            var dicCaches = GetCache();
            if (dicCaches.IsNullOrCount0())
            {
                return;
            }

            foreach (var item in dicCaches)
            {
                if (item.Value.IsNullOrCount0())
                {
                    continue;
                }
                
                lock (GetSyncCacheKey(item.Key))
                {
                    item.Value.RemoveAll(p => p.UseingLength == 0 && p.TotalEndMillseconds > 0 && config.MaxIdleMillseconds > 0 && p.TotalEndMillseconds >= config.MaxIdleMillseconds);
                }
            }
        }

        /// <summary>
        /// 创建资源值
        /// </summary>
        /// <param name="key">资源键</param>
        /// <returns>资源值</returns>
        protected abstract ResourceValueT Create(ResourceKeyT key);

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <returns>缓存</returns>
        protected abstract ConcurrentDictionary<ResourceKeyT, List<ResourceStatusInfo<ResourceValueT>>> GetCache();

        /// <summary>
        /// 获取同步缓存键
        /// </summary>
        /// <param name="key">资源键</param>
        /// <returns>同步缓存键</returns>
        protected abstract object GetSyncCacheKey(ResourceKeyT key);

        /// <summary>
        /// 释放资源值
        /// </summary>
        /// <param name="resourceValue">资源值</param>
        protected abstract void Release(ResourceValueT resourceValue);

        /// <summary>
        /// 获取等待资源队列
        /// key：资源键
        /// value：资源值ID
        /// </summary>
        /// <returns>等待资源队列</returns>
        protected abstract ConcurrentDictionary<ResourceKeyT, ConcurrentQueue<string>> GetWaitResourceQueues();

        /// <summary>
        /// 获取忽略等待资源ID缓存
        /// </summary>
        /// <returns>忽略等待资源ID缓存</returns>
        protected abstract IList<string> GetIgnoreWaitResourceIdCache();

        /// <summary>
        /// 获取同步忽略等待资源ID缓存
        /// </summary>
        /// <returns>同步忽略等待资源ID缓存</returns>
        protected abstract object GetSyncIgnoreWaitResourceIdCache();

        /// <summary>
        /// 获取同步资源状态
        /// </summary>
        /// <param name="rs">资源状态</param>
        /// <returns>同步资源状态</returns>
        protected abstract object GetSyncResourceStatus(ResourceStatusInfo<ResourceValueT> rs);
    }
}
