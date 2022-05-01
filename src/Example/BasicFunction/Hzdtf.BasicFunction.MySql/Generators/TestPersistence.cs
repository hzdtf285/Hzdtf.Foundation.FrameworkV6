﻿using Hzdtf.BasicFunction.Model;
using Hzdtf.MySql;
using Hzdtf.Utility.Attr;
using System;
using Hzdtf.BasicFunction.Persistence.Contract;

namespace Hzdtf.BasicFunction.MySql
{
    /// <summary>
    /// Test持久化
    /// </summary>
    [Inject]
    public partial class TestPersistence : MySqlDapperBase<int, TestInfo>, ITestPersistence
    {
        /// <summary>
        /// 表名
        /// </summary>
        public override string Table => "test";

        /// <summary>
        /// 插入字段名称集合
        /// </summary>
        private readonly static string[] INSERT_FIELD_NAMES = new string[]
        {
            "id",
            "merchant_id",
            "name",
        };

        /// <summary>
        /// 更新字段名称集合
        /// </summary>
        private readonly static string[] UPDATE_FIELD_NAMES = new string[]
        {
            "name",
        };

        /// <summary>
        /// 所有字段映射集合
        /// </summary>
        private readonly static string[] FIELD_MAP_PROPS = new string[]
        {
            "id Id",
            "merchant_id MerchantId",
            "name Name",
        };

        /// <summary>
        /// 根据字段名获取模型的值
        /// </summary>
        /// <param name="model">模型</param>
        /// <param name="field">字段名</param>
        /// <returns>值</returns>
        protected override object GetValueByFieldName(TestInfo model, string field)
        {
            switch (field)
            {
﻿                case "id":
                    return model.Id;

﻿                case "merchant_id":
                    return model.MerchantId;

﻿                case "name":
                    return model.Name;

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
