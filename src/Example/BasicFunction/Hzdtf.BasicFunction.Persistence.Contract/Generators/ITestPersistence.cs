using Hzdtf.Persistence.Contract.Data;
using System;
using System.Collections.Generic;
using System.Text;
using Hzdtf.BasicFunction.Model;

namespace Hzdtf.BasicFunction.Persistence.Contract
{
    /// <summary>
    /// Test持久化接口
    /// </summary>
    public partial interface ITestPersistence : IPersistence<int, TestInfo>
    {
    }
}
