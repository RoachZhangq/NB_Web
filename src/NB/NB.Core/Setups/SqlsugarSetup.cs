using System;
using Furion;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;
using System.Linq;
using NB.Core.Options;
using NB.Core.Common;
using Furion.LinqBuilder;

namespace NB.Core.Setups
{
    public static class SqlsugarSetup
    {
        public static void AddSqlsugarSetup(this IServiceCollection services)
        {
            var connectionConfig = App.GetOptions<ConnectionConfigOptions>();
            var connections = connectionConfig.Connections;
            if (connections.IsNullOrEmpty())
                return;

            #region 配置数据缓存服务
            var _serviceStackRedis = App.GetService<ServiceStackRedis>();
            foreach (var connection in connections)
            {
                connection.ConfigureExternalServices = new ConfigureExternalServices()
                {
                    DataInfoCacheService = new RedisCache(_serviceStackRedis)
                };
            }
            #endregion

            SqlSugarScope sqlSugar = new SqlSugarScope(
                connections
            , db =>
            {
                // 这里配置全局事件，比如拦截执行 SQL 
                db.Aop.OnLogExecuting = (sql, pars) =>
                {
                    Console.WriteLine(sql);
                    Console.WriteLine(string.Join(",", pars?.Select(it => it.ParameterName + ":" + it.Value)));
                    Console.WriteLine();
                    App.PrintToMiniProfiler("SqlSugar", "Info", sql + "\r\n" + db.Utilities.SerializeObject(pars.ToDictionary(it => it.ParameterName, it => it.Value)));
                };
            });
            //这边是SqlSugarScope用AddSingleton
            services.AddSingleton<ISqlSugarClient>(sqlSugar);
        }
    }
}


