using System.Collections.Generic;
using System.Linq;

namespace NB.Core.Extension;

public static class ListExtension
{
    public static List<T> ToPageList<T>(this List<T> source, int pageIndex, int pageSize, ref int totalCount,
        ref int totalPage)
    {
        var total = source.Count();
        totalCount = total;
        totalPage = total / pageSize;

        if (total % pageSize > 0)
            totalPage++;

        var list = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        return list;
    }

    public static List<T> ToPageList<T>(this List<T> source, int pageIndex, int pageSize, ref int totalCount)
    {
        var total = source.Count();
        totalCount = total;

        var list = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        return list;
    }
}