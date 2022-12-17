using Hzdtf.Logger.Contract;
using Hzdtf.Persistence.Contract.Basic;
using Hzdtf.Persistence.Contract.PermissionFilter;
using Hzdtf.Persistence.Dapper;
using Hzdtf.Utility.Localization;
using Hzdtf.Utility.Model;
using Hzdtf.Utility.Model.Identitys;
using Hzdtf.Utility.Utils;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Hzdtf.MySql
{
    /// <summary>
    /// MySql Dapper基类
    /// @ 黄振东
    /// </summary>
    /// <typeparam name="IdT">ID类型</typeparam>
    /// <typeparam name="ModelT">模型类型</typeparam>
    public abstract partial class MySqlDapperBase<IdT, ModelT> : CommonUseSqlDapperBase<IdT, ModelT>
        where ModelT : SimpleInfo<IdT>
    {
        #region 属性与字段

        /// <summary>
        /// 转义符前辍
        /// </summary>
        protected override string PfxEscapeChar { get => "`"; }

        /// <summary>
        /// 转义符后辍
        /// </summary>
        protected override string SufxEscapeChar { get => "`"; }

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
        public MySqlDapperBase(IDefaultConnectionString defaultConnectionString = null, ILogable log = null, IIdentity<IdT> identity = null, ILocalization localize = null, IDataPermissionFilter dataPermissionFilter = null, IFieldPermissionFilter fieldPermissionFilter = null)
            : base(defaultConnectionString, log, identity, localize, dataPermissionFilter, fieldPermissionFilter)
        {
        }

        #endregion

        #region 重写父类的方法

        /// <summary>
        /// 创建数据库连接
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        /// <returns>数据库连接</returns>
        public override IDbConnection CreateDbConnection(string connectionString) => new MySqlConnection(connectionString);

        /// <summary>
        /// 获取部分的分页SQL语句
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页记录数</param>
        /// <returns>部分的分页SQL语句</returns>
        protected override string GetPartPageSql(int pageIndex, int pageSize)
        {
            int[] page = PagingUtil.PageStartEnd(pageIndex, pageSize);
            return $"LIMIT {page[0]},{pageSize}";
        }

        /// <summary>
        /// 获取最后插入ID SQL语句
        /// </summary>
        /// <returns>最后插入ID SQL语句</returns>
        protected override string GetLastInsertIdSql() => "SELECT LAST_INSERT_ID()";

        /// <summary>
        /// 匹配条件SQL
        /// </summary>
        /// <returns>不匹配条件SQL</returns>
        protected override string EqualWhereSql() => " (true) ";

        /// <summary>
        /// 不匹配条件SQL
        /// </summary>
        /// <returns>不匹配条件SQL</returns>
        protected override string NoEqualWhereSql() => " (false) ";

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
        /// 查询模型列表SQL
        /// </summary>
        /// <param name="param">参数</param>
        /// <param name="comData">通用数据</param>
        /// <returns>模型列表SQL</returns>
        protected override string SelectSql(SqlPropInfo param, CommonUseData comData = null)
        {
            var sql = BasicSelectSql(propertyNames: param.PropertyNames, isPropertyGetNot: param.IsPropertyGetNot, comData: comData);
            var filterSql = GetFilterSql(param.FilterSql, param.IsReplaceFilterSql);
            if (!string.IsNullOrWhiteSpace(filterSql))
            {
                sql += " WHERE " + filterSql;
            }
            sql += string.Format(" {0} ", GetSortSql(param.PropMapSortTypes));
            if (param.Top > 0)
            {
                sql += $" LIMIT {param.Top}";
            }

            return sql;
        }

        #endregion

        #region 受保护方法

        /// <summary>
        /// 判断异常是否主键重复
        /// </summary>
        /// <param name="ex">异常</param>
        /// <returns>异常是否主键重复</returns>
        protected virtual bool IsCommonExceptionPkRepeat(Exception ex)
        {
            MySqlException sqlEx = null;
            if (ex is MySqlException)
            {
                sqlEx = ex as MySqlException;
            }
            else
            {
                var innerEx = ex.GetLastInnerException();
                if (innerEx is MySqlException)
                {
                    sqlEx = innerEx as MySqlException;
                }
            }
            if (sqlEx == null)
            {
                return false;
            }

            var result = sqlEx.Number == 1062;
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
        protected virtual bool OtherIsPkRepeat(MySqlException ex) => true;

        #endregion
    }
}
