﻿using Hzdtf.Utility.LoadBalance;
using Hzdtf.Utility.RemoteService.Provider;
using Hzdtf.Utility.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hzdtf.Utility.RemoteService.Builder
{
    /// <summary>
    /// Http服务生成
    /// @ 黄振东
    /// </summary>
    public class HttpServicesBuilder : IServicesBuilder
    {
        /// <summary>
        /// 服务提供者
        /// </summary>
        public IServicesProvider ServiceProvider
        {
            get;
            set;
        }

        /// <summary>
        /// 服务名
        /// </summary>
        public string ServiceName
        {
            get;
            set;
        }

        /// <summary>
        /// 方案
        /// </summary>
        public string Sheme
        {
            get;
            set;
        } = Uri.UriSchemeHttp;

        /// <summary>
        /// 标签
        /// </summary>
        public string Tag
        {
            get;
            set;
        }

        /// <summary>
        /// 负载均衡策略
        /// </summary>
        public ILoadBalance LoadBalance
        {
            get;
            set;
        } = LoadBalanceType.Random;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="serviceProvider">服务提供者</param>
        public HttpServicesBuilder(IServicesProvider serviceProvider = null)
        {
            this.ServiceProvider = serviceProvider;
        }

        /// <summary>
        /// 异步生成地址
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns>生成地址任务</returns>
        public async Task<string> BuilderAsync(string path = null)
        {
            var addresses = await ServiceProvider.GetAddresses(ServiceName, Tag);
            if (addresses.IsNullOrLength0())
            {
                throw new Exception($"获取服务[{ServiceName}],标签[{Tag}]的地址列表为空");
            }

            var address = LoadBalance.Resolve(addresses);
            return BuilderByBaseAddress(address, path);
        }

        /// <summary>
        /// 根据基地址生成地址
        /// </summary>
        /// <param name="baseAddress">基地址</param>
        /// <param name="path">路径</param>
        /// <returns>生成地址</returns>
        public string BuilderByBaseAddress(string baseAddress, string path = null)
        {
            var baseUri = new Uri($"{Sheme}://{baseAddress}");
            if (string.IsNullOrWhiteSpace(path))
            {
                return baseUri.AbsoluteUri;
            }

            return new Uri(baseUri, path).AbsoluteUri;
        }
    }
}
