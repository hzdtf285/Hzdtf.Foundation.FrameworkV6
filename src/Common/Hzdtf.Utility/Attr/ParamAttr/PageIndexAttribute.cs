using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Hzdtf.Utility.Attr.ParamAttr
{
    /// <summary>
    /// 页码特性
    /// @ 黄振东
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class PageIndexAttribute : ValidationAttribute
    {
    }
}
