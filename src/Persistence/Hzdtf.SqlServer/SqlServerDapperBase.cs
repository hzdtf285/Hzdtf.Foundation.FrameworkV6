using Hzdtf.Logger.Contract;
using Hzdtf.Persistence.Contract.Basic;
using Hzdtf.Persistence.Contract.PermissionFilter;
using Hzdtf.Persistence.Dapper;
using Hzdtf.Utility.Localization;
using Hzdtf.Utility.Model;
using Hzdtf.Utility.Model.Identitys;
using Hzdtf.Utility.Utils;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Hzdtf.SqlServer
{
    /// <summary>
    /// SqlServer Dapper基类
    /// @ 黄振东
    /// </summary>
    /// <typeparam name="IdT">ID类型</typeparam>
    /// <typeparam name="ModelT">模型类型</typeparam>
    public abstract partial class SqlServerDapperBase<IdT, ModelT> : CommonUseSqlDapperBase<IdT, ModelT>
        where ModelT : SimpleInfo<IdT>
    {
        #region 属性与字段

        /// <summary>
        /// 转义符前辍
        /// </summary>
        protected override string PfxEscapeChar { get => "["; }

        /// <summary>
        /// 转义符后辍
        /// </summary>
        protected override string SufxEscapeChar { get => "]"; }

        /// <summary>
        /// 同步插入的字段映射类型字典
        /// </summary>
        private static readonly object syncInsertFieldMapTypes = new object();

        /// <summary>
        /// 插入的字段映射类型字典
        /// </summary>
        private static IDictionary<string, Type> insertFieldMapTypes;

        /// <summary>
        /// 插入的字段映射类型字典
        /// </summary>
        protected IDictionary<string, Type> InsertFieldMapTypes
        {
            get
            {
                if (insertFieldMapTypes == null)
                {
                    var allFields = InsertFieldNames();
                    lock (syncInsertFieldMapTypes)
                    {
                        insertFieldMapTypes = new Dictionary<string, Type>(allFields.Length);
                        var type = typeof(ModelT);
                        foreach (var field in allFields)
                        {
                            var prop = type.GetProperty(GetPropByField(field));
                            insertFieldMapTypes.Add(field, prop.PropertyType);
                        }
                    }
                }

                return insertFieldMapTypes;
            }
        }

        #endregion

        #region 初始化

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="defaultConnectionString">默认连接字符串，默认取0主库；1从库</param>
        /// <param name="log">日志</param>
        /// <param name="identity">ID</param>
        /// <param name="localize">本地化</param>
        /// <param name="dataPermissionFilter">数据权限过滤</param>
        /// <param name="fieldPermissionFilter">字段权限过滤</param>
        public SqlServerDapperBase(IDefaultConnectionString defaultConnectionString = null, ILogable log = null, IIdentity<IdT> identity = null, ILocalization localize = null, IDataPermissionFilter dataPermissionFilter = null, IFieldPermissionFilter fieldPermissionFilter = null)
            : base(defaultConnectionString, log, identity, localize, dataPermissionFilter, fieldPermissionFilter)
        {
        }

        #endregion

        #region 重写父类的方法

        /// <summary>
        /// 插入模型列表
        /// </summary>
        /// <param name="models">模型列表</param>
        /// <param name="dbConnection">数据库连接</param>
        /// <param name="propertyNames">属性名称集合</param>
        /// <param name="isPropertyGetNot">属性是否取反，如果取反，则propertyNames则为排除的属性名称集合</param>
        /// <param name="dbTransaction">数据库事务</param>
        /// <param name="comData">通用数据</param>
        /// <returns>影响行数</returns>
        protected override int Insert(IList<ModelT> models, IDbConnection dbConnection, string[] propertyNames = null, bool isPropertyGetNot = false, IDbTransaction dbTransaction = null, CommonUseData comData = null)
        {
            return ExecRecordSqlLog<int>("SqlBulkCopy", () =>
            {
                if (IsSupportIdempotent)
                {
                    try
                    {
                        return ExecSqlBulkCopy(models, dbConnection, propertyNames, isPropertyGetNot, dbTransaction);
                    }
                    catch (Exception ex)
                    {
                        // 如果主键重复，则忽略异常，实现幂等
                        if (IsExceptionPkRepeat(ex))
                        {
                            var ids = models.Select(p => p.Id).ToArray();
                            log.InfoAsync($"批量插入.发生主键重复异常,幂等操作忽略该异常.主键:{ids.ToMergeString(",")}", ex, this.GetType().Name, comData.GetEventId(), "Insert", "SqlBulkCopy");
                            return models.Count;
                        }

                        throw new Exception(ex.Message, ex);
                    }
                }
                else
                {
                    return ExecSqlBulkCopy(models, dbConnection, propertyNames, isPropertyGetNot, dbTransaction);
                }
            }, comData: comData, "Insert", "SqlBulkCopy");
        }

        /// <summary>
        /// 创建数据库连接
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        /// <returns>数据库连接</returns>
        public override IDbConnection CreateDbConnection(string connectionString) => new SqlConnection(connectionString);

        /// <summary>
        /// 获取部分的分页SQL语句
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页记录数</param>
        /// <returns>部分的分页SQL语句</returns>
        protected override string GetPartPageSql(int pageIndex, int pageSize)
        {
            var start = pageIndex > 0 ? pageIndex * pageSize : pageIndex;
            return $"OFFSET {start} ROWS FETCH NEXT {pageSize} ROWS ONLY";
        }

        /// <summary>
        /// 获取最后插入ID SQL语句
        /// </summary>
        /// <returns>最后插入ID SQL语句</returns>
        protected override string GetLastInsertIdSql() => "SELECT SCOPE_IDENTITY()";

        /// <summary>
        /// 判断异常是否主键重复
        /// </summary>
        /// <param name="ex">异常</param>
        /// <returns>异常是否主键重复</returns>
        protected override bool IsExceptionPkRepeat(Exception ex)
        {
            return IsCommonExceptionPkRepeat(ex);
        }

        /// <summary>
        /// 严格判断异常是否主键重复
        /// </summary>
        /// <param name="ex">异常</param>
        /// <returns>异常是否主键重复</returns>
        public override bool StrictnessIsExceptionPkRepeat(Exception ex)
        {
            return IsCommonExceptionPkRepeat(ex);
        }

        /// <summary>
        /// 获取不锁表的SQL
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="alias">别名</param>
        /// <returns>不锁表的SQL</returns>
        protected override string GetNoLockTableSql(string table, string alias = null) => $"{table} {alias} WITH(NOLOCK)";

        /// <summary>
        /// 查询模型列表SQL
        /// </summary>
        /// <param name="param">参数</param>
        /// <param name="comData">通用数据</param>
        /// <returns>模型列表SQL</returns>
        protected override string SelectSql(SqlPropInfo param, CommonUseData comData = null)
        {
            var topSql = param.Top == 0 ? null : $" TOP {param.Top} ";
            var sql = BasicSelectSql(propertyNames: param.PropertyNames, isPropertyGetNot: param.IsPropertyGetNot, comData: comData, beforeFieldSqls: topSql);
            var filterSql = GetFilterSql(param.FilterSql, param.IsReplaceFilterSql);
            if (!string.IsNullOrWhiteSpace(filterSql))
            {
                sql += " WHERE " + filterSql;
            }
            sql += string.Format(" {0} ", GetSortSql(param.PropMapSortTypes));

            return sql;
        }

        /// <summary>
        /// 是否分页必须要排序
        /// </summary>
        /// <returns>是否分页必须要排序，默认为是</returns>
        protected override bool IsPageMustSort() => true;

        #endregion

        #region 虚方法

        /// <summary>
        /// 执行批量插入
        /// 会拿模型第1条数据作为Id作为默认值
        /// </summary>
        /// <param name="models">模型列表</param>
        /// <param name="dbConnection">数据库连接</param>
        /// <param name="propertyNames">属性名称数组</param>
        /// <param name="isPropertyGetNot">属性是否取反，如果取反，则propertyNames则为排除的属性名称集合</param>
        /// <param name="dbTransaction">数据库事务</param>
        /// <returns>影响行数</returns>
        protected virtual int ExecSqlBulkCopy(IList<ModelT> models, IDbConnection dbConnection, string[] propertyNames = null, bool isPropertyGetNot = false, IDbTransaction dbTransaction = null)
        {
            var defaultId = models[0].Id;
            // 需要插入的字段数组
            var insertFields = isPropertyGetNot ? WrapInsertNotFieldNames(defaultId, propertyNames) : WrapInsertFieldNames(defaultId, propertyNames);

            var table = new DataTable(Table);

            // 循环需要插入的数组，并添加到对应的内存表里
            foreach (var field in insertFields)
            {
                table.Columns.Add(field);
            }
            var entityType = typeof(ModelT);
            // 循环模型列表，添加内存行并设置单元格数据
            foreach (var item in models)
            {
                var row = table.NewRow();
                foreach (var field in insertFields)
                {
                    var value = entityType.GetProperty(GetPropByField(field)).GetValue(item);
                    if (value == null)
                    {
                        continue;
                    }
                    var propType = InsertFieldMapTypes[field];
                    if (propType.IsEnum)
                    {
                        value = (byte)value;
                    }
                    row[field] = value;
                }

                table.Rows.Add(row);
            }

            return table.ExecSqlBulkCopy(dbConnection, dbTransaction, SetSqlBulkCopy);
        }

        /// <summary>
        /// 判断异常是否主键重复
        /// </summary>
        /// <param name="ex">异常</param>
        /// <returns>异常是否主键重复</returns>
        protected virtual bool IsCommonExceptionPkRepeat(Exception ex)
        {
            SqlException sqlEx = null;
            if (ex is SqlException)
            {
                sqlEx = ex as SqlException;
            }
            else
            {
                var innerEx = ex.GetLastInnerException();
                if (innerEx is SqlException)
                {
                    sqlEx = innerEx as SqlException;
                }
            }
            if (sqlEx == null)
            {
                return false;
            }

            var result = sqlEx.Number == 2627;
            if (result)
            {
                return OtherIsPkRepeat(sqlEx);
            }

            return result;
        }

        /// <summary>
        /// 其他判断主键重复，此方法目的是为了异常可能包含其他非主键重复的
        /// 如果子类没重写，默认为是
        /// </summary>
        /// <param name="ex">异常</param>
        /// <returns>其他判断主键重复</returns>
        protected virtual bool OtherIsPkRepeat(SqlException ex) => true;

        /// <summary>
        /// 设置SQL Bulk Copy
        /// </summary>
        /// <param name="bulk">SQL Bulk Copy</param>
        protected virtual void SetSqlBulkCopy(SqlBulkCopy bulk)
        {
        }

        #endregion
    }
}
