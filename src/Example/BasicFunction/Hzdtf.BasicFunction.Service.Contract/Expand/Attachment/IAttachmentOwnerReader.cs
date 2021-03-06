using Hzdtf.BasicFunction.Model.Expand.Attachment;
using Hzdtf.Utility.Data;
using Hzdtf.Utility.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hzdtf.BasicFunction.Service.Contract.Expand.Attachment
{
    /// <summary>
    /// 附件归属读取接口
    /// @ 黄振东
    /// </summary>
    public partial interface IAttachmentOwnerReader
    {
        /// <summary>
        /// 根据归属类型读取附件归属信息
        /// </summary>
        /// <param name="type">归属类型</param>
        /// <param name="comData">通用数据</param>
        /// <returns>附件归属信息</returns>
        AttachmentOwnerInfo ReaderByOwnerType(short type, CommonUseData comData = null);
    }
}
