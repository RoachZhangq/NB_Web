using NB.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NB.Core.Enums
{
    /// <summary>
    /// 参数日志输出枚举
    /// </summary>
    public enum ParamLogEnum
    {
        /// <summary>
        /// 输入参数
        /// </summary>
        [Description("输入参数")]
        INPUT,
        /// <summary>
        /// 输出参数
        /// </summary>
        [Description("输出参数")]
        OUTPUT,
        /// <summary>
        /// 输出输出参数
        /// </summary>
        [Description("输入输出参数")]
        ALL,
    }
}
