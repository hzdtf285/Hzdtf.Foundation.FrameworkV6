using Hzdtf.Utility.RemoteService.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Hzdtf.Utility.Pool.Service
{
    /// <summary>
    /// 统计服务池
    /// @ 黄振东
    /// </summary>
    /// <typeparam name="ResourceValueT">资源值类型</typeparam>
    /// <typeparam name="ConcreateReourseOptionsT">具体资源配置类型</typeparam>
    public class UnityServicePool<ResourceValueT, ConcreateReourseOptionsT> : IUnityServicePool<string, ResourceValueT>
        where ResourceValueT : class
    {
        /// <summary>
        /// 资源池
        /// </summary>
        private readonly IResourcePool<string, ResourceValueT, ConcreateReourseOptionsT> resourcePool;

        /// <summary>
        /// 统计服务生成器
        /// </summary>
        private readonly IUnityServicesBuilder servicesBuilder;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="resourcePool">资源池</param>
        /// <param name="servicesBuilder">统计服务生成器</param>
        public UnityServicePool(IResourcePool<string, ResourceValueT, ConcreateReourseOptionsT> resourcePool, IUnityServicesBuilder servicesBuilder)
        {
            this.resourcePool = resourcePool;
            this.servicesBuilder = servicesBuilder;
        }

        /// <summary>
        /// 异步生成资源值
        /// </summary>
        /// <param name="serviceName">服务名</param>
        /// <param name="path">路径</param>
        /// <param name="tag">标签</param>
        /// <returns>生成资源值</returns>
        public async Task<ResourceValueT> BuilderAsync(string serviceName, string path = null, string tag = null)
        {
            var addr = await servicesBuilder.BuilderAsync(serviceName, path, tag);
            if (string.IsNullOrWhiteSpace(addr))
            {
                return default(ResourceValueT);
            }

            return resourcePool.Get(addr);
        }

        /// <summary>
        /// 回收，使用后需要执行回收
        /// </summary>
        /// <param name="value">资源值</param>
        public void Recycle(ResourceValueT value) => resourcePool.Recycle(value);

        /// <summary>
        /// 执行，会自动回收
        /// </summary>
        /// <param name="key">资源键</param>
        /// <param name="action">回调</param>
        public void Exec(string key, Action<ResourceValueT> action) => resourcePool.Exec(key, action);

        /// <summary>
        /// 异步执行，会自动回收
        /// </summary>
        /// <param name="key">资源键</param>
        /// <param name="func">回调</param>
        public async Task ExecAsync(string key, Func<ResourceValueT, Task> func) => await resourcePool.ExecAsync(key, func);
    }
}
