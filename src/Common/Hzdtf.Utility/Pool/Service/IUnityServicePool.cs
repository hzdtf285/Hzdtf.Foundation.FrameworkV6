using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Hzdtf.Utility.Pool.Service
{
    /// <summary>
    /// 统计服务池接口
    /// @ 黄振东
    /// </summary>
    /// <typeparam name="ResourceKeyT">资源键类型</typeparam>
    /// <typeparam name="ResourceValueT">资源值类型</typeparam>
    public interface IUnityServicePool<ResourceKeyT, ResourceValueT>
        where ResourceValueT : class
    {
        /// <summary>
        /// 异步生成资源值
        /// </summary>
        /// <param name="serviceName">服务名</param>
        /// <param name="path">路径</param>
        /// <param name="tag">标签</param>
        /// <returns>生成资源值</returns>
        Task<ResourceValueT> BuilderAsync(string serviceName, string path = null, string tag = null);

        /// <summary>
        /// 回收，使用后需要执行回收
        /// </summary>
        /// <param name="value">资源值</param>
        void Recycle(ResourceValueT value);

        /// <summary>
        /// 执行，会自动回收
        /// </summary>
        /// <param name="key">资源键</param>
        /// <param name="action">回调</param>
        void Exec(ResourceKeyT key, Action<ResourceValueT> action);

        /// <summary>
        /// 异步执行，会自动回收
        /// </summary>
        /// <param name="key">资源键</param>
        /// <param name="func">回调</param>
        Task ExecAsync(ResourceKeyT key, Func<ResourceValueT, Task> func);
    }
}
