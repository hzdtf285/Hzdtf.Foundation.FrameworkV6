﻿using Hzdtf.Persistence.Contract.Management;
using Hzdtf.Workflow.Model;
using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using Hzdtf.Utility.Enums;

namespace Hzdtf.Workflow.MySql
{
    /// <summary>
    /// 送件流程路线持久化
    /// @ 黄振东
    /// </summary>
    public partial class SendFlowRoutePersistence
    {
        /// <summary>
        /// 根据流程关卡ID查询送件流程路线列表
        /// </summary>
        /// <param name="flowCensorshipId">流程关卡ID</param>
        /// <param name="connectionId">连接ID</param>
        /// <returns>送件流程路线列表</returns>
        public IList<SendFlowRouteInfo> SelectByFlowCensorshipId(int flowCensorshipId, string connectionId = null)
        {
            IList<SendFlowRouteInfo> result = null;
            DbConnectionManager.BrainpowerExecute(connectionId, this, (connId, dbConn) =>
            {
                string sql = $"{BasicSelectSql()} WHERE {GetFieldByProp("FlowCensorshipId")}=@FlowCensorshipId";
                log.TraceAsync(sql, source: this.GetType().Name, tags: "SelectByFlowCensorshipId");
                result = dbConn.Query<SendFlowRouteInfo>(sql, new { FlowCensorshipId = flowCensorshipId }).AsList();
            }, AccessMode.SLAVE);

            return result;
        }

        /// <summary>
        /// 根据流程关卡ID数组查询送件流程路线列表
        /// </summary>
        /// <param name="flowCensorshipIds">流程关卡ID数组</param>
        /// <param name="connectionId">连接ID</param>
        /// <returns>送件流程路线列表</returns>
        public IList<SendFlowRouteInfo> SelectByFlowCensorshipIds(int[] flowCensorshipIds, string connectionId = null)
        {
            IList<SendFlowRouteInfo> result = null;
            DynamicParameters parameters;
            string idSql = GetWhereIdsSql(flowCensorshipIds, out parameters, null, GetFieldByProp("FlowCensorshipId"));
            DbConnectionManager.BrainpowerExecute(connectionId, this, (connId, dbConn) =>
            {
                string sql = $"{BasicSelectSql()} WHERE {idSql}";
                log.TraceAsync(sql, source: this.GetType().Name, tags: "SelectByFlowCensorshipIds");
                result = dbConn.Query<SendFlowRouteInfo>(sql, parameters).AsList();
            }, AccessMode.SLAVE);

            return result;
        }
    }
}
