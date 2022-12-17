using Hzdtf.CodeGenerator.Contract;
using Hzdtf.CodeGenerator.Model;
using Hzdtf.CodeGenerator.Persistence.Contract;
using Hzdtf.Utility.Attr;
using Hzdtf.Utility.Factory;
using Hzdtf.Utility.Model.Return;
using System;
using System.Collections.Generic;
using System.Text;
using Hzdtf.Utility.Utils;
using System.Linq;

namespace Hzdtf.CodeGenerator.Impl.DataSource
{
    /// <summary>
    /// 数据库信息服务
    /// @ 黄振东
    /// </summary>
    [Inject]
    public class DbInfoService : IDbInfoService
    {
        /// <summary>
        /// 数据库信息持久化工厂
        /// </summary>
        public ISimpleFactory<string, IDbInfoPersistence> DbInfoPersistenceFactory
        {
            get;
            set;
        }

        /// <summary>
        /// 查询所有表信息列表
        /// </summary>
        /// <param name="dataBase">数据库</param>
        /// <param name="connectionString">连接字符串</param>
        /// <param name="dataSourceType">数据源类型</param>
        /// <returns>返回信息</returns>
        public ReturnInfo<IList<TableInfo>> Query(string dataBase, string connectionString, string dataSourceType)
        {
            ReturnInfo<IList<TableInfo>> returnInfo = new ReturnInfo<IList<TableInfo>>();
            IDbInfoPersistence persistence = DbInfoPersistenceFactory.Create(dataSourceType);
            IList<TableInfo> tables = persistence.SelectTables(dataBase, connectionString);
            if (tables.IsNullOrCount0())
            {
                return returnInfo;
            }

            var tableNames = tables.Select(p => p.Name).ToArray();
            var tabPks = persistence.SelectPrimaryKeyColumnsByTables(connectionString, tableNames);

            foreach (TableInfo t in tables)
            {
                t.Columns = persistence.SelectColumns(dataBase, connectionString, t.Name);
                if (t.Columns.IsNullOrCount0() || tabPks.IsNullOrCount0())
                {
                    continue;
                }

                var tp = tabPks.FirstOrDefault(p => p.Key == t.Name);
                if (tp == null)
                {
                    continue;
                }

                var c = t.Columns.FirstOrDefault(p => p.Name == tp.Value);
                if (c == null)
                {
                    continue;
                }
                c.IsPrimaryKey = true;
            }
            returnInfo.Data = tables;

            return returnInfo;
        }
    }
}
