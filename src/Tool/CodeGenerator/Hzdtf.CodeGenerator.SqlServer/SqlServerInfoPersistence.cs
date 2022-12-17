using Hzdtf.CodeGenerator.Model;
using Hzdtf.CodeGenerator.Persistence.Contract;
using System;
using System.Collections.Generic;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using Hzdtf.Utility.Model;
using NPOI.SS.UserModel;
using static NPOI.HSSF.Util.HSSFColor;
using Hzdtf.Utility.Utils;

namespace Hzdtf.CodeGenerator.SqlServer
{
    /// <summary>
    /// Sql Server信息持久化
    /// @ 黄振东
    /// </summary>
    public class SqlServerInfoPersistence : IDbInfoPersistence
    {
        /// <summary>
        /// 查询所有表信息列表
        /// </summary>
        /// <param name="dataBase">数据库</param>
        /// <param name="connectionString">连接字符串</param>
        /// <returns>所有表信息列表</returns>
        public IList<TableInfo> SelectTables(string dataBase, string connectionString)
        {
            string sql = "select t.name Name, p.value Description from sys.tables t"
                        + " left join sys.extended_properties p on p.major_id = t.object_id and minor_id = 0 and p.name = 'MS_Description'";
            IDbConnection dbConnection = new SqlConnection(connectionString);
            try
            {
                IList<TableInfo> tables = dbConnection.Query<TableInfo>(sql).AsList();
                return tables;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            finally
            {
                dbConnection.Close();
                dbConnection.Dispose();
            }
        }

        /// <summary>
        /// 查询所有列信息列表
        /// </summary>
        /// <param name="dataBase">数据库</param>
        /// <param name="connectionString">连接字符串</param>
        /// <param name="table">表</param>
        /// <returns>所有列信息列表</returns>
        public IList<ColumnInfo> SelectColumns(string dataBase, string connectionString, string table)
        {
            string sql = "select t.*, p.value [Description] from"
                        + " ("
                        + " select a.name Name, c.name as DataType,case when a.is_nullable = 0 then 0 else 1 end as [IsNull],a.max_length[Length], a.column_id column_id,a.object_id"
                        + " from sys.columns a , sys.objects b, sys.types c"
                        + " where a.object_id = b.object_id and b.name = @Table and c.name != 'sysname' and a.system_type_id = c.system_type_id"
                        + " ) t"
                        + " left join sys.extended_properties p on p.major_id=t.object_id and p.minor_id = t.column_id and p.name = 'MS_Description'"
                        + " order by t.column_id";
            IDbConnection dbConnection = new SqlConnection(connectionString);
            try
            {
                return dbConnection.Query<ColumnInfo>(sql, new { Table = table }).AsList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            finally
            {
                dbConnection.Close();
                dbConnection.Dispose();
            }
        }

        /// <summary>
        /// 根据表名数组查询主键列列表
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        /// <param name="tables">数组</param>
        /// <returns>主键列列表.key：表名，value：主键列名</returns>
        public IList<KeyValueInfo<string, string>> SelectPrimaryKeyColumnsByTables(string connectionString, params string[] tables)
        {
            string sql = $"SELECT TABLE_NAME [Key],COLUMN_NAME [Value] FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE TABLE_NAME IN({tables.ToMergeString(",", "'")})";
            IDbConnection dbConnection = new SqlConnection(connectionString);
            try
            {
                return dbConnection.Query<KeyValueInfo<string, string>>(sql).AsList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            finally
            {
                dbConnection.Close();
                dbConnection.Dispose();
            }
        }
    }
}
