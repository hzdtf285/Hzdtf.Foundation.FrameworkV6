﻿using Hzdtf.BasicFunction.Model;
using Hzdtf.BasicFunction.Model.Expand.Menu;
using Hzdtf.Utility.Model;
using Hzdtf.Utility.Model.Return;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hzdtf.BasicFunction.Service.Impl
{
    /// <summary>
    /// 菜单服务
    /// @ 黄振东
    /// </summary>
    public partial class MenuService
    {
        /// <summary>
        /// 查询菜单树列表
        /// </summary>
        /// <param name="connectionId">连接ID</param>
        /// <param name="comData">通用数据</param>
        /// <returns>返回信息</returns>
        public virtual ReturnInfo<IList<MenuTreeInfo>> QueryMenuTrees(CommonUseData comData = null, string connectionId = null)
        {
            return ExecReturnFunc<IList<MenuTreeInfo>>((reInfo) =>
            {
                IList<MenuInfo> menus = persistence.SelectContainsFunctions(connectionId);
                return menus.ToOrigAndSort().ToMenuTrees();
            });
        }
    }
}
