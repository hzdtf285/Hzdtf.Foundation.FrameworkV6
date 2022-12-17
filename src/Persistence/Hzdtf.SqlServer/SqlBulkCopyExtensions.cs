using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Hzdtf.Utility.Utils;

namespace Microsoft.Data.SqlClient
{
    /// <summary>
    /// SqlBulkCopy扩展类
    /// @ 黄振东
    /// </summary>
    public static class SqlBulkCopyExtensions
    {
        /// <summary>
        /// 执行批量插入
        /// </summary>
        /// <param name="models">模型列表</param>
        /// <param name="tableName">表名</param>
        /// <param name="dbConnection">数据库连接</param>
        /// <param name="dbTransaction">数据库事务</param>
        /// <param name="callbackBulkCopy">回调BulkCopy</param>
        /// <param name="eachPropertyFunc">循环属性函数</param>
        /// <returns>影响行数</returns>
        public static int ExecSqlBulkCopy<T>(this IList<T> models, string tableName, IDbConnection dbConnection, IDbTransaction dbTransaction = null, Action<SqlBulkCopy> callbackBulkCopy = null, Func<PropertyInfo, bool> eachPropertyFunc = null)
        {
            if (models == null)
            {
                throw new ArgumentNullException("模型列表不能为null");
            }
            if (models.Count == 0)
            {
                return 0;
            }
            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new ArgumentNullException("表名不能为空");
            }

            var tb = models.ToDataTable(tableName, eachPropertyFunc);

            return ExecSqlBulkCopy(tb, dbConnection, dbTransaction, callbackBulkCopy);
        }

        /// <summary>
        /// 执行批量插入
        /// </summary>
        /// <param name="table">表</param>
        /// <param name="dbConnection">数据库连接</param>
        /// <param name="dbTransaction">数据库事务</param>
        /// <param name="callbackBulkCopy">回调BulkCopy</param>
        /// <returns>影响行数</returns>
        public static int ExecSqlBulkCopy(this DataTable table, IDbConnection dbConnection, IDbTransaction dbTransaction = null, Action<SqlBulkCopy> callbackBulkCopy = null)
        {            
            if (table == null)
            {
                throw new ArgumentNullException("表不能为null");
            }
            if (table.Rows.Count == 0)
            {
                return 0;
            }
            if (dbConnection == null)
            {
                throw new ArgumentNullException("数据库连接不能为null");
            }

            var trans = dbTransaction == null ? null : dbTransaction as SqlTransaction;
            using (var bulkCopy = new SqlBulkCopy(dbConnection as SqlConnection, SqlBulkCopyOptions.TableLock, trans))
            {
                bulkCopy.BatchSize = 100000;
                bulkCopy.BulkCopyTimeout = 120;
                bulkCopy.DestinationTableName = table.TableName;

                foreach (var item in table.Columns)
                {
                    if (item is DataColumn)
                    {
                        var col = item as DataColumn;
                        bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                    }
                }
                if (callbackBulkCopy != null)
                {
                    callbackBulkCopy(bulkCopy);
                }

                bulkCopy.WriteToServer(table);
            }

            return table.Rows.Count;
        }
    }
}
