using Hzdtf.Utility.Conversion;
using Hzdtf.Utility.ObjectInnerConvert.ValueConvert;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hzdtf.Utility.ObjectInnerConvert
{
    /// <summary>
    /// 类型转换工厂
    /// @ 黄振东
    /// </summary>
    public static class TypeConvertFactory
    {
        /// <summary>
        /// 日期时间转换为本地
        /// </summary>
        private static readonly IConvertable timeConvertLocal = new DateTimeConvertLocal();

        /// <summary>
        /// 获取转换
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>转换</returns>
        public static IConvertable GetConvert(TypeConvertEnum type)
        {
            switch (type)
            {
                case TypeConvertEnum.LocalTime:
                    return timeConvertLocal;

                default:
                    return null;
            }
        }
    }
}
