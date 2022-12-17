using Hzdtf.Utility.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hzdtf.Utility.Resources
{
    /// <summary>
    /// 数据库服务基类
    /// @ 黄振东
    /// </summary>
    public abstract class DatabasePoolServiceBase
    {
        /// <summary>
        /// 数据库池
        /// </summary>
        protected readonly IDatabasePool databasePool;

        /// <summary>
        /// 从数据库池
        /// </summary>
        protected readonly ISlaveDatabasePool slaveDatabasePool;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="databasePool">数据库池</param>
        /// <param name="slaveDatabasePool">从数据库池</param>
        public DatabasePoolServiceBase(IDatabasePool databasePool = null, ISlaveDatabasePool slaveDatabasePool = null)
        {
            this.databasePool = databasePool;
            this.slaveDatabasePool = slaveDatabasePool;
        }

        /// <summary>
        /// 获取数据库池
        /// 如果输入是从，且没找到从，自动返回主
        /// </summary>
        /// <param name="mode">访问模式</param>
        /// <returns>数据库池</returns>
        public virtual IDatabasePool GetDatabasePool(AccessMode mode = AccessMode.MASTER)
        {
            return mode == AccessMode.MASTER
                || databasePool == null
                || !slaveDatabasePool.ExistsSlave
                ? databasePool : slaveDatabasePool;
        }
    }
}
