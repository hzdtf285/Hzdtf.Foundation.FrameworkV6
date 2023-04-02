using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hzdtf.Utility.Pool
{
    /// <summary>
    /// 池配置信息
    /// @ 黄振东
    /// </summary>
    /// <typeparam name="ResourceKeyT">资源键类型</typeparam>
    /// <typeparam name="ConcreateReourseOptionsT">具体资源配置类型</typeparam>
    public class PoolConfigInfo<ResourceKeyT, ConcreateReourseOptionsT>
    {
        /// <summary>
        /// 最大的池大小，0为不限制，默认为100
        /// 如果超过了，则进入等待
        /// </summary>
        public uint MaxPoolSize
        {
            get;
            set;
        } = 100;

        /// <summary>
        /// 最大单个使用大小，0为不限制，默认为20
        /// 如果超过了，则创建新的资源
        /// </summary>
        public uint MaxSingleUseSize
        {
            get;
            set;
        } = 20;

        /// <summary>
        /// 等待超时毫秒数，默认为60000，即1分钟
        /// </summary>
        public uint TimeoutMillseconds
        {
            get;
            set;
        } = 60000;

        /// <summary>
        /// 最大空闲毫秒数，如果超过，则会移除，0则不限制，默认为30分钟
        /// </summary>
        public uint MaxIdleMillseconds
        {
            get;
            set;
        } = 1800000;

        /// <summary>
        /// 定时器检测间隔毫秒数，如果为0，则不检查。默认为2分钟
        /// </summary>
        public uint TimerCheckIntervalMillSeconds
        {
            get;
            set;
        } = 120000;

        /// <summary>
        /// 全局具体资源配置
        /// </summary>
        public ConcreateReourseOptionsT GlobalConcreateResourceOptions
        {
            get;
            set;
        }

        /// <summary>
        /// 具体资源配置字典，如果按资源键未找到，则使用全局具体资源配置
        /// </summary>
        public IDictionary<ResourceKeyT, ConcreateReourseOptionsT> ConcreateResourceOptiones
        {
            get;
            set;
        }
    }
}
