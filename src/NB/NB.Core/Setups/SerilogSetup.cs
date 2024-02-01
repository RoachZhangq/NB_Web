using System;
using Furion;
using Microsoft.Extensions.DependencyInjection;
using NB.Core.Common;
using NB.Core.Options;
using Serilog;
using Serilog.Events;

namespace NB.Core.Setups
{
    public static class SerilogSetup
    {
        public static void AddSerilogSetup(this IServiceCollection services)
        {
            #region 日志配置
            var serilogOptions = App.GetOptions<SerilogOptions>();
            LogHelper.Logger = new LoggerConfiguration()
              .MinimumLevel.Warning()
              .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
              .Enrich.WithMachineName()
              .Enrich.WithProcessId()
              .Enrich.WithThreadId()
              .Enrich.FromLogContext()
              .WriteTo.TencentCloud(
                 serilogOptions.AppName,
                 serilogOptions.WriteToTencentCloud.TopicId,
                 serilogOptions.WriteToTencentCloud.RequestBaseUri)
              .WriteTo.Console()
              .CreateLogger();
            #endregion
        }
    }
}

