﻿using Hzdtf.Quartz.Model;
using Hzdtf.MySql;
using Hzdtf.Utility.Attr;
using System;
using Hzdtf.Quartz.Persistence.Contract;
using Hzdtf.Persistence.Contract.PermissionFilter;
using Hzdtf.Utility.Localization;
using Hzdtf.Utility.Model.Identitys;
using Hzdtf.Logger.Contract;
using Hzdtf.Persistence.Contract.Basic;

namespace Hzdtf.Quartz.MySql
{
    /// <summary>
    /// 作业任务持久化
    /// @ 黄振东
    /// </summary>
    [Inject]
    public partial class JobTaskPersistence : MySqlDapperBase<int, JobTaskInfo>, IJobTaskPersistence
    {
        /// <summary>
        /// 表名
        /// </summary>
        public override string Table => "job_task";

        /// <summary>
        /// 插入字段名称集合
        /// </summary>
        private readonly static string[] INSERT_FIELD_NAMES = new string[]
        {
            "create_date_time",
            "id",
            "job_full_class",
            "job_params_json_string",
            "jt_desc",
            "jt_group",
            "jt_name",
            "modify_date_time",
            "successed_remove",
            "trigger_cron",
            "trigger_params_json_string",
        };

        /// <summary>
        /// 更新字段名称集合
        /// </summary>
        private readonly static string[] UPDATE_FIELD_NAMES = new string[]
        {
            "job_full_class",
            "job_params_json_string",
            "jt_desc",
            "jt_group",
            "jt_name",
            "modify_date_time",
            "successed_remove",
            "trigger_cron",
            "trigger_params_json_string",
        };

        /// <summary>
        /// 所有字段映射集合
        /// </summary>
        private readonly static string[] FIELD_MAP_PROPS = new string[]
        {
            "create_date_time CreateDateTime",
            "id Id",
            "job_full_class JobFullClass",
            "job_params_json_string JobParamsJsonString",
            "jt_desc JtDesc",
            "jt_group JtGroup",
            "jt_name JtName",
            "modify_date_time ModifyTime",
            "successed_remove SuccessedRemove",
            "trigger_cron TriggerCron",
            "trigger_params_json_string TriggerParamsJsonString",
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
        public JobTaskPersistence(IDefaultConnectionString defaultConnectionString = null, ILogable log = null, IIdentity<int> identity = null, ILocalization localize = null, IDataPermissionFilter dataPermissionFilter = null, IFieldPermissionFilter fieldPermissionFilter = null)
            : base(defaultConnectionString, log, identity, localize, dataPermissionFilter, fieldPermissionFilter)
        {
        }

        /// <summary>
        /// 根据字段名获取模型的值
        /// </summary>
        /// <param name="model">模型</param>
        /// <param name="field">字段名</param>
        /// <returns>值</returns>
        protected override object GetValueByFieldName(JobTaskInfo model, string field)
        {
            switch (field)
            {
﻿                case "create_date_time":
                    return model.CreateDateTime;

﻿                case "id":
                    return model.Id;

﻿                case "job_full_class":
                    return model.JobFullClass;

﻿                case "job_params_json_string":
                    return model.JobParamsJsonString;

﻿                case "jt_desc":
                    return model.JtDesc;

﻿                case "jt_group":
                    return model.JtGroup;

﻿                case "jt_name":
                    return model.JtName;

﻿                case "modify_date_time":
                    return model.ModifyDateTime;

﻿                case "successed_remove":
                    return model.SuccessedRemove;

﻿                case "trigger_cron":
                    return model.TriggerCron;

﻿                case "trigger_params_json_string":
                    return model.TriggerParamsJsonString;

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
