﻿using Hzdtf.Example.Model;
using Hzdtf.Example.Persistence.Contract;
using Hzdtf.Example.Service.Contract;
using Hzdtf.Logger.Contract;
using Hzdtf.Service.Impl;
using Hzdtf.Utility.Attr;
using Hzdtf.Utility.Localization;
using Hzdtf.Utility.Model.Identitys;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hzdtf.Example.Service.Impl
{
    /// <summary>
    /// 服务
    /// @ 黄振东
    /// </summary>
    [Inject]
    public partial class TestFormService : ServiceBase<int, TestFormInfo, ITestFormPersistence>, ITestFormService
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="log">日志</param>
        /// <param name="persistence">持久化</param>
        /// <param name="identity">ID</param>
        /// <param name="localize">本地化</param>
        public TestFormService(ILogable log = null, ITestFormPersistence persistence = null, IIdentity<int> identity = null, ILocalization localize = null)
            : base(log, persistence, identity, localize) { }
    }
}
