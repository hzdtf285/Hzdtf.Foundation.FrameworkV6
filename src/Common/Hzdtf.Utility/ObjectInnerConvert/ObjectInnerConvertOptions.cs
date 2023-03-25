using Hzdtf.Utility.Conversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hzdtf.Utility.ObjectInnerConvert
{
    /// <summary>
    /// 对象内部转换选项配置
    /// </summary>
    public sealed class ObjectInnerConvertOptions
    {
        /// <summary>
        /// 自定义转换
        /// key：需要转换的数据类型
        /// value：自定义转换具体实现
        /// </summary>
        public IDictionary<Type, IConvertable> Converts { get; set; }

        /// <summary>
        /// 忽略的属性名称数组，区别大小写
        /// </summary>
        public IList<string> IgnorePropNames { get; set; }

        /// <summary>
        /// 最大深度，为了避免死循环，请尽量设置。如果对象存在相互引用，则必须设置
        /// 为0是不限制
        /// 默认为3
        /// </summary>
        public byte MaxDepth
        {
            get;
            set;
        } = 3;

        /// <summary>
        /// 对象类型是否添加到缓存里，如果是，则有利于提交效率，默认为是
        /// </summary>
        public bool ObjTypeAddCache
        {
            get;
            set;
        } = true;
    }
}
