using System;
using Microsoft.Extensions.DependencyInjection;
using NB.Core.Options;

namespace NB.Core.Setups
{
    public static class OptionsSetup
    {
        public static void AddOptionsSetup(this IServiceCollection services)
        {
            #region 配置注册
            services.AddConfigurableOptions<ConnectionConfigOptions>();
            services.AddConfigurableOptions<SerilogOptions>();
            services.AddConfigurableOptions<ParameterOptions>();
            services.AddConfigurableOptions<RemoteRequestOptions>();
            services.AddConfigurableOptions<RedisOptions>();
            services.AddConfigurableOptions<ConsulOptions>();
            //services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();//注入HttpContextAccessor
            #endregion
        }

    }
}

