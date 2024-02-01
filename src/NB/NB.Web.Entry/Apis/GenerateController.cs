namespace NB.Web.Entry.Apis;

/// <summary>
///     代码生成类
/// </summary>
[ApiDescriptionSettings("Generate")]
public class GenerateController : IDynamicApiController
{
    private readonly IGenerateService _generateService;


    /// <summary>
    ///     Generate
    /// </summary>
    /// <param name="generateService"></param>
    public GenerateController(IGenerateService generateService)
    {
        _generateService = generateService;
    }

    /// <summary>
    ///     生成部分实体
    /// </summary>
    /// <param name="dbName">数据库 ConfigId</param>
    /// <param name="tableNames">表名 逗号分隔</param>
    /// <returns></returns>
    [AllowAnonymous] //允许匿名访问
    public bool GetGeneratePartEntity(string dbName, [FromQuery] string tableNames)
    {
        _generateService.GenerateEntity(dbName, tableNames);
        return true;
    }

    /// <summary>
    ///     生成部分实体DAO层
    /// </summary>
    /// <param name="needPartialClass">需要生成部分类</param>
    /// <param name="dbName">数据库 ConfigId</param>
    /// <param name="tableNames">表名 逗号分隔</param>
    /// <returns></returns>
    [AllowAnonymous] //允许匿名访问
    public bool GetGenerateDao(string dbName, [FromQuery] string tableNames, bool needPartialClass = false)
    {
        _generateService.GenerateDao(needPartialClass, dbName, tableNames);
        return true;
    }

    /// <summary>
    ///     生成部分实体DTO层
    /// </summary>
    /// <param name="dbName"></param>
    /// <param name="tableNames"></param>
    /// <param name="needPartialClass"></param>
    /// <returns></returns>
    [AllowAnonymous] //允许匿名访问
    public bool GetGenerateDto(string dbName, [FromQuery] string tableNames, bool needPartialClass = false)
    {
        _generateService.GenerateDto(needPartialClass, dbName, tableNames);
        return true;
    }

    /// <summary>
    ///     生成接口方法所需的入参和出参类---懒人新工具
    /// </summary>
    /// <param name="interfaceNames"></param>
    /// <returns></returns>
    public bool GetGenerateIServiceDto([FromQuery] string interfaceNames)
    {
        _generateService.GenerateIServiceDto(interfaceNames);
        return true;
    }

    /// <summary>
    ///     生成控制器代码(实现指定接口)---懒人新工具
    /// </summary>
    /// <param name="controllerName"></param>
    /// <param name="interfaceName"></param>
    public void GetGenerateImplementationController(string controllerName, string interfaceName)
    {
        _generateService.GenerateImplementationController(controllerName, interfaceName);
    }

    /// <summary>
    ///     获取表信息
    /// </summary>
    /// <param name="db">数据库configId</param>
    /// <param name="tableNames">表名(|分割)</param>
    [HttpGet]
    [Route("table")]
    [AllowAnonymous] //允许匿名访问
    public void GetTableInfo(string db = "", string tableNames = "")
    {
        var tables = _generateService.GetTableInfo(db, tableNames);
        var htmlStringBuilder = new StringBuilder();
        htmlStringBuilder.Append("<html>");

        #region Css样式

        var css = @"html,
body {
	height: 100%;
}

body {
	margin: 0;
	background: linear-gradient(45deg, #49a09d, #49a09d);
	font-family: sans-serif;
	font-weight: 100;
}

.container {
	position: absolute;
	top: 50%;
	left: 50%;
	transform: translate(-50%, -50%);
}

table {
	width: 100%;
	border-collapse: collapse;
	overflow: hidden;
	box-shadow: 0 0 20px rgba(0,0,0,0.1);
    background-color: rgba(255,255,255,0.2);
    border-radius: 30px;
}

th,
td {
	padding: 15px;	
	color: #fff;
}

th {
	text-align: left;
}
tbody tr {
        line-height:0px;
    }
tbody tr td:last-child{
    white-space:nowrap;
}
thead {
	th {
		background-color: #55608f;
	}
}
tbody tr:hover{
    background-color: rgba(255,255,255,0.3);
}
ul {
    font-size:20px;
}
#catalogueUl a {
    cursor:pointer;
    text-decoration: none;
}
#catalogueDiv p {
    cursor:pointer;
    font: 40px/1.5 'Microsoft YaHei',arial,tahoma,\5b8b\4f53,sans-serif;
}

 
    #catalogueDiv p{
 
		    font-family: ""Microsoft YaHei"";
		    font-size: 60px; 
		    background: -webkit-linear-gradient(left, #0f0, #00f) 0 0 no-repeat;/*设置线性渐变*/
		    -webkit-background-size: 160px;                        /*设置背景大小*/
		    -webkit-background-clip: text;                            /*背景被裁剪到文字*/
		    -webkit-text-fill-color: rgba(255, 255, 255, 0.3);        /*设置文字的填充颜色*/
		    -webkit-animation: shine 3s infinite;                    /*设置动画*/
		}
		@-webkit-keyframes shine{   /*创建动画*/
		    0%{
		        background-position: 0 0;
		    }
		    100%{
		        background-position: 100% 100%;
		    }
		}
 ";

        #endregion

        #region js

        var js = @"function catalogueClick(){
                    var element = document.getElementById('catalogueUl');
                    if(element.style.display=='none'){
                        element.setAttribute('style','display:block');
                    }else{
                        element.setAttribute('style','display:none');
                    }
                }";

        #endregion

        htmlStringBuilder.Append($"<head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />" +
                                 $" <style>{css}</style>"
                                 + $"<script>{js}</script>"
                                 + $"</head>"); //支持中文
        htmlStringBuilder.Append("<body>");
        htmlStringBuilder.Append("<spen style=\"font-size: 300%\">"); //让字体变大
        //string a = $"返回的内容";
        htmlStringBuilder.Append(tables);
        htmlStringBuilder.Append("</spen>");
        htmlStringBuilder.Append("</body>");
        htmlStringBuilder.Append("</html>");
        var result = htmlStringBuilder.ToString();
        var data = Encoding.UTF8.GetBytes(result);

        var response = App.HttpContext.Response;
        response.ContentType = "text/html";

        response.Body.WriteAsync(data, 0, data.Length);
    }
}