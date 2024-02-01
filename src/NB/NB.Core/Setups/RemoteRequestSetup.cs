using System;
using Furion;
using Microsoft.Extensions.DependencyInjection;
using NB.Core.Options;

namespace NB.Core.Setups;

public static class RemoteRequestSetup
{
    public static void AddRemoteRequestSetup(this IServiceCollection services)
    {
        #region 远程服务配置

        var remoteRequest = App.GetOptions<RemoteRequestOptions>();
        remoteRequest.Clients = remoteRequest.Clients.FindAll(o => !string.IsNullOrWhiteSpace(o.Name));

        if (remoteRequest.Clients.Count > 0)
            services.AddRemoteRequest(options =>
            {
                foreach (var client in remoteRequest.Clients)
                    options.AddHttpClient(client.Name, c => { c.BaseAddress = new Uri(client.BaseAddress); });
            });
        else
            services.AddHttpClient();

        #endregion
    }
}