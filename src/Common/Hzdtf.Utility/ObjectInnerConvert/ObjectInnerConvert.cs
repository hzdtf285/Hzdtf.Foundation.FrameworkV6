using Hzdtf.Utility.Attr;
using Hzdtf.Utility.Cache;
using Hzdtf.Utility.Conversion;
using Hzdtf.Utility.ObjectInnerConvert.Attr;
using Hzdtf.Utility.Utils;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Hzdtf.Utility.ObjectInnerConvert
{
    /// <summary>
    /// 对象内部转换
    /// @ 黄振东
    /// </summary>
    [Inject]
    public class ObjectInnerConvert : SingleTypeLocalMemoryBase<Type, PropertyInfo[]>, IObjectInnerConvert
    {
        /// <summary>
        /// 字典缓存
        /// </summary>
        private static readonly IDictionary<Type, PropertyInfo[]> dicCache = new ConcurrentDictionary<Type, PropertyInfo[]>();

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <returns>缓存</returns>
        protected override IDictionary<Type, PropertyInfo[]> GetCache() => dicCache;

        /// <summary>
        /// 转换
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="options">配置回调</param>
        public void Convert(object obj, Action<ObjectInnerConvertOptions> options = null)
        {
            if (obj == null)
            {
                return;
            }

            var op = new ObjectInnerConvertOptions();
            if (options != null)
            {
                options(op);
            }

            Convert(obj, op, 1);
        }

        /// <summary>
        /// 转换
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="op">配置</param>
        /// <param name="level">层级，从1开始</param>
        private void Convert(object obj, ObjectInnerConvertOptions op, byte level)
        {
            if (obj == null)
            {
                return;
            }

            var type = obj.GetType();
            if (type.IsGenericType)
            {
                IDictionary dic = null;
                if (obj is IDictionary)
                {
                    dic = obj as IDictionary;
                }

                var enumer = (obj as IEnumerable).GetEnumerator();
                PropertyInfo[] properties = null;
                Type subType = null;
                while (enumer.MoveNext())
                {
                    if (subType == null)
                    {
                        subType = enumer.Current.GetType();
                        // 如果是字典
                        if (dic != null)
                        {
                            var keyProp = enumer.Current.GetType().GetProperty("Key");
                            var valueProp = enumer.Current.GetType().GetProperty("Value");
                            var isGen = keyProp.PropertyType.IsGenericType && !keyProp.PropertyType.IsValueType;
                            if (keyProp.PropertyType.IsClass || isGen)
                            {
                                var keyObj = keyProp.GetValue(enumer.Current);
                                if (keyObj != null)
                                {
                                    var keyType = keyObj.GetType();
                                    var props = GetPropertys(keyType, op);
                                    ConvertSingleObject(keyProp, props, level, op);
                                }
                            }
                            if (valueProp.PropertyType.IsClass || isGen)
                            {
                                var valueObj = valueProp.GetValue(enumer.Current);
                                if (valueObj != null)
                                {
                                    var valueType = valueObj.GetType();
                                    var props = GetPropertys(valueType, op);
                                    ConvertSingleObject(valueObj, props, level, op);
                                }
                            }

                            continue;
                        }

                        properties = GetPropertys(subType, op);
                        if (properties.IsNullOrLength0())
                        {
                            return;
                        }
                    }
                    ConvertSingleObject(enumer.Current, properties, level, op);
                }
            }
            else
            {
                ConvertSingleObject(obj, GetPropertys(type, op), level, op);
            }
        }

        /// <summary>
        /// 根据类型获取属性信息数组
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="op">配置</param>
        /// <returns>属性信息数组</returns>
        private PropertyInfo[] GetPropertys(Type type, ObjectInnerConvertOptions op)
        {
            PropertyInfo[] properties = null;
            if (op.ObjTypeAddCache && dicCache.ContainsKey(type))
            {
                properties = dicCache[type];
            }
            else
            {
                properties = type.GetProperties().Where(p => p.CanWrite && p.CanWrite).ToArray();
                if (op.ObjTypeAddCache)
                {
                    Add(type, properties);
                }
            }

            return properties;
        }

        /// <summary>
        /// 转换单个对象
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="level">层级，从1开始</param>
        /// <param name="properties">属性信息数组</param>
        /// <param name="op">配置</param>
        private void ConvertSingleObject(object obj, PropertyInfo[] properties, byte level, ObjectInnerConvertOptions op = null)
        {
            if (obj == null || properties.IsNullOrLength0())
            {
                return;
            }

            foreach (var property in properties)
            {
                if (op != null && !op.IgnorePropNames.IsNullOrCount0() && op.IgnorePropNames.Contains(property.Name))
                {
                    continue;
                }

                var value = property.GetValue(obj);
                if (value == null)
                {
                    continue;
                }

                // 如果属性是类，则重新调用转换
                if (property.PropertyType.IsClass || (property.PropertyType.IsGenericType && !property.PropertyType.IsValueType))
                {
                    // 如果超过最大层级，则忽略
                    if (op.MaxDepth != 0 && level >= op.MaxDepth)
                    {
                        continue;
                    }

                    Convert(value, op, (byte)(level + 1));

                    continue;
                }

                // 先找传过来的参数转换，如果没有找到，则找对象属性的转换
                IConvertable convert = null;
                if (op != null && !op.Converts.IsNullOrCount0() && op.Converts.ContainsKey(property.PropertyType))
                {
                    convert = op.Converts[property.PropertyType];
                }
                else
                {
                    var typeConAttr = property.GetCustomAttribute<TypeConvertValueAttribute>();
                    if (typeConAttr == null) 
                    {
                        continue;
                    }
                    if (typeConAttr.Convert == null)
                    {
                        convert = TypeConvertFactory.GetConvert(typeConAttr.ConvertType);
                    }
                    else
                    {
                        convert = typeConAttr.Convert;
                    }
                }

                if (convert == null)
                {
                    continue;
                }

                property.SetValue(obj, convert.To(value));
            }
        }
    }
}
