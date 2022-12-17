using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hzdtf.Utility.Listen
{
    /// <summary>
    /// 监听配置帮助类
    /// @ 黄振东
    /// </summary>
    public static class ListenConfigHelper
    {
        /// <summary>
        /// 读取配置
        /// </summary>
        /// <returns>配置</returns>
        public static ListenConfig Reader()
        {
            return $"{AppContext.BaseDirectory}/app.json".ToJsonObjectFromFile<ListenConfig>();
        }
    }
}
