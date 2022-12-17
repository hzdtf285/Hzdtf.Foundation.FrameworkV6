using Hzdtf.Utility.Model;
using System;
using System.Collections.Generic;

namespace Hzdtf.Utility.HostConfig
{
    /// <summary>
    /// 主机配置JSON文件
    /// @ 黄振东
    /// </summary>
    public class HostConfigJsonFile : IHostConfigReader
    {
        /// <summary>
        /// json文件
        /// </summary>
        private readonly string jsonFile;

        /// <summary>
        /// 构造方法
        /// 默认读取AmqpConfigFile配置，如果没有，则读取当前目录的host.json
        /// </summary>
        public HostConfigJsonFile()
        {
            if (App.CurrConfig == null || string.IsNullOrWhiteSpace(App.CurrConfig["HostConfigFile"]))
            {
                this.jsonFile = "host.json";
            }
            else
            {
                this.jsonFile = App.CurrConfig["HostConfigFile"];
            }
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="jsonFile">配置JSON文件</param>
        public HostConfigJsonFile(string jsonFile)
        {
            this.jsonFile = jsonFile;
        }

        /// <summary>
        /// 读取
        /// </summary>
        /// <returns>数据</returns>
        public IDictionary<string, KeyValueInfo<string, int>[]> Reader()
        {
            return jsonFile.ToJsonObjectFromFile<IDictionary<string, KeyValueInfo<string, int>[]>>();
        }
    }
}
