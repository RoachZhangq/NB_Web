using System;
using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using NB.Core.Options;
using System.Text;

namespace NB.Core.Common
{
    public static class ConsulBuilderExtensions
    {
        public static IApplicationBuilder RegisterConsul(this IApplicationBuilder app, IHostApplicationLifetime lifetime, ConsulOptions consulOption)
        {
            if (!consulOption.ConsulEnabled)
                return app;

            var consulClient = new ConsulClient(x =>
            {
                x.Address = new Uri(consulOption.Address);
            });

            var id = consulOption.ServiceName + "-" + consulOption.ServiceIP + ":" + consulOption.ServicePort;
            string tags = string.IsNullOrEmpty(consulOption.ServiceTags) ? consulOption.ServiceName :
                consulOption.ServiceTags;

            var registration = new AgentServiceRegistration()
            {
                //ID = Guid.NewGuid().ToString(),
                ID = id,
                Name = consulOption.ServiceName,// 服务名
                Address = consulOption.ServiceIP, // 服务绑定IP
                Port = consulOption.ServicePort, // 服务绑定端口
                Tags = tags.Split(","),//标签
                Check = new AgentServiceCheck()
                {
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),//服务启动多久后注册
                    Interval = TimeSpan.FromSeconds(4),//健康检查时间间隔
                    HTTP = consulOption.ServiceHealthCheck.Replace("{ServiceIP}", consulOption.ServiceIP).Replace("{ServicePort}", consulOption.ServicePort.ToString()),//健康检查地址
                    Timeout = TimeSpan.FromSeconds(5)
                }
            };

            // 服务注册
            consulClient.Agent.ServiceRegister(registration).Wait();

            // 应用程序终止时，服务取消注册
            lifetime.ApplicationStopping.Register(() =>
            {
                consulClient.Agent.ServiceDeregister(registration.ID).Wait();
            });

            //权重值
            if (Convert.ToInt32(consulOption.Weight) < 0)
            {
                var pair = new KVPair(id) { Value = Encoding.UTF8.GetBytes(consulOption.Weight) };
                consulClient.KV.Put(pair);
            }
            return app;
        }
    }
}

