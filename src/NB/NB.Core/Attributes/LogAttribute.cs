using NB.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NB.Core.Attributes
{
    /// <summary>
    /// 参数日志
    /// </summary>
    public class ParameLogAttribute : Attribute
    {
        public ParamLogEnum InvokeLogEnum { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="invokeLogEnum"></param>
        public ParameLogAttribute(ParamLogEnum invokeLogEnum)
        {
            this.InvokeLogEnum = invokeLogEnum;
        }
    }

}
