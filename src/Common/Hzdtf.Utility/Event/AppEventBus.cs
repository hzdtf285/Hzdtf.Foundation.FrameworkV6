﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Hzdtf.Utility.Model;
using Hzdtf.Utility.Utils;

namespace Hzdtf.Utility.Event
{
    /// <summary>
    /// 应用事件总线
    /// @ 黄振东
    /// </summary>
    public class AppEventBus : IEventBus
    {
        #region 内部对象定义

        /// <summary>
        /// 处理数据
        /// </summary>
        struct HandlerData
        {
            /// <summary>
            /// 处理
            /// </summary>
            private readonly Type handler;

            /// <summary>
            /// 处理
            /// </summary>
            public Type Hanler { get => handler; }

            /// <summary>
            /// 处理对象
            /// </summary>
            private readonly IEventHandler handlerObj;

            /// <summary>
            /// 处理对象
            /// </summary>
            public IEventHandler HanlerObj { get => handlerObj; }

            /// <summary>
            /// 构造方法
            /// </summary>
            /// <param name="handler">处理</param>
            public HandlerData(Type handler)
            {
                this.handlerObj = AppEventUtil.CreateHandler(handler);
                this.handler = handler;
            }

            /// <summary>
            /// 构造方法
            /// </summary>
            /// <param name="handlerObj">处理对象</param>
            public HandlerData(object handlerObj)
            {
                if (handlerObj == null)
                {
                    throw new ArgumentNullException("处理对象不能为null");
                }

                if (handlerObj is IEventHandler)
                {
                    this.handler = handlerObj.GetType();
                    this.handlerObj = handlerObj as IEventHandler;
                }
                else
                {
                    throw new NotImplementedException("处理对象未实现IEventHandler接口");
                }
            }
        }

        #endregion

        #region 属性与字段

        /// <summary>
        /// 事件源映射事件处理的字典
        /// </summary>
        private static readonly IDictionary<Type, IList<HandlerData>> dicSourceMapHandler = new ConcurrentDictionary<Type, IList<HandlerData>>();

        /// <summary>
        /// 实例
        /// </summary>
        private static readonly AppEventBus INSTANCE = new AppEventBus();

        /// <summary>
        /// 实例
        /// </summary>
        public static AppEventBus Instance { get => INSTANCE; }

        #endregion

        #region 初始化

        /// <summary>
        /// 构造方法
        /// </summary>
        private AppEventBus() { }

        #endregion

        #region IEventBus 接口

        /// <summary>
        /// 绑定事件源与事件处理的关系
        /// </summary>
        /// <param name="eventSourceType">事件源类型</param>
        /// <param name="eventHandlerType">事件处理类型</param>
        public void Bind(Type eventSourceType, Type eventHandlerType)
        {
            var handlerObj = AppEventUtil.CreateHandler(eventHandlerType);
            Bind(eventSourceType, handlerObj);
        }

        /// <summary>
        /// 绑定事件源与事件处理的关系
        /// </summary>
        /// <param name="eventSourceType">事件源类型</param>
        public void Bind<HandlerT>(Type eventSourceType) where HandlerT : IEventHandler
        {
            Bind(eventSourceType, typeof(HandlerT));
        }

        /// <summary>
        /// 绑定事件源与事件处理的关系
        /// </summary>
        /// <param name="eventSourceType">事件源类型</param>
        /// <param name="eventHandler">事件处理</param>
        public void Bind(Type eventSourceType, IEventHandler eventHandler)
        {
            if (dicSourceMapHandler.ContainsKey(eventSourceType))
            {
                if (dicSourceMapHandler[eventSourceType] == null)
                {
                    dicSourceMapHandler[eventSourceType] = new List<HandlerData>() { new HandlerData(eventHandler) };
                    return;
                }

                if (IsExistsHandlerData(dicSourceMapHandler[eventSourceType], eventHandler.GetType()))
                {
                    return;
                }
                else
                {
                    dicSourceMapHandler[eventSourceType].Add(new HandlerData(eventHandler));
                }
            }
            else
            {
                dicSourceMapHandler.Add(eventSourceType, new List<HandlerData>() { new HandlerData(eventHandler) });
            }
        }

        /// <summary>
        /// 解绑事件源与事件处理的关系
        /// </summary>
        /// <param name="eventSourceType">事件源类型</param>
        /// <param name="eventHandlerType">事件处理类型</param>
        public void UnBind(Type eventSourceType, Type eventHandlerType)
        {
            if (dicSourceMapHandler.ContainsKey(eventSourceType) && dicSourceMapHandler[eventSourceType] != null)
            {
                bool isGeted;
                HandlerData handlerData = FindHandlerData(dicSourceMapHandler[eventSourceType], eventHandlerType, out isGeted);
                if (isGeted)
                {
                    dicSourceMapHandler[eventSourceType].Remove(handlerData);
                }
            }
        }

        /// <summary>
        /// 发布事件
        /// </summary>
        /// <param name="eventSourceType">事件源类型</param>
        /// <param name="eventData">事件数据</param>
        /// <param name="comData">通用数据</param>
        /// <param name="connectionId">连接ID</param>
        public void Publish(Type eventSourceType, EventData eventData, CommonUseData comData = null, string connectionId = null)
        {
            if (!dicSourceMapHandler.ContainsKey(eventSourceType) || dicSourceMapHandler[eventSourceType].IsNullOrCount0())
            {
                return;
            }

            IList<HandlerData> handlerDatas = dicSourceMapHandler[eventSourceType];
            foreach (HandlerData handlerData in handlerDatas)
            {
                handlerData.HanlerObj.Execute(eventData, comData, connectionId);
            }
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 根据事件处理判断是否存在列表里
        /// </summary>
        /// <param name="handlerDatas">处理数据列表</param>
        /// <param name="eventHandler">事件处理</param>
        /// <returns>事件处理判断是否存在列表里</returns>
        private bool IsExistsHandlerData(IList<HandlerData> handlerDatas, Type eventHandler)
        {
            bool isGeted;
            FindHandlerData(handlerDatas, eventHandler, out isGeted);
            return isGeted;
        }

        /// <summary>
        /// 从处理数据列表里查找处理数据
        /// </summary>
        /// <param name="handlerDatas">处理数据列表</param>
        /// <param name="eventHandler">事件处理</param>
        /// <param name="isGeted">是否获取到</param>
        /// <returns>处理数据</returns>
        private HandlerData FindHandlerData(IList<HandlerData> handlerDatas, Type eventHandler, out bool isGeted)
        {
            isGeted = false;
            foreach (HandlerData item in handlerDatas)
            {
                if (item.Hanler == eventHandler)
                {
                    isGeted = true;
                    return item;
                }
            }

            return default(HandlerData);
        }

        #endregion
    }

    /// <summary>
    /// 应用事件辅助类
    /// @ 黄振东
    /// </summary>
    public static class AppEventUtil
    {
        /// <summary>
        /// 创建处理
        /// </summary>
        /// <param name="handler">处理</param>
        /// <returns>处理</returns>
        public static IEventHandler CreateHandler(Type handler)
        {
            if (handler.GetInterface(typeof(IEventHandler).Name) == null)
            {
                throw new NotImplementedException("类型[" + handler.FullName + "]未实现IEventHandler接口");
            }

            object instance = null;
            // 优先从容器里获取
            if (App.Instance != null)
            {
                instance = App.Instance.GetService(handler);
            }
            if (instance == null)
            {
                instance = handler.Assembly.CreateInstance(handler.FullName);
            }

            return instance as IEventHandler;
        }
    }
}
