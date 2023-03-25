using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hzdtf.Utility.ObjectInnerConvert
{
    /// <summary>
    /// 对象内部转换接口
    /// @ 黄振东
    /// </summary>
    public interface IObjectInnerConvert
    {
        /// <summary>
        /// 转换
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="options">配置回调</param>
        void Convert(object obj, Action<ObjectInnerConvertOptions> options = null);
    }
}
