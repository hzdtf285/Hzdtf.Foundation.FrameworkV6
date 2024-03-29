﻿using Hzdtf.BasicFunction.Model.Expand.Attachment;
using Hzdtf.BasicFunction.Service.Contract.Expand.Attachment;
using Hzdtf.Service.Impl;
using Hzdtf.Utility.Attr;
using Hzdtf.Utility.Model.Return;
using System;
using System.Collections.Generic;
using System.Text;
using Hzdtf.Utility.Utils;
using Hzdtf.Utility.Model;
using Hzdtf.Logger.Contract;

namespace Hzdtf.BasicFunction.Service.Impl.Expand.Attachment
{
    /// <summary>
    /// 附件存储
    /// @ 黄振东
    /// </summary>
    [Inject]
    public partial class AttachmentStore : BasicServiceBase, IAttachmentStore
    {
        /// <summary>
        /// 文件根路径
        /// </summary>
        private static string fileRoot;

        /// <summary>
        /// 同步文件根路径
        /// </summary>
        private static readonly object syncFileRoot = new object();

        /// <summary>
        /// 同步创建文件夹
        /// </summary>
        private static readonly object syncCreateRoot = new object();

        /// <summary>
        /// 文件根路径
        /// </summary>
        public string FileRoot
        {
            get
            {
                if (fileRoot == null)
                {
                    string rootType = Config["Attachment:UploadRootType"];
                    switch (rootType)
                    {
                        case "virtual":
                            lock (syncFileRoot)
                            {
                                fileRoot = $"{AppContext.BaseDirectory}{Config["Attachment:UploadRoot"]}";
                            }

                            break;

                        default:
                            lock (syncFileRoot)
                            {
                                fileRoot = Config["Attachment:UploadRoot"];
                            }

                            break;
                    }
                }


                return fileRoot;
            }
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="log">日志</param>
        public AttachmentStore(ILogable log = null)
            : base(log) { }

        /// <summary>
        /// 上传
        /// </summary>
        /// <param name="comData">通用数据</param>
        /// <param name="attachmentStream">附件流</param>
        /// <returns>返回信息</returns>
        [ProcTrackLog(IgnoreParamValues = true)]
        public virtual ReturnInfo<IList<string>> Upload(CommonUseData comData = null, params AttachmentStreamInfo[] attachmentStream)
        {
            // 以当前年月为目录
            string yearMonthDic = $"{DateTimeExtensions.Now.ToCompactShortYM()}/";
            string dic = $"{FileRoot}{yearMonthDic}";
            lock (syncCreateRoot)
            {
                dic.CreateNotExistsDirectory();
            }

            ReturnInfo<IList<string>> returnInfo = new ReturnInfo<IList<string>>();
            returnInfo.Data = new List<string>(attachmentStream.Length);

            try
            {
                foreach (var attStream in attachmentStream)
                {
                    string expandName = attStream.FileName.FileExpandName();
                    string newFileName = $"{StringUtil.NewShortGuid()}{expandName}";

                    $"{dic}{newFileName}".WriteFile(attStream.Stream);
                    returnInfo.Data.Add($"{Config["Attachment:DownloadRoot"]}{yearMonthDic}{newFileName}");
                }
            }
            catch (Exception ex)
            {
                log.ErrorAsync(ex.Message, ex, this.GetType().FullName);
                returnInfo.SetFailureMsg(ex.Message, ex.StackTrace, ex);
            }

            return returnInfo;
        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="comData">通用数据</param>
        /// <param name="fileAddress">文件地址</param>
        /// <returns>返回信息</returns>
        public virtual ReturnInfo<bool> Remove(CommonUseData comData = null, params string[] fileAddress)
        {
            ReturnInfo<bool> returnInfo = new ReturnInfo<bool>();
            try
            {
                foreach (string f in fileAddress)
                {
                    // 替换虚拟路径
                    string newF = f.Replace(Config["Attachment:DownloadRoot"], null);
                    $"{FileRoot}{newF}".DeleteFile();
                }
            }
            catch (Exception ex)
            {
                log.ErrorAsync(ex.Message, ex, this.GetType().FullName);
                returnInfo.SetFailureMsg(ex.Message, ex.StackTrace, ex);
            }

            return returnInfo;
        }
    }
}
