using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hzdtf.Utility.Enums
{
    /// <summary>
    /// Like模式
    /// @ 黄振东
    /// </summary>
    public enum LikeMode : byte
    {
        /// <summary>
        /// 左匹配。相当于like '{value}%'
        /// </summary>
        LEFT_EQUAL = 0,

        /// <summary>
        /// 右匹配。相当于like '%{value}'
        /// </summary>
        RIGHT = 1,

        /// <summary>
        /// 全模糊，相当于like '%{value}%'
        /// </summary>
        FULL_BLUR = 2,
    }
}
