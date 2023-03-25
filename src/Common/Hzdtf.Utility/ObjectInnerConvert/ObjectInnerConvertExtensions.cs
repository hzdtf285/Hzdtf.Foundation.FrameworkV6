using Hzdtf.Utility;
using Hzdtf.Utility.ObjectInnerConvert;
using Hzdtf.Utility.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    /// <summary>
    /// 对象内部转换扩展类
    /// @ 黄振东
    /// </summary>
    public static class ObjectInnerConvertExtensions
    {
        /// <summary>
        /// 内部转换
        /// </summary>
        private static IObjectInnerConvert _innerConvert;

        /// <summary>
        /// 内部转换
        /// </summary>
        private static IObjectInnerConvert innerConvert
        {
            get
            {
                if (_innerConvert == null)
                {
                    lock (syncInnerConvert)
                    {
                        if (_innerConvert == null)
                        {
                            _innerConvert = App.GetServiceFromInstance<IObjectInnerConvert>();
                            if (_innerConvert == null)
                            {
                                _innerConvert = new ObjectInnerConvert();
                            }
                        }
                    }
                }

                return _innerConvert;
            }
        }

        /// <summary>
        /// 同步内部转换
        /// </summary>
        private static readonly object syncInnerConvert = new object();

        /// <summary>
        /// 类型转换转换值
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="options">配置回调</param>
        public static void TypeConvertValue(this object obj, Action<ObjectInnerConvertOptions> options = null)
        {
            innerConvert.Convert(obj, options);
        }

        /// <summary>
        /// 类型转换转换值
        /// </summary>
        /// <param name="options">配置回调</param>
        /// <param name="obj">对象</param>
        public static void TypeConvertValues(Action<ObjectInnerConvertOptions> options = null, params object[] obj)
        {
            if (obj.IsNullOrLength0())
            {
                return;
            }

            foreach (var o in obj)
            {
                innerConvert.Convert(o, options);
            }
        }
    }
}
