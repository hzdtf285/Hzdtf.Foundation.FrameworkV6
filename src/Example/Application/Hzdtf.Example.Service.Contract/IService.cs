using Hzdtf.Utility.Model.Return;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hzdtf.Example.Service.Contract
{
    public interface IService
    {
        ReturnInfo<bool> Fun(string connectionId = null);
    }
}
