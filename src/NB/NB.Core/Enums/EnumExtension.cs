using System;
using System.Collections.Generic;
using System.Linq;

namespace NB.Core.Enums;

/// <summary>
///     枚举 扩展
/// </summary>
public static class EnumExtension
{
    public static string GetDescription(this Enum em)
    {
        var type = em.GetType();
        var fd = type.GetField(em.ToString());
        if (fd == null)
            return string.Empty;
        var attrs = fd.GetCustomAttributes(typeof(DescriptionAttribute), false);
        var name = string.Empty;
        foreach (DescriptionAttribute attr in attrs) name = attr.Name;
        return name;
    }

    /// <summary>
    ///     获取枚举类集合
    /// </summary>
    /// <param name="namespaceName">命名空间</param>
    /// <returns></returns>
    public static Type[] GetAllEnumsInNamespace(string namespaceName)
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => p.IsEnum && p.Namespace == namespaceName)
            .ToArray();
    }

    /// <summary>
    ///     枚举转字典集合(Key是value,Value是description（如果不存在description 则是name）)
    /// </summary>
    /// <param name="enumType">枚举类</param>
    /// <returns>返回生成的字典集合</returns>
    public static List<object> EnumToDictionary(Type enumType)
    {
        var list = new List<object>();

        if (!enumType.IsEnum) return list;
        // if (!string.IsNullOrEmpty(keyDefault)) //判断是否添加默认选项
        //     dicEnum.Add(keyDefault, valueDefault);
        var fieldstrs = Enum.GetNames(enumType); //获取枚举字段数组
        foreach (var item in fieldstrs)
        {
            var description = string.Empty;
            var field = enumType.GetField(item);
            var arr = field.GetCustomAttributes(typeof(DescriptionAttribute), true); //获取属性字段数组
            if (arr != null && arr.Length > 0)
                description = ((DescriptionAttribute)arr[0]).Name; //属性描述
            else
                description = item; //描述不存在取字段名称

            //不用枚举的value值作为字典key值的原因从枚举例子能看出来，其实这边应该判断他的值不存在，默认取字段名称

            list.Add(new { Title = description, Value = Enum.Parse(enumType, item) });
        }

        return list;
    }
}

/// <summary>
///     描述
/// </summary>
public class DescriptionAttribute : Attribute
{
    public DescriptionAttribute(string name)
    {
        Name = name;
    }

    public string Name { get; set; }
}