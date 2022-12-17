using Hzdtf.Utility.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    /// <summary>
    /// SQL属性信息
    /// @ 黄振东
    /// </summary>
    public class SqlPropInfo
    {
        /// <summary>
        /// 前面几条，如果为0，则不加此条件
        /// </summary>
        public uint Top
        {
            get;
            set;
        }

        /// <summary>
        /// 参数
        /// </summary>
        public object Params
        {
            get;
            set;
        }

        /// <summary>
        /// 属性名称集合，如果为空则获取全部
        /// </summary>
        public string[] PropertyNames
        {
            get;
            set;
        }

        /// <summary>
        /// 属性是否取反，如果取反，则propertyNames则为排除的属性名称集合
        /// </summary>
        public bool IsPropertyGetNot
        {
            get;
            set;
        }

        /// <summary>
        /// 过滤SQL，如果为空，则不添加任何条件语句
        /// 使用属性名过滤，属性名需要加前后辍，使用"SqlUtil.FilterPrefix属性名SqlUtil.FilterSuffixes"
        /// 注意不用加WHERE
        /// </summary>
        public string FilterSql
        {
            get;
            set;
        }

        /// <summary>
        /// 是否替换过滤SQL，如果过滤SQL包含有特殊属性，则需要设置为true执行替换为对应字段，如果没有，则设置为false能提高性能
        /// 默认为true
        /// </summary>
        public bool IsReplaceFilterSql
        {
            get;
            set;
        } = true;

        /// <summary>
        /// 属性映射排序类型字典
        /// key：属性名，value：排序类型
        /// </summary>
        public IDictionary<string, SortType> PropMapSortTypes
        {
            get;
            set;
        }
    }

    /// <summary>
    /// SQL属性扩展类
    /// @ 黄振东
    /// </summary>
    public static class SqlPropExtension
    {
        /// <summary>
        /// SQL封装属性名
        /// </summary>
        /// <param name="propName">属性名</param>
        /// <returns>封装后的属性名</returns>
        public static string SqlPackPropName(this string propName)
        {
            return string.Format("{0}{1}{2}", SqlUtil.FilterPrefix, propName, SqlUtil.FilterSuffixes);
        }
    }
}
