using Hzdtf.Service.Contract;
using Hzdtf.Utility.Model;
using Hzdtf.Utility.Model.Return;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Hzdtf.Logger.Contract;
using Hzdtf.Utility.Factory;
using Hzdtf.Utility.Localization;
using Hzdtf.Utility.RoutePermission;

namespace Hzdtf.BasicController
{
    /// <summary>
    /// 管理控制器基类
    /// @ 黄振东
    /// </summary>
    /// <typeparam name="IdT">Id类型</typeparam>
    /// <typeparam name="ModelT">模型类型</typeparam>
    /// <typeparam name="ServiceT">服务类型</typeparam>
    /// <typeparam name="PageInfoT">页面信息类型</typeparam>
    public abstract partial class ManageControllerBase<IdT, ModelT, ServiceT, PageInfoT> : PageDataControllerBase<IdT, ModelT, ServiceT, PageInfoT>
        where ModelT : SimpleInfo<IdT>
        where ServiceT : IService<IdT, ModelT>
        where PageInfoT : PageInfo<IdT>
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="log">日志</param>
        /// <param name="service">服务</param>
        /// <param name="localize">本地化</param>
        /// <param name="comUseDataFactory">通用数据工厂</param>
        public ManageControllerBase(ILogable log = null, ServiceT service = default(ServiceT), ILocalization localize = null, ISimpleFactory<HttpContext, CommonUseData> comUseDataFactory = null)
            : base(log, service, localize, comUseDataFactory)
        {
        }

        /// <summary>
        /// 根据ID查找模型
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>返回信息</returns>
        [HttpGet("{id}")]
        [ActionPermission(new string[] { "Query" })]
        public virtual ReturnInfo<ModelT> Get(IdT id) => service.Find(id, comUseDataFactory.Create(HttpContext));

        /// <summary>
        /// 添加模型
        /// </summary>
        /// <param name="model">模型</param>
        /// <returns>返回信息</returns>
        [HttpPost]
        [ActionPermission(new string[] { "Add" })]
        public virtual ReturnInfo<bool> Post(ModelT model) => service.Add(model, comUseDataFactory.Create(HttpContext));

        /// <summary>
        /// 修改模型
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="model">模型</param>
        /// <returns>返回信息</returns>
        [HttpPut("{id}")]
        [ActionPermission(new string[] { "Edit" })]
        public virtual ReturnInfo<bool> Put(IdT id, ModelT model)
        {
            model.Id = id;

            return service.ModifyById(model, comUseDataFactory.Create(HttpContext));
        }

        /// <summary>
        /// 移除模型
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>返回信息</returns>
        [HttpDelete("{id}")]
        [ActionPermission(new string[] { "Remove" })]
        public virtual ReturnInfo<bool> Delete(IdT id) => service.RemoveById(id, comUseDataFactory.Create(HttpContext));

        /// <summary>
        /// 批量添加模型列表
        /// </summary>
        /// <param name="models">模型列表</param>
        /// <returns>返回信息</returns>
        [HttpPost("BatchAdd")]
        [ActionPermission(new string[] { "Add" })]
        public virtual ReturnInfo<bool> BatchAdd(IList<ModelT> models) => service.Add(models, comUseDataFactory.Create(HttpContext));

        /// <summary>
        /// 根据ID集合批量移除模型
        /// </summary>
        /// <param name="ids">ID集合</param>
        /// <returns>返回信息</returns>
        [HttpDelete("BatchRemove")]
        [ActionPermission(new string[] { "Remove" })]
        public virtual ReturnInfo<bool> BatchRemove(IdT[] ids) => service.RemoveByIds(ids, comUseDataFactory.Create(HttpContext));

        /// <summary>
        /// 统计模型数量
        /// </summary>
        /// <returns>返回信息</returns>
        [HttpDelete("Count")]
        [ActionPermission(new string[] { "Query" })]
        public virtual ReturnInfo<int> Count() => service.Count(comUseDataFactory.Create(HttpContext));

        /// <summary>
        /// 根据ID获取是否存在模型
        /// </summary>
        /// <returns>返回信息</returns>
        [HttpGet("Exists/{id}")]
        [ActionPermission(new string[] { "Query" })]
        public virtual ReturnInfo<bool> Exists(IdT id) => service.Exists(id, comUseDataFactory.Create(HttpContext));
    }
}
