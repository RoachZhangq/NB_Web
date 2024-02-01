using Furion;
using Furion.Reflection;
using Furion.Reflection.Extensions;
using NB.Core.Attributes;
using NB.Core.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NB.Core.Common;

namespace NB.Core.Filter
{
    /// <summary>
    /// 自定义服务类拦截 Aop注册拦截
    /// AOP 是非常重要的思想和技术，也就是 面向切面 编程，可以让我们在不改动原来代码的情况下进行动态篡改业务代码。
    /// </summary>
    //public class LogDispatchProxy : AspectDispatchProxy, IGlobalDispatchProxy  // 全局拦截实现
    //拦截优先级: 贴[SuppressProxy] > [Injection(Proxy = typeof(LogDispatchProxy))] > 全局拦截
    public class CustomServiceIntercept : AspectDispatchProxy, IDispatchProxy
    {
        /// <summary>
        /// 当前服务实例
        /// </summary>
        public object Target { get; set; }

        /// <summary>
        /// 服务提供器，可以用来解析服务，如：Services.GetService()
        /// </summary>
        public IServiceProvider Services { get; set; }

        /// <summary>
        /// 拦截方法
        /// </summary>
        /// <param name="method"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public override object Invoke(MethodInfo method, object[] args)
        {

            #region 输入参数记录  
            //ParameLogAttribute 参数日志
            var invokeLog = method.GetActualCustomAttribute<ParameLogAttribute>(Target);
            string logPrefix = $"[方法调用日志] [Method]:{method.Name} ";
            StringBuilder log = new StringBuilder();
            if (invokeLog != null)
            {
                string inputLog = $"{logPrefix} [输入参数]: {JsonConvert.SerializeObject(args)}" +
                    $"\r\n";

                if (invokeLog.InvokeLogEnum == ParamLogEnum.INPUT)
                    LogHelper.WriteInfo(inputLog);
                else if (invokeLog.InvokeLogEnum == ParamLogEnum.ALL)
                    log.Append(inputLog);



            }

            #endregion

            #region 缓存
            //var invokeCache = method.GetActualCustomAttribute<CacheAttribute>(Target);
            //if (invokeCache != null)
            //{
            //    if (string.IsNullOrEmpty(invokeCache.Key))
            //        invokeCache.Key = method.DeclaringType.Name + ":" + method.Name;
            //    var _serviceStackRedis = App.GetService<ServiceStackRedis>();
            //    invokeCache.ReturnValue = "";
            //    //var cacheValue = _serviceStackRedis.Get(invokeCache.Key);
            //    //if (cacheValue != null)
            //    //{
            //    //    invokeCache.ReturnValue = cacheValue;
            //    //    return null;
            //    //}

            //}
            #endregion

            #region 调用方法
            var result = method.Invoke(Target, args);
            #endregion

            #region 输出参数记录
            if (invokeLog != null)
            {
                string outputLog = $"{logPrefix} [输出参数]: {JsonConvert.SerializeObject(result)}";
                if (invokeLog.InvokeLogEnum == ParamLogEnum.OUTPUT)
                    LogHelper.WriteInfo(outputLog);
                else if (invokeLog.InvokeLogEnum == ParamLogEnum.ALL)
                {
                    log.Append(outputLog);
                    LogHelper.WriteInfo(log.ToString());
                }
            }

            #endregion

            return result;
        }

        // 异步无返回值
        public override async Task InvokeAsync(MethodInfo method, object[] args)
        {
            //Console.WriteLine("SayHello 方法被调用了");

            var task = method.Invoke(Target, args) as Task;
            await task;

            //Console.WriteLine("SayHello 方法调用完成");
        }

        // 异步带返回值
        public override async Task<T> InvokeAsyncT<T>(MethodInfo method, object[] args)
        {
            //Console.WriteLine("SayHello 方法被调用了");

            var taskT = method.Invoke(Target, args) as Task<T>;
            var result = await taskT;

            //Console.WriteLine("SayHello 方法返回值：" + result);

            return result;
        }
    }
}
