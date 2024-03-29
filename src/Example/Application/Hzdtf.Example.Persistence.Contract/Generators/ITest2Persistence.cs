﻿using Hzdtf.Persistence.Contract.Data;
using System;
using System.Collections.Generic;
using System.Text;
using Hzdtf.Example.Model;

namespace Hzdtf.Example.Persistence.Contract
{
    /// <summary>
    /// Test2持久化接口
    /// @ 黄振东
    /// </summary>
    public partial interface ITest2Persistence : IPersistence<int, Test2Info>
    {
    }
}
