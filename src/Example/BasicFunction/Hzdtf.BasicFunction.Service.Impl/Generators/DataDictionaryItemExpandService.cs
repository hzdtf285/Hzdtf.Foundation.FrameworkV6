using Hzdtf.BasicFunction.Model;
using Hzdtf.BasicFunction.Persistence.Contract;
using Hzdtf.BasicFunction.Service.Contract;
using Hzdtf.Logger.Contract;
using Hzdtf.Service.Impl;
using Hzdtf.Utility.Attr;
using Hzdtf.Utility.Localization;
using Hzdtf.Utility.Model.Identitys;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hzdtf.BasicFunction.Service.Impl
{
    /// <summary>
    /// 数据字典子项扩展服务
    /// @ 黄振东
    /// </summary>
    [Inject]
    public partial class DataDictionaryItemExpandService : ServiceBase<int, DataDictionaryItemExpandInfo, IDataDictionaryItemExpandPersistence>, IDataDictionaryItemExpandService
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="log">日志</param>
        /// <param name="persistence">持久化</param>
        /// <param name="identity">ID</param>
        /// <param name="localize">本地化</param>
        public DataDictionaryItemExpandService(ILogable log = null, IDataDictionaryItemExpandPersistence persistence = default(IDataDictionaryItemExpandPersistence), IIdentity<int> identity = null, ILocalization localize = null)
            : base(log, persistence, identity, localize) { }
    }
}
