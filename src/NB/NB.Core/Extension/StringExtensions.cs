using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace NB.Core.Extension;

public static class StringExtensions
{
    /// <summary>
    ///     截取字符串
    /// </summary>
    /// <param name="str">字符串</param>
    /// <param name="len">长度</param>
    /// <returns></returns>
    public static string CutString(this string str, int len)
    {
        if (string.IsNullOrWhiteSpace(str)) return string.Empty;

        if (str.Length > len)
            return str.Substring(0, len);
        return str;
    }

    /// <summary>
    ///     移除HTML标签
    /// </summary>
    /// <param name="html"></param>
    /// <returns></returns>
    public static string RemoveTags(this string html)
    {
        return string.IsNullOrEmpty(html)
            ? ""
            : Regex.Replace(html, "<[^<>]*>", "", RegexOptions.Singleline);
    }

    /// <summary>
    ///     去Html标签
    /// </summary>
    /// <param name="Htmlstring"></param>
    /// <returns></returns>
    public static string ToNoHtmlString(this string Htmlstring)
    {
        //删除脚本
        Htmlstring = Regex.Replace(Htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
        //删除HTML
        Htmlstring = Regex.Replace(Htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
        Htmlstring = Regex.Replace(Htmlstring, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
        Htmlstring = Regex.Replace(Htmlstring, @"-->", "", RegexOptions.IgnoreCase);
        Htmlstring = Regex.Replace(Htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);
        Htmlstring = Regex.Replace(Htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
        Htmlstring = Regex.Replace(Htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
        Htmlstring = Regex.Replace(Htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
        Htmlstring = Regex.Replace(Htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
        Htmlstring = Regex.Replace(Htmlstring, @"&(nbsp|#160);", "   ", RegexOptions.IgnoreCase);
        Htmlstring = Regex.Replace(Htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
        Htmlstring = Regex.Replace(Htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
        Htmlstring = Regex.Replace(Htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
        Htmlstring = Regex.Replace(Htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
        Htmlstring = Regex.Replace(Htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);
        Htmlstring.Replace("<", "");
        Htmlstring.Replace(">", "");
        Htmlstring.Replace("\r\n", "");
        //Htmlstring = HttpContext.Current.Server.HtmlEncode(Htmlstring).Trim();

        return Htmlstring;
    }


    /// <summary>
    ///     拆分成数组
    /// </summary>
    /// <param name="str">原字符串</param>
    /// <param name="strSplit">分隔字符</param>
    /// <returns>字符串数组</returns>
    public static string[] Split(this string str, string strSplit)
    {
        if (str.IndexOf(strSplit) < 0)
        {
            string[] tmp = { str };
            return tmp;
        }

        return Regex.Split(str, Regex.Escape(strSplit), RegexOptions.IgnoreCase);
    }

    /// <summary>
    ///     替换占位符
    /// </summary>
    /// <param name="value"></param>
    /// <param name="startLen"></param>
    /// <param name="endLen"></param>
    /// <param name="specialChar"></param>
    /// <returns></returns>
    public static string ReplaceWithSpecialChar(this string value, int startLen = 4, int endLen = 4,
        char specialChar = '*')
    {
        //ReplaceWithSpecialChar("柯小呆", 1, 0, '*')-- > Result: 柯*呆

        //ReplaceWithSpecialChar("622212345678485")-- > Result: 6222*******8485

        //ReplaceWithSpecialChar("622212345678485", 4, 4, '*')-- > Result: 6222*******8485
        var lenth = value.Length - startLen - endLen;

        var replaceStr = value.Substring(startLen, lenth);

        var specialStr = string.Empty;

        for (var i = 0; i < replaceStr.Length; i++) specialStr += specialChar;

        value = value.Replace(replaceStr, specialStr);

        return value;
    }

    /// <summary>
    ///     转long类型
    /// </summary>
    /// <param name="thisValue"></param>
    /// <param name="errorValue">异常默认值</param>
    /// <returns></returns>
    public static long ObjToLong(this object thisValue, long errorValue)
    {
        long result = 0;
        if (thisValue is Enum)
            return (long)thisValue;

        return thisValue != null && thisValue != DBNull.Value && long.TryParse(thisValue.ToString(), out result)
            ? result
            : errorValue;
    }

    /// <summary>
    ///     转byte类型
    /// </summary>
    /// <param name="thisValue"></param>
    /// <param name="errorValue"></param>
    /// <returns></returns>
    public static byte ObjToByte(this object thisValue, byte errorValue)
    {
        byte result = 0;
        if (thisValue is Enum)
            return (byte)thisValue;

        return thisValue != null && thisValue != DBNull.Value && byte.TryParse(thisValue.ToString(), out result)
            ? result
            : errorValue;
    }

    #region AsNull

    /// <summary>
    ///     如果字符串为空怎转换为NULL
    /// </summary>
    /// <param name="items"></param>
    /// <returns></returns>
    public static string AsNullIfEmpty(this string items)
    {
        return string.IsNullOrEmpty(items) ? null : items;
    }

    /// <summary>
    ///     如果字符串为空,空白字符则转换为NULL
    /// </summary>
    /// <param name="items"></param>
    /// <returns></returns>
    public static string AsNullIfWhiteSpace(this string items)
    {
        return string.IsNullOrWhiteSpace(items) ? null : items;
    }

    #endregion

    #region 截取字符串

    /// <summary>
    ///     截取字符串
    /// </summary>
    /// <param name="text">原字符串</param>
    /// <param name="characterCount">截取长度</param>
    /// <returns></returns>
    public static string Ellipsize(this string text, int characterCount)
    {
        return text.Ellipsize(characterCount, "&#160;&#8230;");
    }

    /// <summary>
    ///     截取字符串
    /// </summary>
    /// <param name="text">原字符串</param>
    /// <param name="characterCount">截取长度</param>
    /// <param name="ellipsis">省略部分</param>
    /// <returns></returns>
    public static string Ellipsize(this string text, int characterCount, string ellipsis)
    {
        if (string.IsNullOrWhiteSpace(text))
            return "";

        if (characterCount < 0 || text.Length <= characterCount)
            return text;

        return Regex.Replace(text.Substring(0, characterCount + 1), @"\s+\S*$", "") + ellipsis;
    }

    #endregion


    #region 字符串进行编码

    /// <summary>
    ///     把字符串转换成BASE64
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string EncodeBase64(this string str)
    {
        return Convert.ToBase64String(str.ToByteArray<UTF8Encoding>());
    }

    /// <summary>
    ///     把BASE64转换成字符串
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string DecodeBase64(this string str)
    {
        return Encoding.ASCII.GetString(Convert.FromBase64String(str));
    }

    /// <summary>
    ///     对URL字符串进行编码(UTF-8)
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public static string UrlEncode(this string url)
    {
        if (string.IsNullOrEmpty(url)) return string.Empty;
        return HttpUtility.UrlEncode(url, Encoding.UTF8);
    }

    /// <summary>
    ///     对URL字符串进行解码(UTF-8)
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string UrlDecode(this string str)
    {
        if (string.IsNullOrEmpty(str)) return string.Empty;
        return HttpUtility.UrlDecode(str, Encoding.UTF8);
    }

    #endregion

    #region 字符串转换为字节数

    /// <summary>
    ///     字符串转换为字节数组
    /// </summary>
    /// <typeparam name="TEncoding">编码</typeparam>
    /// <param name="str"></param>
    /// <returns></returns>
    public static byte[] ToByteArray<TEncoding>(this string str) where TEncoding : Encoding
    {
        Encoding enc = Activator.CreateInstance<TEncoding>();
        return enc.GetBytes(str);
    }

    /// <summary>
    ///     字符串转换为字节数组（UTF8）
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static byte[] ToByteArray(this string str)
    {
        return str.ToByteArray<UTF8Encoding>();
    }

    /// <summary>
    ///     字符串转换为字节序列（UTF8）
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static Stream ToStream(this string str)
    {
        return str.ToStream<UTF8Encoding>();
    }

    /// <summary>
    ///     字符串转换为字节序列
    /// </summary>
    /// <typeparam name="TEncoding">编码</typeparam>
    /// <param name="str"></param>
    /// <returns></returns>
    public static Stream ToStream<TEncoding>(this string str) where TEncoding : Encoding
    {
        var bytes = str.ToByteArray<TEncoding>();
        return new MemoryStream(bytes);
    }

    #endregion

    #region 验证

    /// <summary>
    ///     验证整数
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool IsValidNumber(this string input)
    {
        return ValidateString(input, @"^[1-9]{1}[0-9]{0,9}$");
    }

    /// <summary>
    ///     验证是否日期
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool IsValidDate(this string input)
    {
        var bValid = ValidateString(input, @"^[12]{1}(\d){3}[-][01]?(\d){1}[-][0123]?(\d){1}$");
        return bValid && input.CompareTo("1753-01-01") >= 0;
    }

    /// <summary>
    ///     验证EMAIL
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool IsValidEmail(this string input)
    {
        return ValidateString(input,
            @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
    }

    /// <summary>
    ///     验证EMAIL
    /// </summary>
    /// <param name="input"></param>
    /// <param name="expression"></param>
    /// <returns></returns>
    public static bool IsValidEmail(this string input, string expression)
    {
        if (string.IsNullOrEmpty(input)) return false;
        if (string.IsNullOrEmpty(expression)) expression = @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";
        return ValidateString(input, expression);
    }

    /// <summary>
    ///     验证是否手机号码
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool IsValidMobile(this string input)
    {
        if (string.IsNullOrEmpty(input)) return false;
        return ValidateString(input, "^0{0,1}(1[3-8])[0-9]{9}$");
    }

    internal static bool ValidateString(string input, string expression)
    {
        var validator = new Regex(expression, RegexOptions.None);
        return validator.IsMatch(input);
    }

    #endregion
}