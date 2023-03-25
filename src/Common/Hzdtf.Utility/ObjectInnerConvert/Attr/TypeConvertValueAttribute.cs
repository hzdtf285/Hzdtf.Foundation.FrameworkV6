using Hzdtf.Utility.Conversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hzdtf.Utility.ObjectInnerConvert.Attr
{
    /// <summary>
    /// 类型转换值特性
    /// @ 黄振东
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class TypeConvertValueAttribute : Attribute
    {
        /// <summary>
        /// 转换类型
        /// </summary>
        public TypeConvertEnum ConvertType
        {
            get;
            set;
        } = TypeConvertEnum.None;

        /// <summary>
        /// 转换
        /// </summary>
        public IConvertable Convert
        {
            get;
            set;
        }
    }
}
