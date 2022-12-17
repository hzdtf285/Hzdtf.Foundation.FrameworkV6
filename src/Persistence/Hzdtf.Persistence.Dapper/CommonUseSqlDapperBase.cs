using Dapper;
using Hzdtf.Logger.Contract;
using Hzdtf.Persistence.Contract.Basic;
using Hzdtf.Persistence.Contract.PermissionFilter;
using Hzdtf.Utility.Enums;
using Hzdtf.Utility.Localization;
using Hzdtf.Utility.Model;
using Hzdtf.Utility.Model.Identitys;
using Hzdtf.Utility.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Hzdtf.Persistence.Dapper
{
    /// <summary>
    /// 通用Sql Dapper基类
    /// @ 黄振东
    /// </summary>
    /// <typeparam name="IdT">ID类型</typeparam>
    /// <typeparam name="ModelT">模型类型</typeparam>
    public abstract partial class CommonUseSqlDapperBase<IdT, ModelT> : DapperPersistenceBase<IdT, ModelT> 
        where ModelT : SimpleInfo<IdT>
    {
        #region 属性与字段

        /// <summary>
        /// 转义符前辍
        /// </summary>
        protected abstract string PfxEscapeChar { get; }

        /// <summary>
        /// 转义符后辍
        /// </summary>
        protected abstract string SufxEscapeChar { get; }

        /// <summary>
        /// 带ID等于参数的条件SQL（包含前面的WHERE）
        /// </summary>
        protected string WHERE_ID_EQUAL_PARAM_SQL
        {
            get => $"WHERE {PfxEscapeChar}{IdFieldName}{SufxEscapeChar}=@{GetPropByField(IdFieldName)}";
        }

        /// <summary>
        /// 带ID等于参数的条件SQL（不包含前面的WHERE）
        /// </summary>
        protected string ID_EQUAL_PARAM_SQL
        {
            get => $"{PfxEscapeChar}{IdFieldName}{SufxEscapeChar}=@{GetPropByField(IdFieldName)}";
        }

        /// <summary>
        /// 包装加上前后辍表
        /// </summary>
        protected string WrapPfxSufxTable { get => $"{PfxEscapeChar}{Table}{SufxEscapeChar}"; }

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
        public CommonUseSqlDapperBase(IDefaultConnectionString defaultConnectionString = null, ILogable log = null, IIdentity<IdT> identity = null, ILocalization localize = null, IDataPermissionFilter dataPermissionFilter = null, IFieldPermissionFilter fieldPermissionFilter = null)
            : base(defaultConnectionString, log, identity, localize, dataPermissionFilter, fieldPermissionFilter)
        {
        }

        #endregion

        #region 重写父类的方法

        #region 读取方法

        /// <summary>
        /// 根据ID查询模型SQL语句
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="dataPermissionSql">数据权限SQL</param>
        /// <param name="fieldPermissionSql">字段权限SQL</param>
        /// <param name="propertyNames">属性名称集合</param>
        /// <param name="isPropertyGetNot">属性是否取反，如果取反，则propertyNames则为排除的属性名称集合</param>
        /// <param name="comData">通用数据</param>
        /// <returns>SQL语句</returns>
        protected override string SelectSql(IdT id, string dataPermissionSql, string fieldPermissionSql, string[] propertyNames = null, bool isPropertyGetNot = false, CommonUseData comData = null)
        {
            if (!string.IsNullOrWhiteSpace(dataPermissionSql))
            {
                dataPermissionSql = $" AND ({dataPermissionSql})";
            }

            string basicSelectSql = null;
            if (!string.IsNullOrWhiteSpace(fieldPermissionSql) && propertyNames.IsNullOrLength0())
            {
                basicSelectSql = $"SELECT {fieldPermissionSql} FROM {GetSelectTableName(alias: Table)}";
            }
            else
            {
                basicSelectSql = BasicSelectSql(propertyNames: propertyNames, isPropertyGetNot: isPropertyGetNot, comData: comData);
            }

            var merchantIdFilterSql = SelectIsAppendMerchantId() ? GetMerchantIdFilterSql2(isAfterAppAnd: true, comData: comData) : null;
            return $"{basicSelectSql} WHERE {merchantIdFilterSql} {ID_EQUAL_PARAM_SQL} {dataPermissionSql} ";
        }

        /// <summary>
        /// 根据ID集合查询模型列表SQL语句
        /// </summary>
        /// <param name="ids">ID集合</param>
        /// <param name="dataPermissionSql">数据权限SQL</param>
        /// <param name="fieldPermissionSql">字段权限SQL</param>
        /// <param name="parameters">参数</param>
        /// <param name="propertyNames">属性名称集合</param>
        /// <param name="isPropertyGetNot">属性是否取反，如果取反，则propertyNames则为排除的属性名称集合</param>
        /// <param name="comData">通用数据</param>
        /// <returns>SQL语句</returns>
        protected override string SelectSql(IdT[] ids, string dataPermissionSql, string fieldPermissionSql, out DynamicParameters parameters, string[] propertyNames = null, bool isPropertyGetNot = false, CommonUseData comData = null)
        {
            if (!string.IsNullOrWhiteSpace(dataPermissionSql))
            {
                dataPermissionSql = $" AND ({dataPermissionSql})";
            }

            string basicSelectSql = null;
            if (!string.IsNullOrWhiteSpace(fieldPermissionSql) && propertyNames.IsNullOrLength0())
            {
                basicSelectSql = $"SELECT {fieldPermissionSql} FROM {GetSelectTableName(alias: Table)}";
            }
            else
            {
                basicSelectSql = BasicSelectSql(propertyNames: propertyNames, isPropertyGetNot: isPropertyGetNot, comData: comData);
            }

            var merchantIdFilterSql = SelectIsAppendMerchantId() ? GetMerchantIdFilterSql2(isAfterAppAnd: true, comData: comData) : null;
            return $"{basicSelectSql} WHERE {merchantIdFilterSql} {GetWhereIdsSql(ids, out parameters, comData: comData)} {dataPermissionSql}";
        }

        /// <summary>
        /// 根据ID统计模型数SQL语句
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="dataPermissionSql">数据权限SQL</param>
        /// <param name="comData">通用数据</param>
        /// <returns>SQL语句</returns>
        protected override string CountSql(IdT id, string dataPermissionSql, CommonUseData comData = null)
        {
            if (!string.IsNullOrWhiteSpace(dataPermissionSql))
            {
                dataPermissionSql = $" AND ({dataPermissionSql})";
            }

            var merchantIdFilterSql = SelectIsAppendMerchantId() ? GetMerchantIdFilterSql2(isAfterAppAnd: true, comData: comData) : null;
            return $"{BasicCountSql(comData: comData)} WHERE {merchantIdFilterSql} {ID_EQUAL_PARAM_SQL} {dataPermissionSql}";
        }

        /// <summary>
        /// 统计模型数SQL语句
        /// </summary>
        /// <param name="pfx">前辍</param>
        /// <param name="dataPermissionSql">数据权限SQL</param>
        /// <param name="comData">通用数据</param>
        /// <returns>SQL语句</returns>
        protected override string CountSql(string pfx = null, string dataPermissionSql = null, CommonUseData comData = null)
        {
            var whereSql = new StringBuilder($" WHERE {EqualWhereSql()}");
            string tbAlias = string.IsNullOrWhiteSpace(pfx) ? null : pfx.Replace(".", null);
            if (SelectIsAppendMerchantId())
            {
                var merchantIdSql = GetMerchantIdFilterSql2(isAfterAppAnd: false, pfx: tbAlias, comData: comData);
                if (!string.IsNullOrWhiteSpace(merchantIdSql))
                {
                    whereSql.Append(" AND " + merchantIdSql);
                }
            }

            if (!string.IsNullOrWhiteSpace(dataPermissionSql))
            {
                whereSql.AppendFormat(" AND ({0})", dataPermissionSql);
            }

            return $"{BasicCountSql(pfx, comData: comData)} {whereSql.ToString()}";
        }

        /// <summary>
        /// 基本统计模型数SQL语句
        /// </summary>
        /// <param name="pfx">前辍</param>
        /// <param name="comData">通用数据</param>
        /// <returns>SQL语句</returns>
        protected string BasicCountSql(string pfx = null, CommonUseData comData = null)
        {
            string tbAlias = string.IsNullOrWhiteSpace(pfx) ? null : pfx.Replace(".", null);

            return $"SELECT COUNT(*) FROM {GetSelectTableName(alias: tbAlias)}";
        }

        /// <summary>
        /// 查询模型列表SQL语句
        /// </summary>
        /// <param name="pfx">前辍</param>
        /// <param name="appendFieldSqls">追加字段SQL，包含前面的,</param>
        /// <param name="propertyNames">属性名称集合</param>
        /// <param name="isPropertyGetNot">属性是否取反，如果取反，则propertyNames则为排除的属性名称集合</param>
        /// <param name="dataPermissionSql">数据权限SQL</param>
        /// <param name="fieldPermissionSql">字段权限SQL</param>
        /// <param name="comData">通用数据</param>
        /// <returns>SQL语句</returns>
        protected override string SelectSql(string pfx = null, string appendFieldSqls = null, string[] propertyNames = null, bool isPropertyGetNot = false, string dataPermissionSql = null, string fieldPermissionSql = null, CommonUseData comData = null)
        {
            string tbAlias = null;
            if (string.IsNullOrWhiteSpace(pfx))
            {
                pfx = $"{WrapPfxSufxTable}.";
            }
            else
            {
                tbAlias = pfx.Replace(".", null);
            }

            var whereSql = CreateWhereSql();
            if (SelectIsAppendMerchantId())
            {
                var merchantIdSql = GetMerchantIdFilterSql2(isAfterAppAnd: false, pfx: tbAlias, comData: comData);
                if (!string.IsNullOrWhiteSpace(merchantIdSql))
                {
                    whereSql.Append(" AND " + merchantIdSql);
                }
            }
            if (!string.IsNullOrWhiteSpace(dataPermissionSql))
            {
                whereSql.AppendFormat(" AND ({0})", dataPermissionSql);
            }
            string basicSelectSql = null;
            if (!string.IsNullOrWhiteSpace(fieldPermissionSql) && propertyNames.IsNullOrLength0())
            {
                basicSelectSql = $"SELECT {fieldPermissionSql} FROM {GetSelectTableName(alias: tbAlias)}";
            }
            else
            {
                basicSelectSql = BasicSelectSql(pfx, appendFieldSqls, propertyNames, isPropertyGetNot: isPropertyGetNot, comData: comData);
            }

            return $"{basicSelectSql} {whereSql.ToString()}";
        }

        /// <summary>
        /// 基本查询SQL
        /// </summary>
        /// <param name="pfx">前辍</param>
        /// <param name="appendFieldSqls">追加字段SQL，包含前面的,</param>
        /// <param name="propertyNames">属性名称集合</param>
        /// <param name="isPropertyGetNot">属性是否取反，如果取反，则propertyNames则为排除的属性名称集合</param>
        /// <param name="comData">通用数据</param>
        /// <param name="beforeFieldSqls">添加字段前的SQL</param>
        /// <returns>SQL语句</returns>
        protected string BasicSelectSql(string pfx = null, string appendFieldSqls = null, string[] propertyNames = null, bool isPropertyGetNot = false, CommonUseData comData = null, string beforeFieldSqls = null)
        {
            string tbAlias = null;
            if (string.IsNullOrWhiteSpace(pfx))
            {
                pfx = $"{WrapPfxSufxTable}.";
            }
            else
            {
                tbAlias = pfx.Replace(".", null);
            }

            var fieldSql = isPropertyGetNot ? JoinSelectPropMapNotFields(propertyNames, pfx: pfx) : JoinSelectPropMapFields(propertyNames, pfx: pfx);
            return $"SELECT {beforeFieldSqls} {fieldSql}{appendFieldSqls} FROM {GetSelectTableName(alias: tbAlias)}";
        }

        /// <summary>
        /// 查询模型列表并分页SQL语句
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="dataPermissionSql">数据权限SQL</param>
        /// <param name="fieldPermissionSql">字段权限SQL</param>
        /// <param name="parameters">参数</param>
        /// <param name="filter">筛选</param>
        /// <param name="propertyNames">属性名称集合</param>
        /// <param name="isPropertyGetNot">属性是否取反，如果取反，则propertyNames则为排除的属性名称集合</param>
        /// <param name="comData">通用数据</param>
        /// <returns>SQL语句</returns>
        protected override string SelectPageSql(int pageIndex, int pageSize, string dataPermissionSql, string fieldPermissionSql, out DynamicParameters parameters, FilterInfo filter = null, string[] propertyNames = null, bool isPropertyGetNot = false, CommonUseData comData = null)
        {
            StringBuilder whereSql = CreateWhereSql();
            if (SelectIsAppendMerchantId())
            {
                var merchantIdFilterSql = GetMerchantIdFilterSql2(isAfterAppAnd: false, comData: comData);
                if (!string.IsNullOrWhiteSpace(merchantIdFilterSql))
                {
                    whereSql.Append(" AND " + merchantIdFilterSql);
                }
            }
            MergeWhereSql(whereSql, filter, out parameters, comData: comData);
            if (!string.IsNullOrWhiteSpace(dataPermissionSql))
            {
                whereSql.AppendFormat(" AND ({0})", dataPermissionSql);
            }

            string sortSql = GetSelectPageSortSql(filter, GetSelectSortNamePfx(filter, comData: comData), comData);
            if (string.IsNullOrWhiteSpace(sortSql))
            {
                sortSql = DefaultPageSortSql();
            }

            string basicSelectSql = null;
            if (!string.IsNullOrWhiteSpace(fieldPermissionSql) && propertyNames.IsNullOrLength0())
            {
                basicSelectSql = $"SELECT {fieldPermissionSql} FROM {GetSelectTableName(alias: Table)}";
            }
            else
            {
                basicSelectSql = BasicSelectSql(appendFieldSqls: AppendSelectPageFieldsSql(comData: comData), propertyNames: propertyNames, isPropertyGetNot: isPropertyGetNot, comData: comData);
            }

            return $"{basicSelectSql} " +
                $"{GetSelectPageJoinSql(parameters, filter, comData)} {whereSql.ToString()} {sortSql} {GetPartPageSql(pageIndex, pageSize)}";
        }

        /// <summary>
        /// 组合条件SQL
        /// </summary>
        /// <param name="filter">筛选</param>
        /// <param name="parameters">参数</param>
        /// <param name="comData">通用数据</param>
        /// <returns>条件SQL</returns>
        protected virtual StringBuilder MergeWhereSql(FilterInfo filter, out DynamicParameters parameters, CommonUseData comData = null)
        {
            StringBuilder whereSql = CreateWhereSql();
            MergeWhereSql(whereSql, filter, out parameters, comData);
            return whereSql;
        }

        /// <summary>
        /// 组合条件SQL
        /// </summary>
        /// <param name="whereSql">条件SQL</param>
        /// <param name="filter">筛选</param>
        /// <param name="parameters">参数</param>
        /// <param name="comData">通用数据</param>
        protected virtual void MergeWhereSql(StringBuilder whereSql, FilterInfo filter, out DynamicParameters parameters, CommonUseData comData = null)
        {
            parameters = new DynamicParameters();
            if (filter == null)
            {
                return;
            }

            AppendCreateTimeSql(whereSql, filter, parameters, comData: comData);
            AppendKeywordSql(whereSql, filter as KeywordFilterInfo, comData: comData);
            AppendSelectPageWhereSql(whereSql, parameters, filter, comData: comData);
        }

        /// <summary>
        /// 追加创建时间SQL
        /// </summary>
        /// <param name="whereSql">条件SQL</param>
        /// <param name="filter">筛选</param>
        /// <param name="parameters">参数</param>
        /// <param name="comData">通用数据</param>
        protected virtual void AppendCreateTimeSql(StringBuilder whereSql, FilterInfo filter, DynamicParameters parameters, CommonUseData comData = null)
        {
            if (filter == null)
            {
                return;
            }

            string createTimeField = GetFieldByProp("CreateTime");
            if (filter.StartCreateTime != null)
            {
                parameters.Add("@StartCreateTime", filter.StartCreateTime);

                whereSql.AppendFormat(" AND {2}{0}{3}.{1}>=@StartCreateTime", Table, createTimeField, PfxEscapeChar, SufxEscapeChar);
            }
            if (filter.EndCreateTime != null)
            {
                parameters.Add("@EndCreateTime", filter.EndCreateTime.ToLessThanDate());

                whereSql.AppendFormat(" AND {2}{0}{3}.{1}<@EndCreateTime", Table, createTimeField, PfxEscapeChar, SufxEscapeChar);
            }
        }

        /// <summary>
        /// 追加按关键字查询的SQL
        /// </summary>
        /// <param name="whereSql">条件SQL</param>
        /// <param name="keywordFilter">关键字筛选</param>
        /// <param name="comData">通用数据</param>
        protected virtual void AppendKeywordSql(StringBuilder whereSql, KeywordFilterInfo keywordFilter, CommonUseData comData = null)
        {
            if (keywordFilter == null || string.IsNullOrWhiteSpace(keywordFilter.Keyword))
            {
                return;
            }

            string[] keywordFields = GetPageKeywordFields();
            if (!keywordFields.IsNullOrLength0())
            {
                whereSql.Append(" AND (");
                foreach (var f in keywordFields)
                {
                    string pfx = f.Contains(".") ? null : Table + ".";
                    whereSql.AppendFormat("{0}{1} {2} OR ", pfx, f, keywordFilter.Keyword.GetLikeSql(GetLikeMode()));
                }
                whereSql.Remove(whereSql.Length - 4, 4);
                whereSql.Append(")");
            }
        }

        /// <summary>
        /// 获取分页按关键字查询的字段集合
        /// </summary>
        /// <returns>分页按关键字查询的字段集合</returns>
        protected virtual string[] GetPageKeywordFields() => null;

        /// <summary>
        /// 根据筛选信息统计模型数SQL语句
        /// </summary>
        /// <param name="filter">筛选信息</param>
        /// <param name="dataPermissionSql">数据权限SQL</param>
        /// <param name="parameters">参数</param>
        /// <param name="comData">通用数据</param>
        /// <returns>SQL语句</returns>
        protected override string CountByFilterSql(FilterInfo filter, string dataPermissionSql, out DynamicParameters parameters, CommonUseData comData = null)
        {
            StringBuilder whereSql = CreateWhereSql();
            if (SelectIsAppendMerchantId())
            {
                var merchantIdFilterSql = GetMerchantIdFilterSql2(isAfterAppAnd: false, comData: comData);
                if (!string.IsNullOrWhiteSpace(merchantIdFilterSql))
                {
                    whereSql.Append(" AND " + merchantIdFilterSql);
                }
            }           
            MergeWhereSql(whereSql, filter, out parameters, comData: comData);
            if (!string.IsNullOrWhiteSpace(dataPermissionSql))
            {
                whereSql.AppendFormat(" AND ({0})", dataPermissionSql);
            }
           
            return $"{BasicCountSql(comData: comData)} {GetSelectPageJoinSql(parameters, filter, comData: comData)} {whereSql.ToString()}";
        }

        /// <summary>
        /// 获取商户ID筛选SQL
        /// </summary>
        /// <param name="isBeforeAppWhere">是否前面追加WHERE</param>
        /// <param name="isBeforeAppAnd">是否前面追加AND</param>
        /// <param name="pfx">前辍</param>
        /// <param name="comData">通用数据</param>
        /// <returns>商户ID筛选SQL</returns>
        protected virtual string GetMerchantIdFilterSql(bool isBeforeAppWhere = false, bool isBeforeAppAnd = false, string pfx = null, CommonUseData comData = null)
        {
            IdT merchantId;
            if (IsExistsMerchantId(out merchantId, comData))
            {
                var merchantIdField = GetFieldByProp("MerchantId");
                pfx = string.IsNullOrWhiteSpace(pfx) ? null : pfx + ".";
                var sql = $"({pfx}{PfxEscapeChar}{merchantIdField}{SufxEscapeChar}={identity.GetValueSql(merchantId)})";
                if (isBeforeAppWhere)
                {
                    return $" WHERE ({sql})";
                }
                else if (isBeforeAppAnd)
                {
                    return $" AND ({sql})";
                }

                return sql;
            }

            return null;
        }

        /// <summary>
        /// 获取商户ID筛选SQL2
        /// </summary>
        /// <param name="isBeforeAppWhere">是否前面追加WHERE</param>
        /// <param name="isAfterAppAnd">是否后面追加AND</param>
        /// <param name="pfx">前辍</param>
        /// <param name="comData">通用数据</param>
        /// <returns>商户ID筛选SQL</returns>
        protected virtual string GetMerchantIdFilterSql2(bool isBeforeAppWhere = false, bool isAfterAppAnd = false, string pfx = null, CommonUseData comData = null)
        {
            IdT merchantId;
            if (IsExistsMerchantId(out merchantId, comData))
            {
                var merchantIdField = GetFieldByProp("MerchantId");
                pfx = string.IsNullOrWhiteSpace(pfx) ? null : pfx + ".";
                var sql = $"({pfx}{PfxEscapeChar}{merchantIdField}{SufxEscapeChar}={identity.GetValueSql(merchantId)})";
                if (isBeforeAppWhere)
                {
                    return $" WHERE ({sql})";
                }
                else if (isAfterAppAnd)
                {
                    return $" ({sql}) AND ";
                }

                return sql;
            }

            return null;
        }

        /// <summary>
        /// 默认分页排序SQL，默认是按修改时间降序、创建时间降序、ID升序。如果要改变，请在子类重写
        /// </summary>
        /// <param name="pfx">前辍</param>
        /// <returns>默认排序SQL</returns>
        public virtual string DefaultPageSortSql(string pfx = null)
        {
            if (string.IsNullOrWhiteSpace(pfx))
            {
                pfx = Table;
            }
            else
            {
                pfx = pfx.Replace(".", null);
            }

            return $" ORDER BY {pfx}.{PfxEscapeChar}{GetFieldByProp("ModifyTime")}{SufxEscapeChar} DESC, {pfx}.{PfxEscapeChar}{GetFieldByProp("CreateTime")}{SufxEscapeChar} DESC, {pfx}.{PfxEscapeChar}{GetFieldByProp(IdFieldName)}{SufxEscapeChar}";
        }

        /// <summary>
        /// 根据ID和大于修改时间查询修改信息（多用于乐观锁的判断，以修改时间为判断）
        /// </summary>
        /// <param name="model">模型</param>
        /// <param name="comData">通用数据</param>
        /// <returns>只有修改信息的SQL语句</returns>
        protected override string SelectModifyInfoByIdAndGeModifyTimeSql(ModelT model, CommonUseData comData = null)
        {
            var merchantIdFilterSql = SelectIsAppendMerchantId() ? GetMerchantIdFilterSql2(isAfterAppAnd: true, comData: comData) : null;

            var modifyTimeField = $"{PfxEscapeChar}{ GetFieldByProp("ModifyTime") }{SufxEscapeChar}";
            var idField = $"{PfxEscapeChar}{ GetFieldByProp(IdFieldName)}{SufxEscapeChar}";
            return $"SELECT {idField} Id,{PfxEscapeChar}{GetFieldByProp("ModifierId")}{SufxEscapeChar} ModifierId,{PfxEscapeChar}{GetFieldByProp("Modifier")}{SufxEscapeChar} Modifier,{modifyTimeField} ModifyTime"
                + $" FROM {GetSelectTableName(alias: Table)} WHERE  {merchantIdFilterSql} {idField}=@{GetPropByField(IdFieldName)} AND {modifyTimeField}>@ModifyTime";
        }

        /// <summary>
        /// 根据ID和大于修改时间查询修改信息列表（多用于乐观锁的判断，以修改时间为判断）
        /// </summary>
        /// <param name="models">模型数组</param>
        /// <param name="parameters">参数</param>
        /// <param name="comData">通用数据</param>
        /// <returns>只有修改信息的SQL语句</returns>
        protected override string SelectModifyInfosByIdAndGeModifyTimeSql(ModelT[] models, out DynamicParameters parameters, CommonUseData comData = null)
        {
            parameters = new DynamicParameters();
            var modifyTimeField = $"{PfxEscapeChar}{ GetFieldByProp("ModifyTime") }{SufxEscapeChar}";
            var idField = $"{PfxEscapeChar}{ GetFieldByProp(IdFieldName)}{SufxEscapeChar}";

            var whereSql = CreateWhereSql(true);
            if (SelectIsAppendMerchantId())
            {
                var merchantIdSql = GetMerchantIdFilterSql2(isAfterAppAnd: false, comData: comData);
                if (!string.IsNullOrWhiteSpace(merchantIdSql))
                {
                    whereSql.Append(" AND " + merchantIdSql);
                }
            }
            whereSql.Append("(");

            for (var i = 0; i < models.Length; i++)
            {
                var person = models[i] as PersonTimeInfo<IdT>;

                var paraIdName = $"@{GetPropByField(IdFieldName)}{i}";
                var paraModifyTimeName = $"@ModifyTime{i}";
                parameters.Add(paraIdName, person.Id);
                parameters.Add(paraModifyTimeName, person.ModifyDateTime);

                whereSql.AppendFormat(" ({0}={1} AND {2}>{3}) OR", idField, paraIdName, modifyTimeField, paraModifyTimeName);
            }
            whereSql.Remove(whereSql.Length - 3, 3);
            whereSql.Append(")");

            return $"SELECT {idField} Id,{PfxEscapeChar}{GetFieldByProp("ModifierId")}{SufxEscapeChar} ModifierId,{PfxEscapeChar}{GetFieldByProp("Modifier")}{SufxEscapeChar} Modifier,{modifyTimeField} ModifyTime"
                + $" FROM {GetSelectTableName(alias: Table)} {whereSql.ToString()}";
        }

        /// <summary>
        /// 统计模型数SQL
        /// </summary>
        /// <param name="param">参数</param>
        /// <param name="comData">通用数据</param>
        /// <returns>模型数SQL</returns>
        protected override string CountSql(SqlPropInfo param, CommonUseData comData = null)
        {
            var sql = BasicCountSql(comData: comData);
            var filterSql = GetFilterSql(param.FilterSql, param.IsReplaceFilterSql);
            if (string.IsNullOrWhiteSpace(filterSql))
            {
                return sql;
            }
            sql += " WHERE " + filterSql;

            return sql;
        }

        #endregion

        #region 写入方法

        /// <summary>
        /// 插入模型SQL语句
        /// </summary>
        /// <param name="model">模型</param>
        /// <param name="propertyNames">属性名称集合</param>
        /// <param name="isPropertyGetNot">属性是否取反，如果取反，则propertyNames则为排除的属性名称集合</param>
        /// <param name="isGetAutoId">是否获取自增ID</param>
        /// <param name="comData">通用数据</param>
        /// <returns>SQL语句</returns>
        protected override string InsertSql(ModelT model, string[] propertyNames = null, bool isPropertyGetNot = false, bool isGetAutoId = false, CommonUseData comData = null)
        {
            var fieldNames = isPropertyGetNot ? WrapInsertNotFieldNames(model.Id, propertyNames) : WrapInsertFieldNames(model.Id, propertyNames);
            string[] partSql = CombineInsertSqlByFieldNames(fieldNames);
            string sql = $"INSERT INTO {WrapPfxSufxTable}({partSql[0]}) VALUES({partSql[1]})";

            return isGetAutoId ? $"{sql};{GetLastInsertIdSql()}" : sql;
        }

        /// <summary>
        /// 插入模型列表SQL语句
        /// </summary>
        /// <param name="models">模型列表</param>
        /// <param name="para">参数集合</param>
        /// <param name="propertyNames">属性名称集合</param>
        /// <param name="isPropertyGetNot">属性是否取反，如果取反，则propertyNames则为排除的属性名称集合</param>
        /// <param name="comData">通用数据</param>
        /// <returns>SQL语句</returns>
        protected override string InsertSql(IList<ModelT> models, out DynamicParameters para, string[] propertyNames = null, bool isPropertyGetNot = false, CommonUseData comData = null)
        {
            var fieldNames = isPropertyGetNot ? WrapInsertNotFieldNames(models[0].Id, propertyNames) : WrapInsertFieldNames(models[0].Id, propertyNames);
            string[] partSql = CombineBatchInsertSqlByFieldNames(fieldNames, models, out para);
            return $"INSERT INTO {WrapPfxSufxTable}({partSql[0]}) VALUES{partSql[1]}";
        }

        /// <summary>
        /// 根据ID更新模型SQL语句
        /// </summary>
        /// <param name="model">模型</param>
        /// <param name="propertyNames">属性名称集合</param>
        /// <param name="isPropertyGetNot">属性是否取反，如果取反，则propertyNames则为排除的属性名称集合</param>
        /// <param name="comData">通用数据</param>
        /// <returns>SQL语句</returns>
        protected override string UpdateByIdSql(ModelT model, string[] propertyNames = null, bool isPropertyGetNot = false, CommonUseData comData = null)
        {
            var fieldSql = isPropertyGetNot ? GetUpdateNotFieldFieldsSql(propertyNames) : GetUpdateFieldsSql(propertyNames);
            return $"UPDATE {WrapPfxSufxTable} SET {fieldSql} WHERE {GetMerchantIdFilterSql2(isAfterAppAnd: true, comData: comData)} {ID_EQUAL_PARAM_SQL}";
        }

        /// <summary>
        /// 根据ID删除模型SQL语句
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="comData">通用数据</param>
        /// <returns>SQL语句</returns>
        protected override string DeleteByIdSql(IdT id, CommonUseData comData = null) => $"{BasicDeleteSql(comData: comData)} WHERE {GetMerchantIdFilterSql2(isAfterAppAnd: true, comData: comData)} {ID_EQUAL_PARAM_SQL}";

        /// <summary>
        /// 根据ID数组删除模型SQL语句
        /// </summary>
        /// <param name="ids">ID数组</param>
        /// <param name="parameters">参数集合</param>
        /// <param name="comData">通用数据</param>
        /// <returns>SQL语句</returns>
        protected override string DeleteByIdsSql(IdT[] ids, out DynamicParameters parameters, CommonUseData comData = null) => $"{BasicDeleteSql(comData)} WHERE {GetMerchantIdFilterSql2(isAfterAppAnd: true, comData: comData)} {GetWhereIdsSql(ids, out parameters, comData: comData)}";

        /// <summary>
        /// 删除所有模型SQL语句
        /// </summary>
        /// <param name="comData">通用数据</param>
        /// <returns>SQL语句</returns>
        protected override string DeleteSql(CommonUseData comData = null) => $"DELETE FROM {WrapPfxSufxTable}";

        /// <summary>
        /// 更新模型SQL语句
        /// </summary>
        /// <param name="param">参数</param>
        /// <param name="comData">通用数据</param>
        /// <returns>更新模型SQL</returns>
        protected override string UpdateSql(SqlPropInfo param, CommonUseData comData = null)
        {
            var fieldSql = param.IsPropertyGetNot ? GetUpdateNotFieldFieldsSql(param.PropertyNames) : GetUpdateFieldsSql(param.PropertyNames);
            var sql = $"UPDATE {WrapPfxSufxTable} SET {fieldSql}";
            var filterSql = GetFilterSql(param.FilterSql, param.IsReplaceFilterSql);
            if (string.IsNullOrWhiteSpace(filterSql))
            {
                return sql;
            }
            sql += " WHERE " + filterSql;

            return sql;
        }

        /// <summary>
        /// 删除模型SQL语句
        /// </summary>
        /// <param name="param">参数</param>
        /// <param name="comData">通用数据</param>
        /// <returns>删除模型SQL</returns>
        protected override string DeleteSql(SqlPropInfo param, CommonUseData comData = null)
        {
            var sql = BasicDeleteSql(comData: comData);
            var filterSql = GetFilterSql(param.FilterSql, param.IsReplaceFilterSql);
            if (string.IsNullOrWhiteSpace(filterSql))
            {
                return sql;
            }
            sql += " WHERE " + filterSql;

            return sql;
        }

        /// <summary>
        /// 基本删除所有模型SQL语句
        /// </summary>
        /// <param name="comData">通用数据</param>
        /// <returns>SQL语句</returns>
        protected string BasicDeleteSql(CommonUseData comData = null) => $"DELETE FROM {WrapPfxSufxTable}";

        /// <summary>
        /// 模型是否包含商户ID
        /// </summary>
        /// <returns>模型是否包含商户ID</returns>
        protected override bool ModelContainerMerchantId() => !string.IsNullOrWhiteSpace(GetFieldByProp("MerchantID"));

        /// <summary>
        /// 模型是否已设置商户ID
        /// </summary>
        /// <param name="model">模型</param>
        /// <returns>模型是否已设置商户ID</returns>
        protected override bool ModelIsSetMerchantId(ModelT model)
        {
            if (ModelContainerMerchantId() && model is PersonTimeMerchantInfo<IdT>)
            {
                var m = model as PersonTimeMerchantInfo<IdT>;
                return !identity.IsEmpty(m.MerchantID);
            }

            return false;
        }

        #endregion

        /// <summary>
        /// 根据表名删除所有模型SQL语句
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="comData">通用数据</param>
        /// <returns>SQL语句</returns>
        protected override string DeleteByTableSql(string table, CommonUseData comData = null) => $"DELETE FROM {WrapPfxSufxTable} WHERE {EqualWhereSql()}";

        /// <summary>
        /// 基本根据表名删除所有模型SQL语句
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="comData">通用数据</param>
        /// <returns>SQL语句</returns>
        protected string BasicDeleteByTableSql(string table, CommonUseData comData = null) => $"DELETE FROM {WrapPfxSufxTable}";

        /// <summary>
        /// 根据表名、外键字段和外键值删除模型SQL语句
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="foreignKeyName">外键名称</param>
        /// <param name="foreignKeyValues">外键值集合</param>
        /// <param name="parameters">参数</param>
        /// <param name="comData">通用数据</param>
        /// <returns>SQL语句</returns>
        protected override string DeleteByTableAndForignKeySql(string table, string foreignKeyName, IdT[] foreignKeyValues, out DynamicParameters parameters, CommonUseData comData = null)
        {
            parameters = new DynamicParameters();
            StringBuilder whereSql = CreateWhereSql(true);
            if (SelectIsAppendMerchantId())
            {
                var merchantIdSql = GetMerchantIdFilterSql2(isAfterAppAnd: false, comData: comData);
                if (!string.IsNullOrWhiteSpace(merchantIdSql))
                {
                    whereSql.Append(" AND " + merchantIdSql);
                }
            }
            whereSql.Append($"{PfxEscapeChar}{foreignKeyName}{SufxEscapeChar} IN(");
            for (var i = 0; i < foreignKeyValues.Length; i++)
            {
                string p = $"@{foreignKeyName}{i}";
                whereSql.AppendFormat("{0},", p);
                parameters.Add(p, foreignKeyValues[i]);
            }
            whereSql.Remove(whereSql.Length - 1, 1);
            whereSql.Append(")");

            return $"{BasicDeleteByTableSql(table, comData: comData)} {whereSql.ToString()}";
        }

        /// <summary>
        /// 查询模型分页SQL
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="param">参数</param>
        /// <param name="comData">通用数据</param>
        /// <returns>模型分页SQL</returns>
        protected override string SelectPageSql(int pageIndex, int pageSize, SqlPropInfo param, CommonUseData comData = null)
        {
            var sql = BasicSelectSql(propertyNames: param.PropertyNames, isPropertyGetNot: param.IsPropertyGetNot, comData: comData);
            GetPartPageSql(pageIndex, pageSize);
            var filterSql = GetFilterSql(param.FilterSql, param.IsReplaceFilterSql);
            if (!string.IsNullOrWhiteSpace(filterSql))
            {
                sql += " WHERE " + filterSql;
            }

            var propMapSorts = param.PropMapSortTypes;
            if (IsPageMustSort() && propMapSorts.IsNullOrCount0())
            {
                propMapSorts = new Dictionary<string, SortType>(1)
                {
                    {  IdFieldName, SortType.ASC }
                };
            }
            sql += string.Format(" {0} {1}", GetSortSql(propMapSorts), GetPartPageSql(pageIndex, pageSize));

            return sql;
        }

        #endregion

        #region 需要子类重写的方法

        /// <summary>
        /// 插入字段名集合
        /// </summary>
        /// <returns>插入字段名集合</returns>
        protected abstract string[] InsertFieldNames();

        /// <summary>
        /// 更新字段名称集合
        /// </summary>
        /// <returns>更新字段名称集合</returns>
        protected abstract string[] UpdateFieldNames();

        /// <summary>
        /// 根据字段名获取模型的值
        /// </summary>
        /// <param name="model">模型</param>
        /// <param name="field">字段名</param>
        /// <returns>值</returns>
        protected abstract object GetValueByFieldName(ModelT model, string field);

        /// <summary>
        /// 获取部分的分页SQL语句
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页记录数</param>
        /// <returns>部分的分页SQL语句</returns>
        protected abstract string GetPartPageSql(int pageIndex, int pageSize);

        /// <summary>
        /// 获取最后插入ID SQL语句
        /// </summary>
        /// <returns>最后插入ID SQL语句</returns>
        protected abstract string GetLastInsertIdSql();

        #endregion

        #region 受保护的方法

        /// <summary>
        /// 根据ID数组获取ID条件SQL语句，不包含where
        /// </summary>
        /// <param name="ids">ID数组</param>
        /// <param name="parameters">参数</param>
        /// <param name="prefixTable">表前辍</param>
        /// <param name="idField">ID字段</param>
        /// <param name="comData">通用数据</param>
        /// <returns>ID条件SQL语句</returns>
        protected string GetWhereIdsSql(IdT[] ids, out DynamicParameters parameters, string prefixTable = null, string idField = null, CommonUseData comData = null)
        {
            if (idField == null)
            {
                idField = IdFieldName;
            }
            return GetWhereTypesSql<IdT>(ids, out parameters, idField, prefixTable, comData);
        }

        /// <summary>
        /// 根据值数组获取条件SQL语句，不包含where
        /// </summary>
        /// <param name="values">值数组</param>
        /// <param name="parameters">参数</param>
        /// <param name="field">字段</param>
        /// <param name="prefixTable">表前辍</param>
        /// <param name="comData">通用数据</param>
        /// <returns>ID条件SQL语句</returns>
        protected string GetWhereTypesSql<T>(T[] values, out DynamicParameters parameters, string field, string prefixTable = null, CommonUseData comData = null)
        {
            parameters = new DynamicParameters(values.Length);
            StringBuilder whereSql = new StringBuilder($"{prefixTable}{PfxEscapeChar}{field}{SufxEscapeChar} IN(");
            for (int i = 0; i < values.Length; i++)
            {
                string paraName = $"@{field}{i}";
                whereSql.AppendFormat("{0},", paraName);
                parameters.Add(paraName, values[i]);
            }
            whereSql.Remove(whereSql.Length - 1, 1);
            whereSql.Append(")");

            return whereSql.ToString();
        }

        /// <summary>
        /// 获取排序SQL语句
        /// </summary>
        /// <param name="sort">排序</param>
        /// <param name="prop">排序的属性名</param>
        /// <param name="pfx">前辍</param>
        /// <returns>排序SQL语句</returns>
        protected string GetSortSql(SortType sort, string prop, string pfx = null)
        {
            if (string.IsNullOrWhiteSpace(prop))
            {
                return null;
            }
            if (string.IsNullOrWhiteSpace(pfx))
            {
                pfx = Table + ".";
            }

            string field = GetFieldByProp(prop);
            if (string.IsNullOrWhiteSpace(field))
            {
                field = prop;
            }
            StringBuilder sql = new StringBuilder($"ORDER BY {pfx}{field}");
            if (sort == SortType.ASC)
            {
                sql.Append(" ASC");
            }
            else
            {
                sql.Append(" DESC");
            }

            return sql.ToString();
        }

        /// <summary>
        /// 根据字段名获取排序SQL语句
        /// </summary>
        /// <param name="sort">排序</param>
        /// <param name="field">排序的字段名</param>
        /// <param name="pfx">前辍</param>
        /// <returns>排序SQL语句</returns>
        protected string GetSortSqlByField(SortType sort, string field, string pfx = null)
        {
            if (string.IsNullOrWhiteSpace(field))
            {
                return null;
            }
            if (string.IsNullOrWhiteSpace(pfx))
            {
                pfx = Table + ".";
            }

            StringBuilder sql = new StringBuilder($"ORDER BY {pfx}{field}");
            if (sort == SortType.ASC)
            {
                sql.Append(" ASC");
            }
            else
            {
                sql.Append(" DESC");
            }

            return sql.ToString();
        }

        /// <summary>
        /// 连接查询的属性映射字段集合
        /// 带有,号
        /// </summary>
        /// <param name="props">属性名集合（如果为null则取全部）</param>
        /// <param name="pfx">前辍</param>
        /// <returns>连接后的查询的属性映射字段集合</returns>
        protected string JoinSelectPropMapFields(string[] props = null, string pfx = null)
        {
            StringBuilder result = new StringBuilder();
            if (props == null)
            {
                string[] strs = AllFieldMapProps();
                foreach (string s in strs)
                {
                    result.AppendFormat("{0}{1},", pfx, s);
                }
            }
            else
            {
                foreach (string p in props)
                {
                    result.AppendFormat("{0}{1} {2},", pfx, GetFieldByProp(p), p);
                }
            }
            if (result.Length > 0)
            {
                result.Remove(result.Length - 1, 1);
            }

            return result.ToString();
        }

        /// <summary>
        /// 连接查询的属性映射取反字段集合
        /// 带有,号
        /// </summary>
        /// <param name="props">属性名集合（如果为null则取全部）</param>
        /// <param name="pfx">前辍</param>
        /// <returns>连接后的查询的属性映射取反字段集合</returns>
        protected string JoinSelectPropMapNotFields(string[] props = null, string pfx = null)
        {
            StringBuilder result = new StringBuilder();
            string[] allFieldMapProps = AllFieldMapProps();
            if (!props.IsNullOrLength0())
            {
                var newList = new List<string>(allFieldMapProps.Length - props.Length);
                foreach (var fMapP in allFieldMapProps)
                {
                    var arr = fMapP.Split(' ');
                    if (props.Contains(arr[1]))
                    {
                        continue;
                    }

                    newList.Add(fMapP);
                }
            }

            foreach (string s in allFieldMapProps)
            {
                result.AppendFormat("{0}{1},", pfx, s);
            }
            if (result.Length > 0)
            {
                result.Remove(result.Length - 1, 1);
            }

            return result.ToString();
        }

        /// <summary>
        /// 获取修改信息SQL
        /// 前面带有,前辍
        /// </summary>
        /// <param name="model">模型</param>
        /// <returns>修改信息SQL</returns>
        protected string GetModifyInfoSql(ModelT model)
        {
            if (model is PersonTimeInfo<IdT>)
            {
                string[] modifyProps = new string[] { "ModifierId", "Modifier", "ModifyTime" };
                StringBuilder sql = new StringBuilder();
                foreach (var p in modifyProps)
                {
                    string pName = $"@{p}";
                    sql.AppendFormat(",{2}{0}{3}={1}", GetFieldByProp(p), pName, PfxEscapeChar, SufxEscapeChar);
                }

                return sql.ToString();
            }

            return null;
        }

        #endregion

        #region 虚方法

        /// <summary>
        /// 获取查询分页排序的SQL
        /// </summary>
        /// <param name="filter">筛选信息</param>
        /// <param name="pfx">前辍</param>
        /// <param name="comData">通用数据</param>
        /// <returns>分页排序的SQL</returns>
        protected virtual string GetSelectPageSortSql(FilterInfo filter, string pfx = null, CommonUseData comData = null)
        {
            if (filter == null || string.IsNullOrWhiteSpace(filter.SortName))
            {
                return null;
            }

            return GetSortSql(filter.Sort, ConvertSortName(filter.SortName), pfx);
        }

        /// <summary>
        /// 追加查询分页字段SQL
        /// </summary>
        /// <param name="comData">通用数据</param>
        protected virtual string AppendSelectPageFieldsSql(CommonUseData comData = null) => null;

        /// <summary>
        /// 追加查询分页条件SQL
        /// </summary>
        /// <param name="whereSql">where语句</param>
        /// <param name="parameters">参数</param>
        /// <param name="filter">筛选</param>
        /// <param name="comData">通用数据</param>
        protected virtual void AppendSelectPageWhereSql(StringBuilder whereSql, DynamicParameters parameters, FilterInfo filter = null, CommonUseData comData = null) { }

        /// <summary>
        /// 获取查询分页连接SQL
        /// </summary>
        /// <param name="parameters">参数</param>
        /// <param name="filter">筛选</param>
        /// <param name="comData">通用数据</param>
        /// <returns>连接SQL语句</returns>
        protected virtual string GetSelectPageJoinSql(DynamicParameters parameters, FilterInfo filter = null, CommonUseData comData = null) => null;

        /// <summary>
        /// 转换排序名称
        /// </summary>
        /// <param name="sortName">排名名称</param>
        /// <returns>排序名称</returns>
        protected virtual string ConvertSortName(string sortName) => sortName.FristUpper();

        /// <summary>
        /// 包装插入字段名集合
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="propertyNames">属性名称集合</param>
        /// <returns>插入字段名集合</returns>
        protected virtual string[] WrapInsertFieldNames(IdT id, string[] propertyNames = null)
        {
            string[] fields = null;
            if (propertyNames.IsNullOrLength0())
            {
                fields = InsertFieldNames();
            }
            else
            {
                fields = new string[propertyNames.Length];
                for (var i = 0; i < propertyNames.Length; i++)
                {
                    fields[i] = GetFieldByProp(propertyNames[i]);
                }
            }
            if (PrimaryKeyIncr(id))
            {
                var idField = GetFieldByProp(IdFieldName, AllFieldMapProps());
                return fields.Remove(idField);
            }

            return fields;
        }

        /// <summary>
        /// 包装插入取反字段名集合
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="propertyNames">属性名称集合</param>
        /// <returns>插入字段名集合</returns>
        protected virtual string[] WrapInsertNotFieldNames(IdT id, string[] propertyNames = null)
        {
            var fields = InsertFieldNames();
            if (!propertyNames.IsNullOrLength0())
            {
                var newFields = new List<string>(fields.Length - propertyNames.Length);
                foreach (var f in fields)
                {
                    if (propertyNames.Contains(GetPropByField(f)))
                    {
                        continue;
                    }

                    newFields.Add(f);
                }
                fields = newFields.ToArray();
            }

            if (PrimaryKeyIncr(id))
            {
                var idField = GetFieldByProp(IdFieldName, AllFieldMapProps());
                return fields.Remove(idField);
            }

            return fields;
        }

        /// <summary>
        /// 创建where语句
        /// </summary>
        /// <param name="isAppendAnd">是否在尾加加and</param>
        /// <returns>where语句</returns>
        protected virtual StringBuilder CreateWhereSql(bool isAppendAnd = false)
        {
            var andStr = isAppendAnd ? " AND " : null;
            return new StringBuilder($" WHERE {EqualWhereSql()} {andStr}");
        }

        /// <summary>
        /// 获取Like模式，默认为左匹配
        /// </summary>
        /// <returns>Like模式</returns>
        protected virtual LikeMode GetLikeMode() => LikeMode.LEFT_EQUAL;

        /// <summary>
        /// 获取查询表名
        /// </summary>
        /// <param name="table">表名，为空则默认等于WrapPfxSufxTable</param>
        /// <param name="alias">别名</param>
        /// <returns>查询表名</returns>
        protected virtual string GetSelectTableName(string table = null, string alias = null)
        {
            if (string.IsNullOrWhiteSpace(table))
            {
                table = WrapPfxSufxTable;
            }
            if (IsEnableSelectNolockTable())
            {
                return $"{GetNoLockTableSql(table, alias)}";
            }

            return $"{table} {alias}";
        }

        /// <summary>
        /// 获取过滤SQL
        /// </summary>
        /// <param name="filterSql">过滤SQL</param>
        /// <param name="isReplaceFilterSql">是否替换过滤SQL</param>
        /// <returns>过滤SQL</returns>
        protected virtual string GetFilterSql(string filterSql, bool isReplaceFilterSql)
        {
            if (string.IsNullOrWhiteSpace(filterSql))
            {
                return null;
            }

            if (isReplaceFilterSql)
            {
                var whereSql = filterSql;
                foreach (var fieldMapProp in AllFieldMapProps())
                {
                    var array = fieldMapProp.Split(' ');
                    whereSql = whereSql.Replace(array[1].SqlPackPropName(), string.Format("{0}{1}{2}", PfxEscapeChar, array[0], SufxEscapeChar));
                }

                return whereSql;
            }

            return null;
        }

        /// <summary>
        /// 获取排序SQL
        /// </summary>
        /// <param name="propMapSortTypes">属性映射排序类型字典 key：属性名，value：排序类型</param>
        /// <returns>排序SQL</returns>
        protected virtual string GetSortSql(IDictionary<string, SortType> propMapSortTypes)
        {
            if (propMapSortTypes.IsNullOrCount0())
            {
                return null;
            }

            var sql = new StringBuilder();
            foreach (var item in propMapSortTypes)
            {
                var field = GetFieldByProp(item.Key);
                if (string.IsNullOrWhiteSpace(field))
                {
                    throw new ArgumentNullException($"找不到属性名[{item.Key}]映射的字段名");
                }
                sql.AppendFormat("{0} {1},", GetFieldByProp(item.Key), item.Value.ToString());
            }
            sql.Remove(sql.Length - 1, 1);

            return $"ORDER BY {sql}";
        }

        /// <summary>
        /// 是否分页必须要排序
        /// </summary>
        /// <returns>是否分页必须要排序，默认为否</returns>
        protected virtual bool IsPageMustSort() => false;

        #endregion

        #region 私有方法

        /// <summary>
        /// 根据字段名称集合组合插入SQL语句
        /// </summary>
        /// <param name="fieldNames">字段名称集合</param>
        /// <returns>插入SQL语句</returns>
        private string[] CombineInsertSqlByFieldNames(string[] fieldNames)
        {
            StringBuilder fieldBuilder = new StringBuilder();
            StringBuilder valueBuilder = new StringBuilder();
            string[] fieldMapProps = AllFieldMapProps();
            foreach (string field in fieldNames)
            {
                fieldBuilder.AppendFormat("{1}{0}{2},", field, PfxEscapeChar, SufxEscapeChar);
                valueBuilder.AppendFormat("@{0},", GetPropByField(field));
            }

            fieldBuilder.Remove(fieldBuilder.Length - 1, 1);
            valueBuilder.Remove(valueBuilder.Length - 1, 1);

            return new string[] { fieldBuilder.ToString(), valueBuilder.ToString() };
        }

        /// <summary>
        /// 根据字段名称集合组合批量插入SQL语句
        /// </summary>
        /// <param name="fieldNames">字段名称集合</param>
        /// <param name="models">模型列表</param>
        /// <param name="para">参数</param>
        /// <returns>插入SQL语句</returns>
        private string[] CombineBatchInsertSqlByFieldNames(string[] fieldNames, IList<ModelT> models, out DynamicParameters para)
        {
            para = new DynamicParameters();
            StringBuilder fieldBuilder = new StringBuilder();
            StringBuilder[] valueBuilder = new StringBuilder[models.Count];
            for (int i = 0; i < valueBuilder.Length; i++)
            {
                valueBuilder[i] = new StringBuilder();
            }

            string[] fieldMapProps = AllFieldMapProps();
            foreach (string field in fieldNames)
            {
                fieldBuilder.AppendFormat("{1}{0}{2},", field, PfxEscapeChar, SufxEscapeChar);

                for (int i = 0; i < valueBuilder.Length; i++)
                {
                    string paraName = $"@{GetPropByField(field)}{i}";
                    para.Add(paraName, GetValueByFieldName(models[i], field));
                    valueBuilder[i].AppendFormat("{0},", paraName);
                }
            }
            fieldBuilder.Remove(fieldBuilder.Length - 1, 1);
            StringBuilder valResultSql = new StringBuilder();
            for (int i = 0; i < valueBuilder.Length; i++)
            {
                valueBuilder[i].Remove(valueBuilder[i].Length - 1, 1);
                valResultSql.AppendFormat("({0}),", valueBuilder[i].ToString());
            }
            valResultSql.Remove(valResultSql.Length - 1, 1);

            return new string[] { fieldBuilder.ToString(), valResultSql.ToString() };
        }

        /// <summary>
        /// 根据字段名称集合组合更新SQL语句
        /// </summary>
        /// <param name="fieldNames">字段名称集合</param>
        /// <returns>更新SQL语句</returns>
        private string CompareUpdateSqlByFieldNames(string[] fieldNames)
        {
            StringBuilder fieldValueBuilder = new StringBuilder();
            string[] fieldMapProps = AllFieldMapProps();
            foreach (string field in fieldNames)
            {
                fieldValueBuilder.AppendFormat("{2}{0}{3}=@{1},", field, GetPropByField(field), PfxEscapeChar, SufxEscapeChar);
            }
            fieldValueBuilder.Remove(fieldValueBuilder.Length - 1, 1);

            return fieldValueBuilder.ToString();
        }

        /// <summary>
        /// 获取更新字段SQL
        /// 如果传入的属性名称为null则获取子类的字段
        /// </summary>
        /// <param name="propertyNames">属性名称</param>
        /// <returns>更新字段SQL</returns>
        private string GetUpdateFieldsSql(string[] propertyNames = null)
        {
            string[] fields = null;
            if (propertyNames == null)
            {
                fields = UpdateFieldNames();
            }
            else
            {
                fields = new string[propertyNames.Length];
                for (var i = 0; i < propertyNames.Length; i++)
                {
                    fields[i] = GetFieldByProp(propertyNames[i]);
                    if (string.IsNullOrWhiteSpace(fields[i]))
                    {
                        throw new KeyNotFoundException($"找不到属性名[{propertyNames[i]}]映射的列");
                    }
                }
            }

            return CompareUpdateSqlByFieldNames(fields);
        }

        /// <summary>
        /// 获取更新取反字段SQL
        /// 如果传入的属性名称为null则获取子类的字段
        /// </summary>
        /// <param name="propertyNames">属性名称</param>
        /// <returns>更新字段SQL</returns>
        private string GetUpdateNotFieldFieldsSql(string[] propertyNames = null)
        {
            var fields = UpdateFieldNames();
            if (!propertyNames.IsNullOrLength0())
            {
                var newFields = new List<string>(fields.Length - propertyNames.Length - 1);
                foreach (var f in fields)
                {
                    // 忽略更新主键
                    if (IdFieldName.Equals(f))
                    {
                        continue;
                    }

                    var propName = GetPropByField(f);
                    if (string.IsNullOrWhiteSpace(propName))
                    {
                        throw new KeyNotFoundException($"找不到属性名[{propName}]映射的列");
                    }
                    if (propertyNames.Contains(propName))
                    {
                        continue;
                    }

                    newFields.Add(f);
                }
                fields = newFields.ToArray();
            }

            return CompareUpdateSqlByFieldNames(fields);
        }

        #endregion
    }
}