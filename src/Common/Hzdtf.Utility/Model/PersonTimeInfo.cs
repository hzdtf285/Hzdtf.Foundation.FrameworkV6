using MessagePack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Hzdtf.Utility.Model
{
    /// <summary>
    /// 带有人时间信息
    /// @ 黄振东
    /// </summary>
    /// <typeparam name="IdT">ID类型</typeparam>
    [MessagePackObject]
    public class PersonTimeInfo<IdT> : TimeInfo<IdT>
    {
        /// <summary>
        /// 创建人ID_名称
        /// </summary>
        public const string CreaterID_Name = "CreaterID";

        /// <summary>
        /// 创建人ID
        /// </summary>
        [JsonProperty("createrID")]
        [Display(AutoGenerateField = false)]
        [MessagePack.Key("createrID")]
        public IdT CreaterID
        {
            get;
            set;
        }

        /// <summary>
        /// 创建人ID字符串_名称
        /// </summary>
        public const string CreaterIDString_Name = "CreaterIDString";

        /// <summary>
        /// 创建人ID字符串，如果ID类型为长整型，则在JS前端使用此属性为字符串类型，因为JS中长整型会丢失精度
        /// </summary>
        [JsonProperty("createrIDString")]
        [Display(AutoGenerateField = false)]
        [MessagePack.Key("createrIDString")]
        public string CreaterIDString
        {
            get => CreaterID.ToString();
        }

        /// <summary>
        /// 创建人_名称
        /// </summary>
        public const string Creater_Name = "Creater";

        /// <summary>
        /// 创建人
        /// </summary>
        [JsonProperty("creater")]
        [Display(AutoGenerateField = false)]
        [MessagePack.Key("creater")]
        public string Creater
        {
            get;
            set;
        }

        /// <summary>
        /// 修改人ID_名称
        /// </summary>
        public const string ModifierId_Name = "ModifierID";

        /// <summary>
        /// 修改人ID
        /// </summary>
        [JsonProperty("modifierID")]
        [Display(AutoGenerateField = false)]
        [MessagePack.Key("modifierID")]
        public IdT ModifierID
        {
            get;
            set;
        }

        /// <summary>
        /// 修改人ID字符串_名称
        /// </summary>
        public const string ModifierIDString_Name = "ModifierIDString";

        /// <summary>
        /// 修改人ID字符串，如果ID类型为长整型，则在JS前端使用此属性为字符串类型，因为JS中长整型会丢失精度
        /// </summary>
        [JsonProperty("modifierIDString")]
        [Display(AutoGenerateField = false)]
        [MessagePack.Key("modifierIDString")]
        public string ModifierIDString
        {
            get => ModifierID.ToString();
        }

        /// <summary>
        /// 修改人_名称
        /// </summary>
        public const string Modifier_Name = "Modifier";

        /// <summary>
        /// 修改人
        /// </summary>
        [JsonProperty("modifier")]
        [Display(AutoGenerateField = false)]
        [MessagePack.Key("modifier")]
        public string Modifier
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 带有人时间信息
    /// @ 黄振东
    /// </summary>
    [MessagePackObject]
    public class PersonTimeInfo : PersonTimeInfo<int>
    {
    }

    /// <summary>
    /// 带有人时间商户信息
    /// </summary>
    /// <typeparam name="IdT">ID类型</typeparam>
    public class PersonTimeMerchantInfo<IdT> : PersonTimeInfo<IdT>
    {
        /// <summary>
        /// 商户ID_名称
        /// </summary>
        public const string MerchantID_Name = "MerchantID";

        /// <summary>
        /// 商户ID
        /// </summary>
        [JsonProperty("merchantID")]
        [Display(Name = "商户ID", Order = 10, AutoGenerateField = false)]
        [MessagePack.Key("merchantID")]
        public IdT MerchantID
        {
            get;
            set;
        }

        /// <summary>
        /// 商户ID字符串_名称
        /// </summary>
        public const string MerchantIDString_Name = "MerchantIDString";

        /// <summary>
        /// 商户ID字符串，如果ID类型为长整型，则在JS前端使用此属性为字符串类型，因为JS中长整型会丢失精度
        /// </summary>
        [JsonProperty("merchantIDString")]
        [Display(AutoGenerateField = false)]
        [MessagePack.Key("merchantIDString")]
        public string MerchantIdString
        {
            get => MerchantID.ToString();
        }
    }

    /// <summary>
    /// 带人和时间信息扩展类
    /// @ 黄振东
    /// </summary>
    public static class SimpleInfoExtensions
    {
        /// <summary>
        /// 设置创建信息
        /// </summary>
        /// <typeparam name="IdT">ID类型</typeparam>
        /// <param name="model">模型</param>
        /// <param name="currUser">当前用户</param>
        public static void SetCreateInfo<IdT>(this PersonTimeInfo<IdT> model, BasicUserInfo<IdT> currUser = null)
        {
            var user = UserTool<IdT>.GetCurrUser(currUser);
            if (user == null)
            {
                return;
            }

            model.CreaterID = model.ModifierID = user.Id;
            model.Creater = model.Modifier = user.Name;
            model.CreateDateTime = model.ModifyDateTime = DateTimeExtensions.CstNow();
        }

        /// <summary>
        /// 设置修改信息
        /// </summary>
        /// <typeparam name="IdT">ID类型</typeparam>
        /// <param name="model">模型</param>
        /// <param name="currUser">当前用户</param>
        public static void SetModifyInfo<IdT>(this PersonTimeInfo<IdT> model, BasicUserInfo<IdT> currUser = null)
        {
            var user = UserTool<IdT>.GetCurrUser(currUser);
            if (user == null)
            {
                return;
            }

            model.ModifierID = user.Id;
            model.Modifier = user.Name;
            model.ModifyDateTime = DateTimeExtensions.CstNow();
        }
    }
}
