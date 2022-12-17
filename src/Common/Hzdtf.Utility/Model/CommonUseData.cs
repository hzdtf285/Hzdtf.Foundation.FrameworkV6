using Hzdtf.Utility.TheOperation;
using MessagePack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hzdtf.Utility.Utils;
using Hzdtf.Utility.Enums;
using System.Collections.Concurrent;

namespace Hzdtf.Utility.Model
{
    /// <summary>
    /// 通用数据
    /// @ 黄振东
    /// </summary>
    [MessagePackObject]
    public class CommonUseData
    {
        /// <summary>
        /// 键
        /// </summary>
        [JsonProperty("key")]
        [Key("key")]
        public string Key
        {
            get;
            set;
        }

        /// <summary>
        /// 控制器
        /// </summary>
        private readonly string controller;

        /// <summary>
        /// 控制器
        /// </summary>
        [JsonProperty("controller")]
        [Key("controller")]
        public string Controller
        {
            get => controller;
        }

        /// <summary>
        /// 动作
        /// </summary>
        private readonly string action;

        /// <summary>
        /// 动作
        /// </summary>
        [JsonProperty("action")]
        [Key("action")]
        public string Action
        {
            get => action;
        }

        /// <summary>
        /// 路径，小写
        /// </summary>
        private readonly string path;

        /// <summary>
        /// 路径，小写
        /// </summary>
        [JsonProperty("path")]
        [Key("path")]
        public string Path
        {
            get => path;
        }

        /// <summary>
        /// 客户端远程IP
        /// </summary>
        private readonly string clientRemoteIp;

        /// <summary>
        /// 客户端远程IP
        /// </summary>
        [JsonProperty("clientRemoteIp")]
        [Key("clientRemoteIp")]
        public string ClientRemoteIp
        {
            get => clientRemoteIp;
        }

        /// <summary>
        /// 通讯方式
        /// </summary>
        private readonly CommunicationMode commMode = CommunicationMode.NONE;

        /// <summary>
        /// 通讯方式
        /// </summary>
        [JsonProperty("commMode")]
        [Key("commMode")]
        public CommunicationMode CommMode
        {
            get => commMode;
        }

        /// <summary>
        /// 菜单编码
        /// </summary>
        [JsonProperty("menuCode")]
        [Key("menuCode")]
        public string MenuCode
        {
            get;
            set;
        }

        /// <summary>
        /// 功能编码数组
        /// </summary>
        [JsonProperty("functionCodes")]
        [Key("functionCodes")]
        public string[] FunctionCodes
        {
            get;
            set;
        }

        /// <summary>
        /// 当前用户
        /// </summary>
        [JsonProperty("currUser")]
        [Key("currUser")]
        public object CurrUser
        {
            get;
            set;
        }

        /// <summary>
        /// 票据
        /// </summary>
        private readonly string token;

        /// <summary>
        /// 票据
        /// </summary>
        [JsonProperty("token")]
        [Key("token")]
        public string Token
        {
            get => token;
        }

        /// <summary>
        /// 事件ID
        /// </summary>
        [JsonProperty("eventId")]
        [Key("eventId")]
        public string EventId
        {
            get;
            set;
        }

        /// <summary>
        /// 扩展属性
        /// </summary>
        [JsonProperty("extend")]
        [Key("extend")]
        public IDictionary<string, object> Extend
        {
            get;
            set;
        }

        /// <summary>
        /// 标签
        /// </summary>
        [JsonProperty("tag")]
        [Key("tag")]
        public object Tag
        {
            get;
            set;
        }

        /// <summary>
        /// 回调字典
        /// Key：方法
        /// value：参数
        /// </summary>
        private IDictionary<Action<object>, object> callbacks;

