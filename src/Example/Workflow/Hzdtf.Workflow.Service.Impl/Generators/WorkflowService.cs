using Hzdtf.Workflow.Model;
using Hzdtf.Workflow.Persistence.Contract;
using Hzdtf.Workflow.Service.Contract;
using Hzdtf.Service.Impl;
using Hzdtf.Utility.Attr;
using System;
using System.Collections.Generic;
using System.Text;
using Hzdtf.Logger.Contract;
using Hzdtf.Utility.Localization;
using Hzdtf.Utility.Model.Identitys;

namespace Hzdtf.Workflow.Service.Impl
{
    /// <summary>
    /// 工作流服务
    /// @ 黄振东
    /// </summary>
    [Inject]
    public partial class WorkflowService : ServiceBase<int, WorkflowInfo, IWorkflowPersistence>, IWorkflowService
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="log">日志</param>
        /// <param name="persistence">持久化</param>
        /// <param name="identity">ID</param>
        /// <param name="localize">本地化</param>
        public WorkflowService(ILogable log = null, IWorkflowPersistence persistence = default(IWorkflowPersistence), IIdentity<int> identity = null, ILocalization localize = null)
            : base(log, persistence, identity, localize) { }
    }
}
