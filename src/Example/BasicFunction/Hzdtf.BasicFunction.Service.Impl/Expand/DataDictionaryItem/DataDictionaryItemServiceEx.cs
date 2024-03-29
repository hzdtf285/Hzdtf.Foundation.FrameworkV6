﻿using Hzdtf.BasicFunction.Model;
using Hzdtf.BasicFunction.Persistence.Contract;
using Hzdtf.Utility.Attr.ParamAttr;
using Hzdtf.Utility.Model.Return;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Hzdtf.Utility.Utils;
using System.Text;
using Hzdtf.Utility.Model;

namespace Hzdtf.BasicFunction.Service.Impl
{
    /// <summary>
    /// 数据字典子项服务
    /// @ 黄振东
    /// </summary>
    public partial class DataDictionaryItemService
    {
        #region 属性与字段

        /// <summary>
        /// 数据字典子项扩展持久化
        /// </summary>
        public IDataDictionaryItemExpandPersistence DataDictionaryItemExpandPersistence
        {
            get;
            set;
        }

        #endregion

        #region IDataDictionaryService 接口

        /// <summary>
        /// 根据数据字典ID获取数据字典子项列表
        /// </summary>
        /// <param name="dataDictionaryId">数据字典ID</param>
        /// <param name="connectionId">连接ID</param>
        /// <param name="comData">通用数据</param>
        /// <returns>返回信息</returns>
        public virtual ReturnInfo<IList<DataDictionaryItemInfo>> QueryByDataDictionaryId([DisplayName2("数据字典ID"), Id] int dataDictionaryId, CommonUseData comData = null, string connectionId = null)
        {
            return ExecReturnFunc<IList<DataDictionaryItemInfo>>((reInfo) =>
            {
                return persistence.SelectByDataDictionaryId(dataDictionaryId, connectionId);
            });            
        }

        /// <summary>
        /// 根据数据字典编码获取数据字典子项列表
        /// </summary>
        /// <param name="dataDictionaryCode">数据字典编码</param>
        /// <param name="connectionId">连接ID</param>
        /// <param name="comData">通用数据</param>
        /// <returns>返回信息</returns>
        public virtual ReturnInfo<IList<DataDictionaryItemInfo>> QueryByDataDictionaryCode([DisplayName2("数据字典编码"), Required] string dataDictionaryCode, CommonUseData comData = null, string connectionId = null)
        {
            return ExecReturnFunc<IList<DataDictionaryItemInfo>>((reInfo) =>
            {
                return persistence.SelectByDataDictionaryCode(dataDictionaryCode, connectionId);
            });
        }

        #endregion

        #region 重写父类的方法

