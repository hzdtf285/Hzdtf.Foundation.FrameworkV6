﻿using Hzdtf.Utility.Attr;
using Hzdtf.Utility.Cache;
using Hzdtf.Utility.Model.Return;
using Polly;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hzdtf.Polly.Extensions
{
    /// <summary>
    /// 策略返回数据缓存
    /// @ 黄振东
    /// </summary>
    [Inject]
    public class PolicyReturnDataCache : SingleTypeLocalMemoryBase<string, IAsyncPolicy<ReturnInfo<object>>>, IPolicyReturnData
    {
        /// <summary>
        /// 缓存键
        /// </summary>
        private static readonly IDictionary<string, IAsyncPolicy<ReturnInfo<object>>> dicCaches = new ConcurrentDictionary<string, IAsyncPolicy<ReturnInfo<object>>>();

        /// <summary>
        /// 获取缓存对象
        /// </summary>
        /// <returns>缓存对象</returns>
        protected override IDictionary<string, IAsyncPolicy<ReturnInfo<object>>> GetCache()
        {
            return dicCaches;
        }

        /// <summary>
        /// 设置，如果存在则忽略，不存在则添加
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="options">选项配置</param>
        /// <returns>异步策略</returns>
        public IAsyncPolicy<ReturnInfo<object>> SetIgnoreExistskey(string key, Action<BreakerWrapPolicyOptions<ReturnInfo<object>>> options = null)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException("键不能为空");
            }

            if (dicCaches.ContainsKey(key))
            {
                return dicCaches[key];
            }

            var asyncPolicy = PolicyUtil.BuilderBreakerWrapPollicyReturnInfoAsync<Exception, object>(options);
            Set(key, asyncPolicy);

            return asyncPolicy;
        }

        /// <summary>
        /// 异步执行
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="execFunc">执行回调</param>
        /// <param name="options">选项配置</param>
        /// <returns>返回信息任务</returns>
        public Task<ReturnInfo<object>> ExecuteAsync(string key, Func<Task<ReturnInfo<object>>> execFunc, Action<BreakerWrapPolicyOptions<ReturnInfo<object>>> options = null)
        {
            return SetIgnoreExistskey(key, options).ExecuteAsync(execFunc);
        }
    }
}
