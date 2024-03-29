﻿using Hzdtf.Utility.RemoteService.Builder;
using Hzdtf.Utility.RemoteService.Options;
using Hzdtf.Utility.RemoteService.Provider;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hzdtf.Utility.RemoteService
{
    /// <summary>
    /// 统一服务生成配置
    /// @ 黄振东
    /// </summary>
    public class UnitServiceBuilderOptions
    {
        /// <summary>
        /// 实例
        /// </summary>
        public static UnitServiceBuilderOptions Instance { get; set; }

        /// <summary>
        /// 服务配置
        /// </summary>
        public UnityServicesOptions ServicesOptions
        {
            get;
            set;
        }

        /// <summary>
        /// 统一服务配置
        /// </summary>
        public IUnityServicesOptions UnityServicesOptions
        {
            get;
            set;
        }

        /// <summary>
        /// 服务生成配置Json文件
        /// </summary>
        public string ServiceBuilderConfigJsonFile
        {
            get;
            set;
        } = "Config/serviceBuilderConfig.json";

        /// <summary>
        /// 统一服务生成器
        /// </summary>
        public IUnityServicesBuilder UnityServicesBuilder
        {
            get;
            set;
        }

        /// <summary>
        /// 统一服务生成器类型
        /// </summary>
        public Type UnityServicesBuilderType
        {
            get;
            set;
        }

        /// <summary>
        /// 原生服务提供者
        /// </summary>
        public INativeServicesProvider NativeServicesProvider
        {
            get;
            set;
        }

        /// <summary>
        /// 原生服务提供者类型
        /// </summary>
        public Type NativeServicesProviderType
        {
            get;
            set;
        }

        /// <summary>
        /// 服务提供者
        /// </summary>
        public IServicesProvider ServicesProvider
        {
            get;
            set;
        }

        /// <summary>
        /// 服务提供者类型
        /// </summary>
        public Type ServicesProviderType
        {
            get;
            set;
        }
    }
}
