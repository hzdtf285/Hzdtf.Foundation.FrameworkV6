using Hzdtf.Utility.Attr;
using Hzdtf.Utility.HostConfig;
using Hzdtf.Utility.Model;
using Hzdtf.Utility.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hzdtf.Utility.RemoteService.Provider
{
    /// <summary>
    /// 主机配置服务提供者
    /// @ 黄振东
    /// </summary>
    public class HostConfigServiceProvider : IServicesProvider
    {
        /// <summary>
        /// 主机配置读取
        /// </summary>
        private readonly IHostConfigReader configReader;

        /// <summary>
        /// 数据字典
        /// </summary>
        private IDictionary<string, KeyValueInfo<string, int>[]> dicData;

        /// <summary>
        /// 同步数据字典
        /// </summary>
        private readonly object syncDicData = new object();

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="configReader">主机配置读取，如果为null，默认为HostConfigJsonFile</param>
        public HostConfigServiceProvider(IHostConfigReader configReader = null)
        {
            if (configReader == null)
            {
                this.configReader = new HostConfigJsonFile();
            }
            else 
            { 
                this.configReader = configReader;
            }
        }

        /// <summary>
        /// 异步根据服务名获取地址数组
        /// </summary>
        /// <param name="serviceName">服务名</param>
        /// <param name="tag">标签</param>
        /// <returns>地址数组任务</returns>
        public async Task<string[]> GetAddresses(string serviceName, string tag = null)
        {
            if (dicData == null)
            {
                lock (syncDicData)
                {
                    if (dicData == null)
                    {
                        dicData = configReader.Reader();
                    }
                }
                if (dicData.IsNullOrCount0())
                {
                    return null;
                }
            }
            if (dicData.ContainsKey(serviceName))
            {
                var values = dicData[serviceName];
                var urls = new string[values.Length];
                for (var i = 0; i < urls.Length; i++)
                {
                    urls[i] = $"{values[i].Key}:{values[i].Value}";
                }

                return urls;
            }

            return null;
        }

        /// <summary>
        /// 释放
        /// </summary>
        [ProcTrackLog(ExecProc = false)]
        public void Dispose()
        {
            if (syncDicData == null)
            {
                return;
            }
            dicData.Clear();
            dicData = null;
        }
    }
}
