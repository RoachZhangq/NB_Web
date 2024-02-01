namespace NB.Core.Dto;

public class PagedBaseReq
{
    /// <summary>
    ///     当前页数
    /// </summary>
    public int PageIndex { get; set; } = 1;

    /// <summary>
    ///     每页显示条数
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    ///     排序字段
    /// </summary>
    public string OrderByField { get; set; }

    /// <summary>
    ///     模糊查询参数
    /// </summary>
    public string Keyword { get; set; }
}