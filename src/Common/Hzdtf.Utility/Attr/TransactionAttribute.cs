using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Hzdtf.Utility.Attr
{
    /// <summary>
    /// 事务特性
    /// @ 黄振东
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class TransactionAttribute : Attribute
    {
        /// <summary>
        /// 连接ID位置索引
        /// 默认为-1，为-1时，表示方法没有连接ID的参数
        /// </summary>
        public sbyte ConnectionIdIndex
        {
            get;
            set;
        } = -1;

        /// <summary>
        /// 事务等级
        /// </summary>
        public IsolationLevel Level
        {
            get;
            set;
        } = IsolationLevel.ReadCommitted;

        /// <summary>
        /// 是否分布式事务，默认为否
        /// </summary>
        public bool IsDistribute
        {
            get;
            set;
        }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// 标签
        /// </summary>
        public object Tag
        {
            get;
            set;
        }

        /// <summary>
        /// 通用使用数据位置索引
        /// 默认为-1，为-1时，表示方法没有连接ID的参数
        /// 如果在事务提交成功后要执行后续的方法，请使用此参数，且添加CommonUseData.AddCallback。回调后，会自动清空所有回调
        /// </summary>
        public sbyte CommonUseDataIndex
        {
            get;
            set;
        } = -1;
    }
}
