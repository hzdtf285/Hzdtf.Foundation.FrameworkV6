using Hzdtf.BasicFunction.Model;
using Hzdtf.Utility.Model;
using Hzdtf.Utility.Model.Return;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Hzdtf.BasicFunction.Service.Contract
{
    /// <summary>
    /// 附件服务接口
    /// @ 黄振东
    /// </summary>
    public partial interface IAttachmentService
    {
        /// <summary>
        /// 上传
        /// </summary>
        /// <param name="attachments">附件信息列表</param>
        /// <param name="streams">文件流列表</param>
        /// <param name="comData">通用数据</param>
        /// <returns>返回信息</returns>
        ReturnInfo<bool> Upload(IList<AttachmentInfo> attachments, IList<Stream> streams, CommonUseData comData = null);

        /// <summary>
        /// 上传
        /// </summary>
        /// <param name="attachment">附件信息</param>
        /// <param name="stream">文件流列表</param>
        /// <param name="comData">通用数据</param>
        /// <returns>返回信息</returns>
        ReturnInfo<bool> Upload(AttachmentInfo attachment, Stream stream, CommonUseData comData = null);

        /// <summary>
        /// 根据归属查询附件列表
        /// </summary>
        /// <param name="ownerType">归属类型</param>
        /// <param name="ownerId">归属ID</param>
        /// <param name="blurTitle">模糊标题</param>
        /// <param name="connectionId">连接ID</param>
        /// <param name="comData">通用数据</param>
        /// <returns>返回信息</returns>
        ReturnInfo<IList<AttachmentInfo>> QueryByOwner(short ownerType, int ownerId, string blurTitle = null, CommonUseData comData = null, string connectionId = null);

        /// <summary>
        /// 根据归属统计附件个数
        /// </summary>
        /// <param name="ownerType">归属类型</param>
        /// <param name="ownerId">归属ID</param>
        /// <param name="blurTitle">模糊标题</param>
        /// <param name="connectionId">连接ID</param>
        /// <param name="comData">通用数据</param>
        /// <returns>返回信息</returns>
        ReturnInfo<int> CountByOwner(short ownerType, int ownerId, string blurTitle = null, CommonUseData comData = null, string connectionId = null);

        /// <summary>
        /// 根据归属是否存在附件
        /// </summary>
        /// <param name="ownerType">归属类型</param>
        /// <param name="ownerId">归属ID</param>
        /// <param name="blurTitle">模糊标题</param>
        /// <param name="connectionId">连接ID</param>
        /// <param name="comData">通用数据</param>
        /// <returns>返回信息</returns>
        ReturnInfo<bool> ExistsByOwner(short ownerType, int ownerId, string blurTitle = null, CommonUseData comData = null, string connectionId = null);

        /// <summary>
        /// 根据归属移除
        /// </summary>
        /// <param name="ownerType">归属类型</param>
        /// <param name="ownerId">归属ID</param>
        /// <param name="connectionId">连接ID</param>
        /// <param name="comData">通用数据</param>
        /// <returns>返回信息</returns>
        ReturnInfo<bool> RemoveByOwner(short ownerType, int ownerId, CommonUseData comData = null, string connectionId = null);
    }
}
