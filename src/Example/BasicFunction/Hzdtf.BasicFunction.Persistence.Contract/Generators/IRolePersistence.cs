using Hzdtf.Persistence.Contract.Data;
using System;
using System.Collections.Generic;
using System.Text;
using Hzdtf.BasicFunction.Model;

namespace Hzdtf.BasicFunction.Persistence.Contract
{
    /// <summary>
    /// 角色持久化接口
    /// @ 黄振东
    /// </summary>
    public partial interface IRolePersistence : IPersistence<int, RoleInfo>
    {
    }
}
