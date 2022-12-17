﻿using Hzdtf.BasicFunction.Model;
using Hzdtf.Persistence.Contract.Management;
using Hzdtf.Utility.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using Hzdtf.BasicFunction.Model.Expand.DataDictionaryItem;
using Hzdtf.Utility.Model;

namespace Hzdtf.BasicFunction.MySql
{
    /// <summary>
    /// 数据字典子项扩展持久化
    /// @ 黄振东
    /// </summary>
    public partial class DataDictionaryItemExpandPersistence
    {
        /// <summary>
        /// 根据数据字典子项ID查询数据字典子项扩展列表
        /// </summary>
        /// <param name="dataDictionaryItemId">数据字典子项ID</param>
        /// <param name="connectionId">连接ID</param>
        /// <returns>数据字典子项扩展列表</returns>
        public IList<DataDictionaryItemExpandInfo> SelectByDataDictionaryItemId(int dataDictionaryItemId, string connectionId = null)
        {
            IList<DataDictionaryItemExpandInfo> result = null;
            DbConnectionManager.BrainpowerExecute(connectionId, this, (connId, dbConn) =>
            {
                string sql = $"{BasicSelectSql()} WHERE {GetFieldByProp("DataDictionaryItemId")}=@DataDictionaryItemId";
                log.TraceAsync(sql, source: this.GetType().Name, tags: "SelectByDataDictionaryItemId");
                result = dbConn.Query<DataDictionaryItemExpandInfo>(sql, new { DataDictionaryItemId = dataDictionaryItemId }).AsList();
            }, AccessMode.SLAVE);

            return result;
        }

        /// <summary>
        /// 追加查询分页SQL
        /// </summary>
        /// <param name="whereSql">where语句</param>
        /// <param name="parameters">参数</param>
        /// <param name="filter">筛选</param>
        /// <param name="comData">通用数据</param>
        protected override void AppendSelectPageWhereSql(StringBuilder whereSql, DynamicParameters parameters, FilterInfo filter = null, CommonUseData comData = null)
        {
            if (filter is DataDictionaryItemExpandFilterInfo)
            {
                DataDictionaryItemExpandFilterInfo dataFilter = filter as DataDictionaryItemExpandFilterInfo;
                whereSql.AppendFormat(" AND `{0}`=@DataDictionaryItemId", GetFieldByProp("DataDictionaryItemId"));
                parameters.Add("@DataDictionaryItemId", dataFilter.DataDictionaryItemId);
            }
        }
    }
}
