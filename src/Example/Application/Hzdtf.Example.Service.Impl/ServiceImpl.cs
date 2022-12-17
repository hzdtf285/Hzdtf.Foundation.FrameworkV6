using Hzdtf.Example.Persistence.Contract;
using Hzdtf.Example.Service.Contract;
using Hzdtf.Persistence.Contract.Basic;
using Hzdtf.Persistence.Contract.Basic.Default;
using Hzdtf.Utility.Attr;
using Hzdtf.Utility.Data;
using Hzdtf.Utility.Model.Return;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hzdtf.Utility.Model;
using Hzdtf.Utility.Enums;

namespace Hzdtf.Example.Service.Impl
{
    public class ServiceImpl : IService
    {
        private readonly ITest2Service service;

        private readonly ITest2Persistence persistence;
        public ServiceImpl(ITest2Service service, ITest2Persistence persistence)
        {
            this.service = service;
            this.persistence = persistence;
        }

        [Transaction(ConnectionIdIndex = 0)]
        public virtual ReturnInfo<bool> Fun(string connectionId = null)
        {
            var s = new Model.Test2Info()
            {
                OrderNo = 1
            };

            ReturnInfo<bool> re = null;
            try
            {
                var ll = persistence.UpdateById(new Model.Test2Info()
                {
                    Id = 1,
                    OrderNo = 100,
                }, propertyNames: new string[] { "OrderNo"});

                var a = persistence.SelectPage(0, 1, param: new SqlPropInfo()
                {
                    PropertyNames = new string[]
                    {
                        "OrderNo"
                    },
                    FilterSql = $"{"OrderNo".SqlPackPropName()}=@OrderNo",
                    Params = new
                    {
                        OrderNo = 1,
                    },
                });


                var lll = persistence.Delete(new SqlPropInfo()
                {
                    Top = 1,
                    PropertyNames = new string[]
                    {
                        "OrderNo"
                    },
                    FilterSql = $"{"OrderNo".SqlPackPropName()}=@OrderNo",
                    Params = new
                    {
                        OrderNo = 10,
                        Order1 = 2
                    }
                });
                //re = service.Add(s, connectionId: connectionId);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("IX_Test2_orderno"))
                {
                    s.Id = 0;
                    re = service.Add(s, connectionId: connectionId);
                }
            }


            return re;
        }
    }
}
