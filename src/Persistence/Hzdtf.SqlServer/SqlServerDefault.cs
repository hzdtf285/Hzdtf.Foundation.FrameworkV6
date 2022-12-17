using Hzdtf.Logger.Contract;
using Hzdtf.Persistence.Contract.Basic;
using Hzdtf.Persistence.Contract.Basic.Default;
using Hzdtf.Utility.Attr;
using Hzdtf.Utility.Enums;
using Hzdtf.Utility.Model.Return;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hzdtf.SqlServer
{
    /// <summary>
    /// SQL Server默认
    /// @ 黄振东
    /// </summary>
    [Inject]
    public sealed class SqlServerDefault : PersistenceConnectionBase, IPersistenceConnectionDefault
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="defaultConnectionString">默认连接字符串，默认取0主库；1从库</param>
        /// <param name="log">日志</param>
        public SqlServerDefault(IDefaultConnectionString defaultConnectionString = null, ILogable log = null)
            : base(defaultConnectionString, log)
        {
        }

        /// <summary>
        /// 执行返回函数且带有连接ID
        /// </summary>
        /// <typeparam name="OutT">输出类型</typeparam>
        /// <param name="func">函数</param>
        /// <param name="returnInfo">返回信息</param>
        /// <param name="connectionId">连接ID</param>
        /// <param name="accessMode">访问模式</param>
        /// <returns>返回信息</returns>
        public ReturnInfo<OutT> ExecReturnFuncAndConnectionId<OutT>(Func<ReturnInfo<OutT>, string, OutT> func, ReturnInfo<OutT> returnInfo = null, string connectionId = null, AccessMode accessMode = AccessMode.MASTER)
        {
            return ExecReturnFunc<OutT>((reInfo) =>
            {
                OutT result = default(OutT);
                ExecProcConnectionId((connId) =>
                {
                    result = func(reInfo, connId);
                }, connectionId: connectionId, accessMode);

                return result;
            }, returnInfo);
        }

        /// <summary>
        /// 执行返回函数
        /// </summary>
        /// <typeparam name="OutT">输出类型</typeparam>
        /// <param name="func">函数</param>
        /// <param name="returnInfo">返回信息</param>
        /// <returns>返回信息</returns>
        public ReturnInfo<OutT> ExecReturnFunc<OutT>(Func<ReturnInfo<OutT>, OutT> func, ReturnInfo<OutT> returnInfo = null)
        {
            if (returnInfo == null)
            {
                returnInfo = new ReturnInfo<OutT>();
            }

            returnInfo.Data = func(returnInfo);

            return returnInfo;
        }

        /// <summary>
        /// 执行连接ID过程
        /// 如果传过来的连接ID为空，则会创建新的连接ID，结束后会自动注释连接ID，否则不会
        /// </summary>
        /// <param name="action">动作</param>
        /// <param name="connectionId">连接ID</param>
        /// <param name="accessMode">访问模式</param>
        public void ExecProcConnectionId(Action<string> action, string connectionId = null, AccessMode accessMode = AccessMode.MASTER)
        {
            if (string.IsNullOrWhiteSpace(connectionId))
            {
                connectionId = NewConnectionId(accessMode);

                try
                {
                    action(connectionId);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
                finally
                {
                    Release(connectionId);
                }
            }
            else
            {
                action(connectionId);
            }
        }

        /// <summary>
        /// 创建数据库连接
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        /// <returns>数据库连接</returns>
        public override IDbConnection CreateDbConnection(string connectionString) => new SqlConnection(connectionString);
    }
}
