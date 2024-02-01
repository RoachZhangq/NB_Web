using Furion;
using Furion.UnifyResult;
using NB.Core.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NB.Core.Filter
{
    /// <summary>
    /// 接口缓存
    /// </summary>
    public class ApiCache : ActionFilterAttribute
    {
        //参考 https://www.cnblogs.com/lyps/p/13679954.html

        /// <summary>
        /// Header是否参与缓存验证
        /// </summary>
        public bool SignHeader = false;
        /// <summary>
        /// 缓存有效时间（分钟）
        /// </summary>
        public uint CacheMinutes = 5;

        /// <summary>
        /// 自定义缓存Key
        /// </summary>
        public string CustomCacheKey = string.Empty;



        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheMinutes">缓存有效时间（分钟）</param>
        /// <param name="customCacheKey">自定义缓存Key</param>
        /// <param name="signHeader">Header是否参与请求体签名</param> 
        public ApiCache(uint cacheMinutes, string customCacheKey = "", bool signHeader = false)
        {
            SignHeader = signHeader;
            CustomCacheKey = customCacheKey;
            CacheMinutes = cacheMinutes;
        }


        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var actionName = filterContext.RouteData.Values["action"].ToString();
            // actionName = (filterContext.ActionDescriptor as Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor).ActionName;
            var controllerName = filterContext.RouteData.Values["controller"].ToString();

            //请求体签名
            string cacheKey = getKey(filterContext.HttpContext.Request, controllerName, actionName);
            //根据签名查询缓存
            var _serviceStackRedis = App.GetService<ServiceStackRedis>();
            string data = _serviceStackRedis.Get<string>(cacheKey);
            if (!string.IsNullOrWhiteSpace(data))
            {
                //有缓存则设置返回信息
                //var content = new Microsoft.AspNetCore.Mvc.ContentResult();
                //content.Content = data;
                //content.ContentType = "application/json; charset=utf-8";
                //content.StatusCode = 200;

                var obj = JsonConvert.DeserializeObject<RESTfulResult<object>>(data);
                var content = new Microsoft.AspNetCore.Mvc.JsonResult(obj.Data);
                content.ContentType = "application/json; charset=utf-8";
                content.StatusCode = 200;
                UnifyContext.Fill(obj.Extras);

                filterContext.HttpContext.Response.Headers.Add("Content-Type", "application/json; charset=utf-8");
                filterContext.HttpContext.Response.Headers.Add("CacheData", "Redis");
                filterContext.Result = content;
            }
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            base.OnResultExecuting(filterContext);
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            if (filterContext.HttpContext.Response.Headers.ContainsKey("CacheData")) return;

            var actionName = filterContext.RouteData.Values["action"].ToString();
            var controllerName = filterContext.RouteData.Values["controller"].ToString();
            //获取缓存key
            string cacheKey = getKey(filterContext.HttpContext.Request, controllerName, actionName);
            var result = filterContext.Result as Microsoft.AspNetCore.Mvc.JsonResult;
            var data = JsonConvert.SerializeObject(result.Value, new JsonSerializerSettings
            {
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            });
            //如果缓存null，则设置较短过期时间（此处是防止缓存穿透）
            var disData = JsonConvert.DeserializeObject<Dictionary<string, object>>(data);
            if (disData.ContainsKey("data") && disData["data"] == null)
            {
                CacheMinutes = 1;
            }
            var _serviceStackRedis = App.GetService<ServiceStackRedis>();
            _serviceStackRedis.Set(cacheKey, data, Convert.ToInt32(CacheMinutes * 60));
        }
        /// <summary>
        /// 请求体MDH签名
        /// </summary>
        /// <param name="request"></param>
        /// <param name="controllerName"></param>
        /// <param name="actionName"></param>
        /// <returns></returns>
        private string getKey(HttpRequest request, string controllerName, string actionName)
        {

            if (!string.IsNullOrEmpty(CustomCacheKey))
                return Const.ApiCachePrefixKey + CustomCacheKey;

            var keyContent = request.Host.Value + request.Path.Value + request.QueryString.Value + request.Method + request.ContentType + request.ContentLength;
            try
            {
                if (request.Method.ToUpper() != "DELETE" && request.Method.ToUpper() != "GET" && request.Form.Count > 0)
                {
                    foreach (var item in request.Form)
                    {
                        keyContent += $"{item.Key}={item.Value.ToString()}";
                    }
                }
            }
            catch (Exception)
            {

            }
            if (SignHeader)
            {
                var hs = request.Headers.Where(a => !(new string[] { "Postman-Token", "User-Agent" }).Contains(a.Key)).ToDictionary(a => a);
                foreach (var item in hs)
                {
                    keyContent += $"{item.Key}={item.Value.ToString()}";
                }
            }
            //md5加密
            string key = MD5Helper.MD5Encrypt32(keyContent);
            return $"{Const.ApiCachePrefixKey}{controllerName}:{actionName}:{key}";
            //return Const.ApiCachePrefixKey + controllerName + ":" + actionName + ":" + MD5Helper.MD5Encrypt32(keyContent);
        }
    }
}
