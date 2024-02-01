using System.Reflection;

namespace NB.Web.Entry;

/// <summary>
///     SingleFilePublish
/// </summary>
public class SingleFilePublish : ISingleFilePublish
{
    /// <summary>
    ///     IncludeAssemblies
    /// </summary>
    /// <returns></returns>
    public Assembly[] IncludeAssemblies()
    {
        return Array.Empty<Assembly>();
    }

    /// <summary>
    ///     IncludeAssemblyNames
    /// </summary>
    /// <returns></returns>
    public string[] IncludeAssemblyNames()
    {
        return new[]
        {
            "NB.Application",
            "NB.Core",
            "NB.Web.Core"
        };
    }
}