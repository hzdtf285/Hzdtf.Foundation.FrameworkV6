using System;
using System.Collections.Generic;

namespace Hzdtf.Utility.Cache
{
    /// <summary>
    /// 单类型的本地内存基类
    /// 本类不会加锁，子类必须要考虑使用线程安全的字典
    /// @ 黄振东
    /// </summary>
    /// <typeparam name="KeyT">键类型</typeparam>
    /// <typeparam name="ValueT">值类型</typeparam>
    public abstract class SingleTypeLocalMemoryBase<KeyT, ValueT> : ISingleTypeCache<KeyT, ValueT>
    {
        #region ICacheWrite<KeyT, ValueT> 接口

        /// <summary>
        /// 缓存键数量
        /// </summary>
        public int Count { get => GetCache().Count; }

        /// <summary>
        /// 判断键是否存在
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>键是否存在</returns>
        public virtual bool Exists(KeyT key)
        {
            if (key == null)
            {
                return false;
            }
            return GetCache().ContainsKey(key);
        }
                
        /// <summary>
        /// 根据键获取值
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>值</returns>
        public virtual ValueT Get(KeyT key)
        {
            if (key == null)
            {
                return default(ValueT);
            }
            ValueT value;
            if (GetCache().TryGetValue(key, out value))
            {
                return value;
            }
            else
            {
                return default(ValueT);
            }
        }

        /// <summary>
        /// 读取
        /// </summary>
        /// <returns>数据</returns>
        public virtual IDictionary<KeyT, ValueT> Reader() => GetCache();

        /// <summary>
        /// 添加
        /// 如果存在则不添加，返回false
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns>是否添加成功</returns>
        public virtual bool Add(KeyT key, ValueT value)
        {
            if (Exists(key))
            {
                return false;
            }
            try
            {
                GetCache().Add(key, value);
            }
            catch (ArgumentException) // 忽略添加相同的键异常，为了预防密集的线程过来
            {
                System.Console.WriteLine($"{this.GetType().Name}.发生相同添加相同的key异常(程序忽略),key:{key}.value:{value}");
            }

            return true;
        }

        /// <summary>
        /// 更新
        /// 如果不存在则不更新，返回false
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns>是否添加成功</returns>
        public virtual bool Update(KeyT key, ValueT value)
        {
            if (Exists(key))
            {
                GetCache()[key] = value;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 设置
        /// 如果存在则更新，不存在则添加
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns>是否设置成功</returns>
        public virtual bool Set(KeyT key, ValueT value)
        {
            if (Exists(key))
            {
                if (Update(key, value))
                {
                    return true;
                }
                else
                {
                    return Add(key, value);
                }
            }
            else
            {
                return Add(key, value);
            }
        }

        /// <summary>
        /// 移除
        /// 如果存在则删除并返回true，否则返回false
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>是否移除成功</returns>
        public virtual bool Remove(KeyT key)
        {
            if (Exists(key))
            {
                if (Exists(key))
                {
                    return GetCache().Remove(key);
                }
                else
                {
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="keys">键数组</param>
        /// <returns>是否移除成功</returns>
        public virtual bool Remove(KeyT[] keys)
        {
            foreach (KeyT key in keys)
            {
                if (Exists(key))
                {
                    GetCache().Remove(key);
                }
            }

            return true;
        }

        /// <summary>
        /// 清空
        /// </summary>
        public virtual void Clear()
        {
            GetCache().Clear();
        }

        #endregion

        #region 需要子类重写的方法

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <returns>缓存</returns>
        protected abstract IDictionary<KeyT, ValueT> GetCache();

        #endregion

        #region 受保护的方法

        /// <summary>
        /// 设置全部
        /// </summary>
        /// <param name="keyValues">键值对</param>
        protected void SetAll(IDictionary<KeyT, ValueT> keyValues)
        {
            var dic = GetCache();
            dic = keyValues;
        }

        #endregion
    }
}
