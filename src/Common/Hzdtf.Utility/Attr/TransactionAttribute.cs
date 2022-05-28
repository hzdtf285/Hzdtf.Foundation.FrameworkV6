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
        /// 执行事务方法前的方法名
        /// 方法名必须是public
        /// 用法主要在事务开启之前，需要从持久化读取数据，而读取数据一般不需要加入事务，因为一旦读取加入事务，在事务完成之前会锁定读取的数据范围，影响性能
        /// </summary>
        public string BeforeMethod
        {
            get;
            set;
        }

        /// <summary>
        /// 执行事务方法前的方法是否使用缓存
        /// 如果使用的单实例，建议使用以提高性能。否则不需要使用
        /// 默认为否
        /// </summary>
        public bool BeforeMethodUseCache
        {
            get;
            set;
        }

        /// <summary>
        /// 执行方法前的方法返回值输入位置索引
        /// 默认为-1，为-1时，表示方法没有返回值
        /// </summary>
        public sbyte BeforeMethodReturnValueInIndex
        {
            get;
            set;
        } = -1;

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
