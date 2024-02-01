namespace NB.Core;

public static class Const
{
    /// <summary>
    ///     实体生成位置
    /// </summary>
    public const string EntityPath = "\\NB.Core\\Entity";

    public const string EntityNamespace = "NB.Core.Entity";

    public const string DaoLayerPath = "\\NB.Core\\Dao";

    public const string DaoLayerNamespace = "NB.Core.Dao";

    public const string DtoPath = "\\NB.Core\\Dto";

    public const string DtoNamespace = "NB.Core.Dto";

    public const string ControllerPath = "\\NB.Web.Entry\\Apis";

    public const string ControllerNamespace = "NB.Web.Entry.Apis";

    public const string InterfacePath = "\\NB.Application\\IServices";

    /// <summary>
    ///     生成代码时忽略的接口
    /// </summary>
    public const string GenerateIgnoreInterfaces = "IGenerateService,ISystemService,ITest";

    /// <summary>
    ///     生成的请求dto后缀
    /// </summary>
    public const string GenerateRequestDtoSuffix = "Request";

    /// <summary>
    ///     生成的响应dto后缀
    /// </summary>
    public const string GenerateResponseDtoSuffix = "Response";


    /// <summary>
    ///     日期格式化
    /// </summary>
    public const string DateFormat = "yyyy-MM-dd";

    public const string DatetimeFormat = "yyyy-MM-dd HH:mm:ss";


    #region Redis-Key

    public const string ApiCachePrefixKey = "api_cache:"; //Api缓存key前缀

    #endregion
}