using Hzdtf.Utility.Data;
using Hzdtf.Utility.Release;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hzdtf.Utility.Pool
{
    /// <summary>
    /// 资源池接口
    /// @ 黄振东
    /// </summary>
    /// <typeparam name="ResourceKeyT">资源键类型</typeparam>
    /// <typeparam name="ResourceValueT">资源值类型</typeparam>
    /// <typeparam name="ConcreateReourseOptionsT">具体资源配置类型</typeparam>
    public interface IResourcePool<ResourceKeyT, ResourceValueT, ConcreateReourseOptionsT> : IGetable<ResourceKeyT, ResourceValueT>, IClose
        where ResourceValueT : class
    {
        /// <summary>
        /// 配置
        /// </summary>
        PoolConfigInfo<ResourceKeyT, ConcreateReourseOptionsT> Config { get; }

        /// <summary>
        /// 回收，使用后需要执行回收
        /// </summary>
        /// <param name="value">资源值</param>
        void Recycle(ResourceValueT value);

        /// <summary>
        /// 根据资源键移除
        /// </summary>
        /// <param name="key">资源键</param>
        void Remove(ResourceKeyT key);

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
