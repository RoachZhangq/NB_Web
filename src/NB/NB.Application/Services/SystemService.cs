using NB.Application.IServices;

namespace NB.Application.Services;

public class SystemService : ISystemService, ITransient
{
    public string GetDescription()
    {
        return "让 .NET 开发更简单，更通用，更流行。";
    }

    public (string userName, string password) GetLoginInfo()
    {
        // 读取配置信息
        var userName = App.Configuration["SpecificationDocumentSettings:LoginInfo:UserName"];
        var password = App.Configuration["SpecificationDocumentSettings:LoginInfo:Password"];
        return (userName, password);
    }
}
