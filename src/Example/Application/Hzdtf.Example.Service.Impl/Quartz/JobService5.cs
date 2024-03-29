﻿using Hzdtf.Quartz.Extensions.Job;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hzdtf.Example.Service.Impl.Quartz
{
    /// <summary>
    /// 作业服务5
    /// @ 黄振东
    /// </summary>
    public class JobService5 : JobBase
    {
        /// <summary>
        /// 执行业务处理
        /// </summary>
        /// <param name="context">工作执行上下文</param>
        /// <param name="transId">事务ID</param>
        public override void ExecBusinessHandle(IJobExecutionContext context, long transId)
        {
            Console.WriteLine($"{DateTimeExtensions.Now.ToFullFixedDateTime()}.JobService5");
        }
    }
}
