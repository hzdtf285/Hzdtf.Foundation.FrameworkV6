﻿﻿using Hzdtf.BasicFunction.Model;
using Hzdtf.MySql;
using Hzdtf.Utility.Attr;
using System;
using Hzdtf.BasicFunction.Persistence.Contract;
using Hzdtf.Persistence.Contract.PermissionFilter;
using Hzdtf.Utility.Localization;
using Hzdtf.Utility.Model.Identitys;
using Hzdtf.Logger.Contract;
using Hzdtf.Persistence.Contract.Basic;

namespace Hzdtf.BasicFunction.MySql
{
    /// <summary>
    /// 角色持久化
    /// @ 黄振东
    /// </summary>
    [Inject]
    public partial class RolePersistence : MySqlDapperBase<int, RoleInfo>, IRolePersistence
    {
        /// <summary>
        /// 表名
        /// </summary>
        public override string Table => "role";

        /// <summary>
        /// 插入字段名称集合
        /// </summary>
        private readonly static string[] INSERT_FIELD_NAMES = new string[]
        {
            "id",
            "system_inlay",
            "system_hide",
            "memo",
            "name",
            "creater_id",
            "creater",
            "create_time",
            "modifier_id",
            "modifier",
            "modify_time",
            "code",
        };

        /// <summary>
        /// 更新字段名称集合
        /// </summary>
        private readonly static string[] UPDATE_FIELD_NAMES = new string[]
        {
            "system_inlay",
            "system_hide",
            "memo",
            "name",
            "modifier_id",
            "modifier",
            "modify_time",
            "code",
        };

        /// <summary>
        /// 所有字段映射集合
        /// </summary>
        private readonly static string[] FIELD_MAP_PROPS = new string[]
        {
            "id Id",
            "system_inlay SystemInlay",
            "system_hide SystemHide",
            "memo Memo",
            "name Name",
            "creater_id CreaterId",
            "creater Creater",
            "create_time CreateTime",
            "modifier_id ModifierId",
            "modifier Modifier",
            "modify_time ModifyTime",
            "code Code",
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
        public RolePersistence(IDefaultConnectionString defaultConnectionString = null, ILogable log = null, IIdentity<int> identity = null, ILocalization localize = null, IDataPermissionFilter dataPermissionFilter = null, IFieldPermissionFilter fieldPermissionFilter = null)
            : base(defaultConnectionString, log, identity, localize, dataPermissionFilter, fieldPermissionFilter)
        {
        }

        /// <summary>
        /// 根据字段名获取模型的值
        /// </summary>
        /// <param name="model">模型</param>
        /// <param name="field">字段名</param>
        /// <returns>值</returns>
        protected override object GetValueByFieldName(RoleInfo model, string field)
        {
            switch (field)
            {
﻿                case "id":
                    return model.Id;

﻿                case "system_inlay":
                    return model.SystemInlay;

﻿                case "system_hide":
                    return model.SystemHide;

﻿                case "memo":
                    return model.Memo;

﻿                case "name":
                    return model.Name;

﻿                case "creater_id":
                    return model.CreaterId;

﻿                case "creater":
                    return model.Creater;

﻿                case "create_time":
                    return model.CreateTime;

﻿                case "modifier_id":
                    return model.ModifierId;

﻿                case "modifier":
                    return model.Modifier;

﻿                case "modify_time":
                    return model.ModifyTime;

﻿                case "code":
                    return model.Code;

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
