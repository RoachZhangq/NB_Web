using NB.Core.Enums;
using NB.Core.Filter;

namespace NB.Web.Entry.Apis;

/// <summary>
///     枚举
/// </summary>
[ApiDescriptionSettings("Enum-枚举")]
[AllowAnonymous]
public class EnumController : IDynamicApiController
{
    /// <summary>
    ///     获取枚举字典
    /// </summary>
    /// <param name="enumName"></param>
    /// <returns></returns>
    [ApiCache(10)]
    public async Task<List<object>> GetList(string enumName)
    {
        var dictionary = new List<object>();
        var enums = EnumExtension.GetAllEnumsInNamespace(typeof(EnumExtension).Namespace);

        foreach (var enumType in enums)
            if (enumName == enumType.Name)
            {
                dictionary = EnumExtension.EnumToDictionary(enumType);
                break;
            }

        return await Task.FromResult(dictionary);
    }
}