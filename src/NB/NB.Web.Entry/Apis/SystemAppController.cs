using Furion.SpecificationDocument;

namespace NB.Web.Entry.Apis;

/// <summary>
///     系统服务接口
/// </summary>
public class SystemAppService : IDynamicApiController
{
    private readonly ISystemService _systemService;

    /// <summary>
    ///     System
    /// </summary>
    /// <param name="systemService"></param>
    public SystemAppService(ISystemService systemService)
    {
        _systemService = systemService;
    }

    /// <summary>
    ///     获取系统描述
    /// </summary>
    /// <returns></returns>
    public string GetDescription()
    {
        return _systemService.GetDescription();
    }

    /// <summary>
    ///     swagger 检查
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [AllowAnonymous]
    [NonUnify]
    public int CheckUrl()
    {
        return 401;
    }

    /// <summary>
    ///     swagger 登录
    /// </summary>
    /// <param name="auth"></param>
    /// <returns></returns>
    [HttpPost]
    [AllowAnonymous]
    [NonUnify]
    public int SubmitUrl([FromForm] SpecificationAuth auth)
    {
        (var userName, var password) = _systemService.GetLoginInfo();

        if (auth.UserName == userName && auth.Password == password)
            return 200;
        return 401;
    }
}