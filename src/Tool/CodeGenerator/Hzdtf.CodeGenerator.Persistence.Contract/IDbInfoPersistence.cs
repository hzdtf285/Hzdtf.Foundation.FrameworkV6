﻿using Hzdtf.CodeGenerator.Model;
using Hzdtf.Utility.Model;
using System;
using System.Collections.Generic;

namespace Hzdtf.CodeGenerator.Persistence.Contract
{
    /// <summary>
    /// 数据库信息持久化
    /// @ 黄振东
    /// </summary>
    public interface IDbInfoPersistence
    {
        /// <summary>
        /// 查询所有表信息列表
        /// </summary>
        /// <param name="dataBase">数据库</param>
        /// <param name="connectionString">连接字符串</param>
        /// <returns>所有表信息列表</returns>
        IList<TableInfo> SelectTables(string dataBase, string connectionString);

        /// <summary>
        /// 查询所有列信息列表
        /// </summary>
        /// <param name="dataBase">数据库</param>
        /// <param name="connectionString">连接字符串</param>
        /// <param name="table">表</param>
        /// <returns>所有列信息列表</returns>
        IList<ColumnInfo> SelectColumns(string dataBase, string connectionString, string table);

        /// <summary>
        /// 根据表名数组查询主键列列表
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        /// <param name="tables">数组</param>
        /// <returns>主键列列表.key：表名，value：主键列名</returns>
        IList<KeyValueInfo<string, string>> SelectPrimaryKeyColumnsByTables(string connectionString, params string[] tables);
    }
}
