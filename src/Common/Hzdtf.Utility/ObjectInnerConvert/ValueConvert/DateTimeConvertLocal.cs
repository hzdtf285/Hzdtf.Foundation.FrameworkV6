using Hzdtf.Utility.Attr;
using Hzdtf.Utility.Conversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hzdtf.Utility.ObjectInnerConvert.ValueConvert
{
    /// <summary>
    /// 日期时间转换为本地日期时间
    /// @ 黄振东
    /// </summary>
    [Inject]
    public class DateTimeConvertLocal : ConvertBase
    {
        /// <summary>
        /// 哪些类型需要转换为本地时间列表
        /// </summary>
        public static readonly IList<DateTimeKind> NeedToLocalFromKinds = new List<DateTimeKind>(2)
        {
            DateTimeKind.Unspecified,
            DateTimeKind.Utc,
        };

        /// <summary>
        /// 转换新值
        /// </summary>
        /// <param name="value">值</param>
        /// <returns>新值</returns>
        protected override object ToNew(object value)
        {
            if (value is DateTime || value is DateTime?)
            {
                var time = (DateTime)value;
                if (time.Kind != DateTimeKind.Local && NeedToLocalFromKinds.Contains(time.Kind))
                {
                    return time.ToLocalTime();
                }
            }

            return value;
        }
    }
}
