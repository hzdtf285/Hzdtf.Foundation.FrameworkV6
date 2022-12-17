﻿using Hzdtf.Example.Model;
using Hzdtf.SqlServer;
using Hzdtf.Utility.Attr;
using Hzdtf.Example.Persistence.Contract;
using Hzdtf.Persistence.Contract.PermissionFilter;
using Hzdtf.Utility.Localization;
using Hzdtf.Utility.Model.Identitys;
using System;
using Hzdtf.Persistence.Contract.Basic;
using Hzdtf.Logger.Contract;

namespace Hzdtf.Example.SqlServer
{
    /// <summary>
    /// Test2持久化
    /// @ 黄振东
    /// </summary>
    [Inject]
    public partial class Test2Persistence : SqlServerDapperBase<int, Test2Info>, ITest2Persistence
    {
        /// <summary>
        /// 表名
        /// </summary>
        public override string Table => "Test2";

        /// <summary>
        /// 插入字段名称集合
        /// </summary>
        private readonly static string[] INSERT_FIELD_NAMES = new string[]
        {
            "Id",
            "OrderNo",
        };

        /// <summary>
        /// 更新字段名称集合
        /// </summary>
        private readonly static string[] UPDATE_FIELD_NAMES = new string[]
        {
            "OrderNo",
        };

        /// <summary>
        /// 所有字段映射集合
        /// </summary>
        private readonly static string[] FIELD_MAP_PROPS = new string[]
        {
            "Id Id",
            "OrderNo OrderNo",
        };

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="defaultConnectionString">默认连接字符串，默认取0主库；1从库</param>
        /// <param name="log">日志</param>
        /// <param name="identity">ID</param>
        /// <param name="localize">本地化</param>
        /// <param name="dataPermissionFilter">数据权限过滤</param>
        /// <param name="fieldPermissionFilter">字段权限过滤</param>
        public Test2Persistence(IDefaultConnectionString defaultConnectionString = null, ILogable log = null, IIdentity<int> identity = null, ILocalization localize = null, IDataPermissionFilter dataPermissionFilter = null, IFieldPermissionFilter fieldPermissionFilter = null)
            : base(defaultConnectionString, log, identity, localize, dataPermissionFilter, fieldPermissionFilter)
        {
        }

        /// <summary>
        /// 根据字段名获取模型的值
        /// </summary>
        /// <param name="model">模型</param>
        /// <param name="field">字段名</param>
        /// <returns>值</returns>
        protected override object GetValueByFieldName(Test2Info model, string field)
        {
            switch (field)
            {
﻿                case "Id":
                    return model.Id;

﻿                case "OrderNo":
                    return model.OrderNo;

                default:
                    return null;
            }
        }

        /// <summary>
        /// 插入字段名集合
        /// </summary>
        /// <returns>插入字段名集合</returns>
        protected override string[] InsertFieldNames() => INSERT_FIELD_NAMES;

        /// <summary>
        /// 更新字段名称集合
        /// </summary>
        /// <returns>更新字段名称集合</returns>
        protected override string[] UpdateFieldNames() => UPDATE_FIELD_NAMES;

		/// <summary>
        /// 所有字段映射集合
        /// </summary>
        /// <returns>所有字段映射集合</returns>
        public override string[] AllFieldMapProps() => FIELD_MAP_PROPS;
    }
}
