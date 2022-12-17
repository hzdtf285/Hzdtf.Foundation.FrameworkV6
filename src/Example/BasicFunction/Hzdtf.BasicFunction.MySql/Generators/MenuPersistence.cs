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
    /// 菜单持久化
    /// @ 黄振东
    /// </summary>
    [Inject]
    public partial class MenuPersistence : MySqlDapperBase<int, MenuInfo>, IMenuPersistence
    {
        /// <summary>
        /// 表名
        /// </summary>
        public override string Table => "menu";

        /// <summary>
        /// 插入字段名称集合
        /// </summary>
        private readonly static string[] INSERT_FIELD_NAMES = new string[]
        {
            "id",
            "link",
            "icon",
            "parent_id",
            "sort",
            "code",
            "name",
            "creater_id",
            "creater",
            "create_time",
            "modifier_id",
            "modifier",
            "modify_time",
        };

        /// <summary>
        /// 更新字段名称集合
        /// </summary>
        private readonly static string[] UPDATE_FIELD_NAMES = new string[]
        {
            "link",
            "icon",
            "parent_id",
            "sort",
            "code",
            "name",
            "modifier_id",
            "modifier",
            "modify_time",
        };

        /// <summary>
        /// 所有字段映射集合
        /// </summary>
        private readonly static string[] FIELD_MAP_PROPS = new string[]
        {
            "id Id",
            "link Link",
            "icon Icon",
            "parent_id ParentId",
            "sort Sort",
            "code Code",
            "name Name",
            "creater_id CreaterId",
            "creater Creater",
            "create_time CreateTime",
            "modifier_id ModifierId",
            "modifier Modifier",
            "modify_time ModifyTime",
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
        public MenuPersistence(IDefaultConnectionString defaultConnectionString = null, ILogable log = null, IIdentity<int> identity = null, ILocalization localize = null, IDataPermissionFilter dataPermissionFilter = null, IFieldPermissionFilter fieldPermissionFilter = null)
            : base(defaultConnectionString, log, identity, localize, dataPermissionFilter, fieldPermissionFilter)
        {
        }

        /// <summary>
        /// 根据字段名获取模型的值
        /// </summary>
        /// <param name="model">模型</param>
        /// <param name="field">字段名</param>
        /// <returns>值</returns>
        protected override object GetValueByFieldName(MenuInfo model, string field)
        {
            switch (field)
            {
﻿                case "id":
                    return model.Id;

﻿                case "link":
                    return model.Link;

﻿                case "icon":
                    return model.Icon;

﻿                case "parent_id":
                    return model.ParentId;

﻿                case "sort":
                    return model.Sort;

﻿                case "code":
                    return model.Code;

﻿                case "name":
                    return model.Name;

﻿                case "creater_id":
                    return model.CreaterID;

﻿                case "creater":
                    return model.Creater;

﻿                case "create_time":
                    return model.CreateDateTime;

﻿                case "modifier_id":
                    return model.ModifierID;

﻿                case "modifier":
                    return model.Modifier;

﻿                case "modify_time":
                    return model.ModifyDateTime;

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