        /// <summary>
        /// 添加模型前
        /// </summary>
        /// <param name="returnInfo">返回信息</param>
        /// <param name="model">模型</param>
        /// <param name="connectionId">连接ID</param>
        /// <param name="comData">通用数据</param>
        protected override void BeforeAdd(ReturnInfo<bool> returnInfo, DataDictionaryItemInfo model, ref string connectionId, CommonUseData comData = null)
        {
            bool idClose = false;
            if (string.IsNullOrWhiteSpace(connectionId))
            {
                idClose = true;
                connectionId = persistence.NewConnectionId();
            }
            try
            {
                ValiExistsParam(returnInfo, model, persistence.CountByDataItemIdAndText(model.DataDictionaryId, model.Text, connectionId));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            finally
            {
                if (idClose && returnInfo.Failure())
                {
                    persistence.Release(connectionId);
                }
            }
        }

        /// <summary>
        /// 根据ID修改模型前
        /// </summary>
        /// <param name="returnInfo">返回信息</param>
        /// <param name="model">模型</param>
        /// <param name="connectionId">连接ID</param>
        /// <param name="comData">通用数据</param>
        protected override void BeforeModifyById(ReturnInfo<bool> returnInfo, DataDictionaryItemInfo model, ref string connectionId, CommonUseData comData = null)
        {
            bool idClose = false;
            if (string.IsNullOrWhiteSpace(connectionId))
            {
                idClose = true;
                connectionId = persistence.NewConnectionId();
            }
            try
            {
                ValiExistsParam(returnInfo, model, persistence.CountByDataItemIdAndTextNotId(model.Id, model.DataDictionaryId, model.Text, connectionId));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            finally
            {
                if (idClose && returnInfo.Failure())
                {
                    persistence.Release(connectionId);
                }
            }
        }

        /// <summary>
        /// 添加模型列表前
        /// </summary>
        /// <param name="returnInfo">返回信息</param>
        /// <param name="models">模型列表</param>        
        /// <param name="connectionId">连接ID</param>
        /// <param name="comData">通用数据</param>
        protected override void BeforeAdd(ReturnInfo<bool> returnInfo, IList<DataDictionaryItemInfo> models, ref string connectionId, CommonUseData comData = null)
        {
            for (var i = 0; i < models.Count; i++)
            {
                BeforeAdd(returnInfo, models[i], ref connectionId);
                if (returnInfo.Failure())
                {
                    returnInfo.SetFailureMsg($"第{i + 1}行:{returnInfo.Msg}");
                    return;
                }
            }
        }

        /// <summary>
        /// 根据ID查找模型后
        /// </summary>
        /// <param name="returnInfo">返回信息</param>
        /// <param name="id">ID</param>
        /// <param name="connectionId">连接ID</param>
        /// <param name="comData">通用数据</param>
        protected override void AfterFind(ReturnInfo<DataDictionaryItemInfo> returnInfo, int id, ref string connectionId, CommonUseData comData = null)
        {
            if (returnInfo.Success() && returnInfo.Data != null && !string.IsNullOrWhiteSpace(returnInfo.Data.ExpandTable))
            {
                if("data_dictionary_item_expand".Equals(returnInfo.Data.ExpandTable))
                {
                    returnInfo.Data.Expands = DataDictionaryItemExpandPersistence.SelectByDataDictionaryItemId(returnInfo.Data.Id, connectionId);
                }
            }
        }

        /// <summary>
        /// 根据ID移除模型前
        /// </summary>
        /// <param name="returnInfo">返回信息</param>
        /// <param name="id">ID</param>
        /// <param name="connectionId">连接ID</param>
        /// <param name="comData">通用数据</param>
        /// <returns>返回信息</returns>
        protected override void BeforeRemoveById(ReturnInfo<bool> returnInfo, int id, ref string connectionId, CommonUseData comData = null)
        {
            ValiCanRemove(returnInfo, persistence.Select(id, connectionId: connectionId, comData: comData));
        }

        /// <summary>
        /// 根据ID集合移除模型前
        /// </summary>
        /// <param name="returnInfo">返回信息</param>
        /// <param name="ids">ID集合</param>
        /// <param name="connectionId">连接ID</param>
        /// <param name="comData">通用数据</param>
        /// <returns>返回信息</returns>
        protected override void BeforeRemoveByIds(ReturnInfo<bool> returnInfo, int[] ids, ref string connectionId, CommonUseData comData = null)
        {
            IList<DataDictionaryItemInfo> users = persistence.Select(ids, connectionId: connectionId, comData: comData);
            if (users.IsNullOrCount0())
            {
                return;
            }

            foreach (DataDictionaryItemInfo item in users)
            {
                ValiCanRemove(returnInfo, item);
                if (returnInfo.Failure())
                {
                    return;
                }
            }
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 验证存在的参数
        /// </summary>
        /// <param name="returnInfo">返回信息</param>
        /// <param name="model">模型</param>
        /// <param name="existsCount">存在的个数</param>
        private void ValiExistsParam(ReturnInfo<bool> returnInfo, DataDictionaryItemInfo model, int existsCount)
        {
            if (existsCount <= 0)
            {
                return;
            }

            returnInfo.SetFailureMsg($"文本:{model.Text}已存在");
        }

        /// <summary>
        /// 验证是否能移除
        /// </summary>
        /// <param name="returnInfo">返回信息</param>
        /// <param name="item">数据字典子项</param>
        private void ValiCanRemove(ReturnInfo<bool> returnInfo, DataDictionaryItemInfo item)
        {
            if (item == null)
            {
                return;
            }

            if (item.SystemInlay)
            {
                returnInfo.SetFailureMsg($"数据字典子项[{item.Text}]是系统内置不能删除");
                return;
            }
        }

        #endregion
    }
}
