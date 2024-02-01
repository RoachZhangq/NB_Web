using Furion;
using Furion.SpecificationDocument;
using IGeekFan.AspNetCore.Knife4jUI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using NB.Core;
using NB.Core.Common;
using NB.Core.Filter;
using NB.Core.Options;
using NB.Core.Setups;

namespace NB.Web.Core;

public class Startup : AppStartup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddOptionsSetup(); //配置文件映射

        services.AddConsoleFormatter(); //控制台格式化

        //services.AddJwt<JwtHandler>(enableGlobalAuthorize: true);  //.AddJwt() 必须在.AddControllers() 之前注册

        services.AddCorsAccessor(); //跨域


        services.AddControllers(options =>
            {
                options.Filters.Add(typeof(GlobalAsyncActionFilter), int.MinValue); //过滤器
            })
            .AddDataValidation() //注册验证服务
            .AddFriendlyException() //友好的异常服务
            .AddNewtonsoftJson(options => //遇到过的问题:IIS8部署 响应的长度受限制
            {
                options.SerializerSettings.DateFormatString = Const.DatetimeFormat; //日期格式
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore; //忽略循环引用
                options.SerializerSettings.Converters.AddLongTypeConverters(); //long 类型序列化时转 string
                options.SerializerSettings.Converters.AddDateOnlyConverters(); // DateOnly
                options.SerializerSettings.Converters.AddTimeOnlyConverters(); // TimeOnly
                //options.SerializerSettings.ContractResolver = new DefaultContractResolver();//原样式输出
            })
            /*.AddJsonOptions(options =>//遇到过的问题:ApiCache缓存有问题
             {
                options.JsonSerializerOptions.Converters.AddDateTimeTypeConverters(Const.DatetimeFormat);//日期格式
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;//忽略循环引用
                //options.JsonSerializerOptions.IncludeFields = true;//包含成员字段序列化
                //options.JsonSerializerOptions.AllowTrailingCommas = true; //允许尾随逗号
                //options.JsonSerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;//允许注释
                //options.JsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;//处理乱码问题
                // options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;//不区分大小写
                options.JsonSerializerOptions.Converters.AddLongTypeConverters();//long 类型序列化时转 string
                options.JsonSerializerOptions.Converters.AddDateOnlyConverters();   // DateOnly
                options.JsonSerializerOptions.Converters.AddTimeOnlyConverters();   // TimeOnly
                //options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;//忽略null的字段
             })*/
            .AddInjectWithUnifyResult(); //规范化结果

        services.AddSerilogSetup(); //日志配置

        services.AddRemoteRequestSetup(); //远程访问

        services.AddSqlsugarSetup(); //SqlSugar

        services.AddEventBus(); //事件总线

        services.AddMvcFilter<RequestAuditFilter>(); //审计日志
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime)
    {
        if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

        //白名单
        //app.UseMiddleware<AdminSafeListMiddleware>("127.0.0.1;192.168.1.5;::1");

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseCorsAccessor();

        app.UseAuthentication();

        app.UseAuthorization();

        app.UseKnife4UI(options =>
        {
            options.RoutePrefix = "newapi"; // 配置 Knife4UI 路由地址
            foreach (var groupInfo in SpecificationDocumentBuilder.GetOpenApiGroups())
                options.SwaggerEndpoint("/" + groupInfo.RouteTemplate, groupInfo.Title);
        });

        app.UseInject(string.Empty);

        app.RegisterConsul(lifetime, App.GetOptions<ConsulOptions>()); //注册Consul        

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}