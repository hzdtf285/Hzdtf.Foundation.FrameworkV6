using Hzdtf.BasicFunction.Model;
using Hzdtf.Utility.Model;
using Hzdtf.Utility.Model.Return;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hzdtf.BasicFunction.Service.Contract
{
    /// <summary>
    /// 角色服务接口
    /// @ 黄振东
    /// </summary>
    public partial interface IRoleService
    {
        /// <summary>
        /// 查询角色列表并去掉系统隐藏
        /// </summary>
        /// <param name="connectionId">连接ID</param>
        /// <param name="comData">通用数据</param>
        /// <returns>返回信息</returns>
        ReturnInfo<IList<RoleInfo>> QueryAndNotSystemHide(CommonUseData comData = null, string connectionId = null);

        /// <summary>
        /// 根据筛选条件查询角色列表
        /// </summary>
        /// <param name="filter">筛选</param>
        /// <param name="connectionId">连接ID</param>
        /// <param name="comData">通用数据</param>
        /// <returns>返回信息</returns>
        ReturnInfo<IList<RoleInfo>> QueryByFilter(KeywordFilterInfo filter, CommonUseData comData = null, string connectionId = null);
    }
}
