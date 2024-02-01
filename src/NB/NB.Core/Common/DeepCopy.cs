using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace NB.Core.Common;

/// <summary>
///     深度拷贝
/// </summary>
/// <typeparam name="TIn"></typeparam>
/// <typeparam name="TOut"></typeparam>
/// <example>
///     <code>
///     var newObject = DeepCopy&lt;TIn, TOut&gt;.Trans(oldObject);
///  </code>
/// </example>
public static class DeepCopy<TIn, TOut>

{
    private static readonly Func<TIn, TOut> Cache = GetFunc();

    private static Func<TIn, TOut> GetFunc()
    {
        var parameterExpression = Expression.Parameter(typeof(TIn), "p");

        List<MemberBinding> memberBindingList = new();

        foreach (var item in typeof(TOut).GetProperties())

        {
            if (!item.CanWrite)

                continue;

            var property = Expression.Property(parameterExpression, typeof(TIn).GetProperty(item.Name));

            MemberBinding memberBinding = Expression.Bind(item, property);

            memberBindingList.Add(memberBinding);
        }


        var memberInitExpression =
            Expression.MemberInit(Expression.New(typeof(TOut)), memberBindingList.ToArray());

        var lambda =
            Expression.Lambda<Func<TIn, TOut>>(memberInitExpression, parameterExpression);

        return lambda.Compile();
    }


    public static TOut Trans(TIn tIn)
    {
        return Cache(tIn);
    }
}