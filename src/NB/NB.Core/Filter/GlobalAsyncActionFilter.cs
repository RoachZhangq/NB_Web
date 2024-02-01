
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NB.Core.Common;
using Furion;
using NB.Core.Attributes;
using Microsoft.AspNetCore.Mvc.Controllers;
using NB.Core.Enums;

namespace NB.Core.Filter
{
    public class GlobalAsyncActionFilter : IAsyncActionFilter
    {
        public static bool IsFirstRun = true;

        public static string Redis_ZSet_RequestErrorRanking_Key = "Ranking:RequestErrorRanking";

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            #region 拦截之前
            string path = context.HttpContext.Request.Path;
            string method = context.HttpContext.Request.Method;
            var actionArgumentsStr = JsonConvert.SerializeObject(context.ActionArguments);
            var _serviceStackRedis = App.GetService<ServiceStackRedis>();
            if (IsFirstRun)
            {
                RankingClear(_serviceStackRedis);
                IsFirstRun = false;
            }

            //查找日志过滤器
            var controllerActionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;

            var parameLog = controllerActionDescriptor.MethodInfo.GetCustomAttributes(typeof(ParameLogAttribute), false).FirstOrDefault() as ParameLogAttribute;

            #endregion
            var resultContext = await next();

            #region 拦截之后                        
            try
            {
                var jsonResult = resultContext.Result as Microsoft.AspNetCore.Mvc.JsonResult;
                if (jsonResult != null)
                {
                    string resultStr = JsonConvert.SerializeObject(jsonResult.Value);
                    if (parameLog != null)
                    {
                        string log = string.Empty;
                        switch (parameLog.InvokeLogEnum)
                        {
                            case ParamLogEnum.INPUT:
                                log += $"[请求] Path:{path} Method{method} \r\n ActionArguments:{actionArgumentsStr}";
                                break;
                            case ParamLogEnum.OUTPUT:
                                log += $"\r\n [响应] {resultStr}";
                                break;
                            case ParamLogEnum.ALL:
                                log += $"[请求] Path:{path} Method{method} \r\n ActionArguments:{actionArgumentsStr}";
                                log += $"\r\n [响应] {resultStr}";
                                break;
                            default:
                                break;
                        }
                        if (!string.IsNullOrEmpty(log))
                            LogHelper.WriteWarning(log);
                    }
                }

            }
            catch (Exception)
            {

            }
            #endregion

            #region 异常拦截
            if (resultContext.Exception != null)
            {
                #region 记录错误日志
                //string err = resultContext.Exception.ToString();
                var index = resultContext.Exception.StackTrace.IndexOf("--- End of stack trace from previous location ---");
                string stackTrace = string.Empty;
                if (index > -1)
                    stackTrace = resultContext.Exception.StackTrace.Substring(0, index);
                else
                    stackTrace = resultContext.Exception.StackTrace;

                #region 异常行号
                int errLineNumber = 0;
                try
                {
                    System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(resultContext.Exception, true);
                    errLineNumber = trace.GetFrame(0).GetFileLineNumber();
                }
                catch (Exception)
                {
                    errLineNumber = -1;
                }
                #endregion

                string log = $"[异常日志] RequestPath: {path}" +
                    $"\r\n" +
                    $"[Exception.Message]: {resultContext.Exception.Message}" +
                    $"\r\n" +
                    $"[Exception.StackTrace]: {stackTrace}" +
                    $"\r\n" +
                    $"[Exception.LineNumber]: {errLineNumber}";
                LogHelper.WriteError(log);
                #endregion

                RankingIncrement(_serviceStackRedis, path, 1);
            }
            #endregion
        }

        /// <summary>
        /// 清空排名榜
        /// </summary>
        /// <param name="serviceStackRedis"></param>
        private void RankingClear(ServiceStackRedis serviceStackRedis)
        {
            try
            {
                LogHelper.WriteInfo("RankingClear");
                serviceStackRedis.RemoveRangeFromSortedSet(Redis_ZSet_RequestErrorRanking_Key, 0, int.MaxValue);
            }
            catch (Exception)
            {


            }

        }
        /// <summary>
        /// 排名榜加减Score
        /// </summary>
        /// <param name="serviceStackRedis"></param>
        /// <param name="path"></param>
        /// <param name="score"></param>
        private void RankingIncrement(ServiceStackRedis serviceStackRedis, string path, long score)
        {
            try
            {
                serviceStackRedis.IncrementItemInSortedSet(Redis_ZSet_RequestErrorRanking_Key, path, score);
                //serviceStackRedis.AddItemToSortedSet("Add_" + Redis_ZSet_RequestRanking_Key, path, score);
            }
            catch (Exception)
            {


            }
        }
    }
}
