using Hzdtf.Utility.Model.Page;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hzdtf.Utility.Utils
{
    /// <summary>
    /// 分页辅助类
    /// @ 黄振东
    /// </summary>
    public static class PagingUtil
    {
        /// <summary>
        /// 计算分页总数
        /// </summary>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="total">总数</param>
        /// <returns>分页总数</returns>
        public static int PageCount(int pageSize, int total)
        {
            if (pageSize <= 0)
            {
                return 0;
            }
            int pageCount = total / pageSize;
            if (total % pageSize != 0)
            {
                pageCount++;
            }

            return pageCount;
        }

        /// <summary>
        /// 计算分页开始结束数
        /// </summary>
        /// <param name="pageIndex">页码，从0开始</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="baseStartNum">基层开始数</param>
        /// <returns>分页开始结束数</returns>
        public static int[] PageStartEnd(int pageIndex, int pageSize, int baseStartNum = 0)
        {
            int[] result = new int[2];
            result[0] = pageIndex > 0 ? pageIndex * pageSize : pageIndex;
            result[0] += baseStartNum;
            result[1] = result[0] + pageSize;

            return result;
        }

        /// <summary>
        /// 按指定数据类型执行分页函数
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="countFunc">统计数函数</param>
        /// <param name="selectPageFunc">查询分页函数，输入参数：推荐页码</param>
        /// <returns>分页信息</returns>
        public static PagingInfo<DataT> ExecPage<DataT>(int pageIndex, int pageSize, Func<int> countFunc, Func<int, IList<DataT>> selectPageFunc)
        {
            var pagingInfo = new PagingInfo<DataT>();
            pagingInfo.PageIndex = pageIndex;
            pagingInfo.PageSize = pageSize;
            // 先执行统计，如果<1则不用再往下查询，提高性能
            int count = countFunc();
            if (count < 1)
            {
                return pagingInfo;
            }

            pagingInfo.Records = count;

            var maxPageIndex = pagingInfo.PageCount - 1;
            if (pageIndex >= maxPageIndex)
            {
                pagingInfo.PageIndex = maxPageIndex;
            }
            pagingInfo.Rows = selectPageFunc(pagingInfo.PageIndex);

            return pagingInfo;
        }

        /// <summary>
        /// 循环分页
        /// </summary>
        /// <param name="callback">回调。0：页码，1：每页记录数，2：返回总页数</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="maxForCount">最大循环次数</param>
        public static void ForPage(Func<int, int, int> callback, int pageIndex = 0, int pageSize = 500, int maxForCount = 10000)
        {
            for (; pageIndex < maxForCount; pageIndex++)
            {
                var pageCount = callback(pageIndex, pageSize);
                if (pageIndex < pageCount - 1)
                {
                    continue;
                }

                return;
            }
        }
    }
}
