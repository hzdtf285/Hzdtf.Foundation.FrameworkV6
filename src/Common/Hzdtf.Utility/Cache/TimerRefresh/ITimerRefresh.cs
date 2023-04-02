using Hzdtf.Utility.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hzdtf.Utility.Cache.TimerRefresh
{
    /// <summary>
    /// 定时刷新接口
    /// @ 黄振东
    /// </summary>
    /// <typeparam name="ValueT">值类型</typeparam>
    public interface ITimerRefresh<ValueT> : IReader<ValueT>
    {
        /// <summary>
        /// 定时刷新后事件
        /// </summary>
        event Action<object, ValueT> TimerRefreshed;

        /// <summary>
        /// 刷新
        /// </summary>
        /// <param name="state">状态</param>
        /// <returns>值</returns>
        ValueT Refresh(object state);
    }
}
