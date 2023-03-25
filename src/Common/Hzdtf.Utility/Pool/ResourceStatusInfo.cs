using Hzdtf.Utility.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hzdtf.Utility.Pool
{
    /// <summary>
    /// 资源状态信息
    /// @ 黄振东
    /// </summary>
    /// <typeparam name="ResourceT">资源类型</typeparam>
    public class ResourceStatusInfo<ResourceT> where ResourceT : class
    {
        /// <summary>
        /// 使用中数量
        /// </summary>
        public uint UseingLength
        {
            get;
            set;
        }

        /// <summary>
        /// 开始一次使用时间
        /// </summary>
        public DateTime StartUseTime
        {
            get;
            set;
        }

        /// <summary>
        /// 结束使用时间
        /// </summary>
        public DateTime EndUseTime
        {
            get;
            set;
        }

        /// <summary>
        /// 资源
        /// </summary>
        public ResourceT Resource
        {
            get;
            set;
        }

        /// <summary>
        /// 统计现在与结束时间的间隔毫秒数
        /// </summary>
        public double TotalEndMillseconds
        {
            get
            {
                if (EndUseTime == DateTime.MinValue)
                {
                    return 0;
                }
                var ts = DateTimeExtensions.Now - EndUseTime;
                return ts.TotalMilliseconds;
            }
        }
    }
}
