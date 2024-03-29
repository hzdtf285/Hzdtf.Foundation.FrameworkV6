﻿using Hzdtf.BasicFunction.Model.Expand.Attachment;
using Hzdtf.BasicFunction.Service.Contract.Expand.Attachment;
using Hzdtf.Utility.Attr;
using Hzdtf.Utility.Cache;
using Hzdtf.Utility.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Hzdtf.BasicFunction.Service.Impl.Expand.Attachment
{
    /// <summary>
    /// 附件归属本地内存
    /// @ 黄振东
    /// </summary>
    [Inject]
    public class AttachmentOwnerLocalMember : SingleTypeLocalMemoryBase<short, AttachmentOwnerInfo>, IAttachmentOwnerReader
    {
        #region 属性与字段

        /// <summary>
        /// 字典缓存
        /// </summary>
        private static readonly IDictionary<short, AttachmentOwnerInfo> dicCache = new ConcurrentDictionary<short, AttachmentOwnerInfo>();

        /// <summary>
        /// 原生附件归属读取
        /// </summary>
        public IAttachmentOwnerReader ProtoAttachmentOwnerReader
        {
            get;
            set;
        }

        #endregion

        #region IAttachmentOwnerReader 接口

        /// <summary>
        /// 根据归属类型读取附件归属信息
        /// </summary>
        /// <param name="type">归属类型</param>
        /// <param name="comData">通用数据</param>
        /// <returns>附件归属信息</returns>
        public AttachmentOwnerInfo ReaderByOwnerType(short type, CommonUseData comData = null)
        {
            if (dicCache.ContainsKey(type))
            {
                return dicCache[type];
            }

            AttachmentOwnerInfo AttachmentOwnerInfo = ProtoAttachmentOwnerReader.ReaderByOwnerType(type, comData);
            if (AttachmentOwnerInfo == null)
            {
                return null;
            }

            Add(type, AttachmentOwnerInfo);

            return AttachmentOwnerInfo;
        }

        #endregion

        #region 需要子类重写的方法

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <returns>缓存</returns>
        protected override IDictionary<short, AttachmentOwnerInfo> GetCache() => dicCache;

        #endregion
    }
}
