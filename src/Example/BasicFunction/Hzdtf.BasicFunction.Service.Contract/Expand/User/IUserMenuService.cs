using Hzdtf.BasicFunction.Model.Expand.User;
using Hzdtf.Utility.Model;
using Hzdtf.Utility.Model.Return;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hzdtf.BasicFunction.Service.Contract.User
{
    /// <summary>
    /// 用户菜单服务接口
    /// @ 黄振东
    /// </summary>
    public partial interface IUserMenuService
    {
        /// <summary>
        /// 根据用户ID获取能访问的菜单列表
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="connectionId">连接ID</param>
        /// <param name="comData">通用数据</param>
        /// <returns>返回信息</returns>
        ReturnInfo<UserMenuInfo> CanAccessMenus(int userId, CommonUseData comData = null, string connectionId = null);
    }
}
