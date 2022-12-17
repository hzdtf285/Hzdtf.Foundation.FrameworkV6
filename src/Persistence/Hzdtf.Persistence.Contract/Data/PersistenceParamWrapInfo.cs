using Hzdtf.Utility.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hzdtf.Persistence.Contract.Data
{
    /// <summary>
    /// 持久化参数包装信息
    /// @ 黄振东
    /// </summary>
    public class PersistenceParamWrapInfo
    {
        /// <summary>
        /// 属性名称数组，如果为空，则默认所有
        /// </summary>
        public string[] PropertyNames
        {
            get;
            set;
        }

        /// <summary>
        /// 参数
        /// </summary>
        public object Params
        {
            get;
            set;
        }

        /// <summary>
        /// 过滤SQL，如果为空，则不添加任何条件语句
        /// 使用属性名过滤，属性名需要加前后辍，使用"SqlUtil.FilterPrefix属性名SqlUtil.FilterSuffixes"
        /// 注意不用加WHERE
        /// </summary>
        public string FilterSql
        {
            get;
            set;
        }

        /// <summary>
        /// 通用数据
        /// </summary>
        public CommonUseData ComData
        {
            get;
            set;
        }

        /// <summary>
        /// 连接ID
        /// </summary>
        public string ConnectionId
        {
            get;
            set;
        }
    }
}