        /// <summary>
        /// 回调字典
        /// Key：方法
        /// value：参数
        /// </summary>
        [IgnoreMember]
        [JsonIgnore]
        public IDictionary<Action<object>, object> Callbacks
        {
            get => callbacks;
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        public CommonUseData() { }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="controller">控制器</param>
        /// <param name="action">动作</param>
        /// <param name="path">路径</param>
        /// <param name="commMode">通讯方式</param>
        /// <param name="clientRemoteIp">客户端远程IP</param>
        /// <param name="token">票据</param>
        public CommonUseData(string controller = null, string action = null, string path = null,
            CommunicationMode commMode = CommunicationMode.NONE, string clientRemoteIp = null, string token = null)
        {
            this.controller = controller;
            this.action = action;
            this.path = path;
            this.commMode = commMode;
            this.clientRemoteIp = clientRemoteIp;
            this.token = token;
        }

        /// <summary>
        /// 添加回调
        /// </summary>
        /// <param name="callback">回调方法</param>
        /// <param name="inParams">输入参数数组</param>
        public void AddCallback(Action<object> callback, object inParams)
        {
            if (callbacks == null)
            {
                callbacks = new ConcurrentDictionary<Action<object>, object>();
            }
            callbacks.Add(callback, inParams);
        }

        /// <summary>
        /// 移除回调
        /// </summary>
        /// <param name="callback">回调</param>
        public void RemoveCallback(Action<object> callback)
        {
            if (callback == null || callbacks.IsNullOrCount0())
            {
                return;
            }
            if (callbacks.ContainsKey(callback))
            {
                callbacks.Remove(callback);
            }
        }

        /// <summary>
        /// 清空回调
        /// </summary>
        public void ClearCallback()
        {
            if (callbacks == null)
            {
                return;
            }

            callbacks.Clear();
            callbacks = null;
        }
    }

    /// <summary>
    /// 通用数据扩展类
    /// @ 黄振东
    /// </summary>
    public static class CommonUseDataExtensions
    {
        /// <summary>
        /// 获取当前用户
        /// </summary>
        /// <param name="comData">通用数据</param>
        /// <returns>当前用户</returns>
        public static object GetCurrUser(this CommonUseData comData)
        {
            return comData != null ? comData.CurrUser : null;
        }

        /// <summary>
        /// 获取Token，如果里面的为空，则会执行App.GetTokenFunc() 
        /// </summary>
        /// <param name="comData">通用数据</param>
        /// <returns>Token</returns>
        public static string GetToken(this CommonUseData comData)
        {
            if (comData == null || string.IsNullOrWhiteSpace(comData.Token))
            {
                return App.GetTokenFunc != null ? App.GetTokenFunc() : null;
            }
            else
            {
                return comData.Token;
            }
        }

        /// <summary>
        /// 获取事件ID，如果里面的为空，则会执行theOperation.EventId,如果传入的theOperation为null，则会执行 App.TheOperation.EventId
        /// </summary>
        /// <param name="comData">通用数据</param>
        /// <param name="theOperation">本次操作</param>
        /// <returns>事件ID</returns>
        public static string GetEventId(this CommonUseData comData, ITheOperation theOperation = null)
        {
            if (comData == null || string.IsNullOrWhiteSpace(comData.EventId))
            {
                theOperation = theOperation == null ? App.TheOperation : theOperation;
                if (theOperation == null)
                {
                    return null;
                }

                return theOperation.EventId;
            }
            else
            {
                return comData.EventId;
            }
        }

        /// <summary>
        /// 获取扩展字典值
        /// </summary>
        /// <param name="comData">通用数据</param>
        /// <param name="key">键</param>
        /// <returns>扩展字典值</returns>
        public static object GetExtendValue(this CommonUseData comData, string key)
        {
            if (comData == null || string.IsNullOrWhiteSpace(key) || comData.Extend.IsNullOrCount0())
            {
                return null;
            }

            return comData.Extend.ContainsKey(key) ? comData.Extend[key] : null;
        }

        /// <summary>
        /// 获取标签
        /// </summary>
        /// <param name="comData">通用数据</param>
        /// <returns>扩展字典值</returns>
        public static object GetTag(this CommonUseData comData)
        {
            return comData == null ? null : comData.Tag;
        }
    }
}
