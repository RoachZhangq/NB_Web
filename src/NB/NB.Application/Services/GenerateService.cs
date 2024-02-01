using NB.Application.IServices;
using NB.Core.Generate;

namespace NB.Application.Services;

public class GenerateService : IGenerateService, ITransient
{
    private readonly List<ConnectionConfig> _connections;
    private readonly IEntityGenerate _entityGenerate;
    private readonly SqlSugarScope _sqlSugarScope;

    public GenerateService(IEntityGenerate entityGenerate, ISqlSugarClient sqlSugarClient)
    {
        _entityGenerate = entityGenerate;
        _sqlSugarScope = sqlSugarClient as SqlSugarScope;
        _connections = App.GetOptions<ConnectionConfigOptions>().Connections;
    }

    /// <summary>
    ///     生成实体
    /// </summary>
    /// <param name="dbName"></param>
    /// <param name="tableNames"></param>
    public void GenerateEntity(string dbName, string tableNames)
    {
        if (_connections == null)
            return;

        foreach (var connection in _connections)
        {
            if (connection.ConfigId != dbName)
                continue;

            //全部表生成
            if (string.IsNullOrEmpty(tableNames))
            {
                _entityGenerate.GenerateEntity(Const.EntityPath, Const.EntityNamespace, connection.ConfigId.ToString(), "");
                continue;
            }

            //指定表生成
            var listTables = tableNames.Split(',').ToList();
            foreach (var tableName in listTables)
            {
                if (string.IsNullOrEmpty(tableName))
                    continue;
                _entityGenerate.GenerateEntity(Const.EntityPath, Const.EntityNamespace, connection.ConfigId.ToString(), tableName);
            }
        }
    }

    /// <summary>
    ///     生成Dao部分类
    /// </summary>
    /// <param name="partialClass"></param>
    /// <param name="dbName"></param>
    /// <param name="tableNames"></param>
    public void GenerateDao(bool partialClass, string dbName, string tableNames)
    {
        if (_connections == null)
            return;

        foreach (var connection in _connections)
        {
            if (connection.ConfigId != dbName)
                continue;

            //全部表生成
            if (string.IsNullOrEmpty(tableNames))
            {
                _entityGenerate.GenerateDao(Const.DaoLayerPath, Const.DaoLayerNamespace, Const.EntityNamespace,
                    connection.ConfigId.ToString(), connection.ConfigId.ToString(), "");
                if (partialClass)
                    _entityGenerate.GeneratePartialDao(Const.DaoLayerPath, Const.DaoLayerNamespace,
                        Const.EntityNamespace, connection.ConfigId.ToString(), connection.ConfigId + "Partial", "");
                continue;
            }

            //指定表生成
            var listTables = tableNames.Split(',').ToList();
            foreach (var tableName in listTables)
            {
                if (string.IsNullOrEmpty(tableName))
                    continue;
                _entityGenerate.GenerateDao(Const.DaoLayerPath, Const.DaoLayerNamespace, Const.EntityNamespace,
                    connection.ConfigId.ToString(), connection.ConfigId.ToString(), tableName);
                if (partialClass)
                    _entityGenerate.GeneratePartialDao(Const.DaoLayerPath, Const.DaoLayerNamespace,
                        Const.EntityNamespace, connection.ConfigId.ToString(), connection.ConfigId + "Partial", tableName);
            }
        }
    }

    /// <summary>
    ///     生成Dto类
    /// </summary>
    /// <param name="partialClass"></param>
    /// <param name="dbName"></param>
    /// <param name="tableName"></param>
    public void GenerateDto(bool partialClass, string dbName, string tableNames)
    {
        if (_connections == null)
            return;

        foreach (var connection in _connections)
        {
            if (connection.ConfigId != dbName)
                continue;

            //全部表生成
            if (string.IsNullOrEmpty(tableNames))
            {
                _entityGenerate.GenerateDto(Const.DtoPath, Const.DtoNamespace, connection.ConfigId.ToString(), connection.ConfigId.ToString(),
                    "");
                if (partialClass)
                    _entityGenerate.GeneratePartialDto(Const.DtoPath, Const.DtoNamespace, connection.ConfigId.ToString(),
                        connection.ConfigId + "Partial", "");
                continue;
            }

            //指定表生成
            var listTables = tableNames.Split(',').ToList();
            foreach (var tableName in listTables)
            {
                if (string.IsNullOrEmpty(tableName))
                    continue;
                _entityGenerate.GenerateDto(Const.DtoPath, Const.DtoNamespace, connection.ConfigId.ToString(), connection.ConfigId.ToString(),
                    tableName);

                if (partialClass)
                    _entityGenerate.GeneratePartialDto(Const.DtoPath, Const.DtoNamespace, connection.ConfigId.ToString(),
                        connection.ConfigId + "Partial", tableName);
            }
        }
    }

    /// <summary>
    ///     生成接口方法所需的入参和出参类
    /// </summary>
    /// <param name="interfaceNames"></param>
    public void GenerateIServiceDto(string interfaceNames)
    {
        //需要生成的命名空间
        var namespaceName = typeof(IGenerateService).Namespace;
        //忽略的
        var ignoreInterfaces = Const.GenerateIgnoreInterfaces.Split(',');

        var interfaces = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => p.IsInterface
                        && p.Namespace == namespaceName
                        && !ignoreInterfaces.Contains(p.Name))
            .WhereIF(!string.IsNullOrWhiteSpace(interfaceNames), p => interfaceNames.Contains(p.Name))
            .ToArray();

        foreach (var _interface in interfaces)
        {
            var allMethods = _interface.GetMethods();
            foreach (var methodInfo in allMethods)
            {
                //var fileName = char.ToLower(_interface.Name[0]) == 'i' ? _interface.Name.Substring(1) : _interface.Name;
                var fileName = _interface.Name;
                //请求类
                _entityGenerate.GenerateIServiceDto(Const.DtoPath, Const.DtoNamespace, fileName,
                    methodInfo.Name + Const.GenerateRequestDtoSuffix);
                //响应类
                _entityGenerate.GenerateIServiceDto(Const.DtoPath, Const.DtoNamespace, fileName,
                    methodInfo.Name + Const.GenerateResponseDtoSuffix);
            }
        }
    }

    /// <summary>
    ///     生成控制器代码(实现指定接口)
    /// </summary>
    /// <param name="controllerName"></param>
    /// <param name="interfaceName"></param>
    public void GenerateImplementationController(string controllerName, string interfaceName)
    {
        _entityGenerate.GenerateImplementationController(controllerName, interfaceName, Const.ControllerPath,
            Const.ControllerNamespace);
    }

    public string GetTableInfo(string configId, string tableNames = "")
    {
        //表格
        var tableHtml = new StringBuilder();
        //目录
        var catalogueHtml = new StringBuilder();

        if (!string.IsNullOrEmpty(configId))
            _sqlSugarScope.ChangeDatabase(configId);

        var tables = _sqlSugarScope.DbMaintenance.GetTableInfoList();
        //过滤
        var tbNames = tableNames.ToLower().Split('|');
        if (!string.IsNullOrEmpty(tableNames) && tbNames.Length > 0)
            tables = tables.FindAll(o => tbNames.Contains(o.Name.ToLower()));

        //排序
        tables = tables.OrderBy(o => o.Name).ToList();

        foreach (var table in tables)
        {
            //---------- 目录
            catalogueHtml.Append($"<li><a href='#{table.Name}'>{table.Name} {table.Description}</a></li>");
            //Console.WriteLine(table.Description);//输出表信息

            //--------- 表格
            var tableStart = $"<table  id='{table.Name}'>";
            var tabledEnd = "</table>";

            //--------- 表说明
            var caption = $"<caption><h2>{table.Name} {table.Description}</h2></caption>\n";

            //--------- 列
            string[] theads = { "序号", "列名", "数据类型", "长度", "允许Null值", "默认值", "说明" };
            var thead = new StringBuilder();
            thead.Append("<thead><tr>");
            foreach (var t in theads) thead.Append($"<th>{t}</th>");
            thead.Append("</tr></thead>");


            //获取列信息
            var columns = _sqlSugarScope.DbMaintenance.GetColumnInfosByTableName(table.Name, false);
            var tr = new StringBuilder();
            var index = 0;
            foreach (var column in columns)
            {
                var isIdentity = column.IsIdentity ? "[主键]" : "";
                tr.Append($"<tr>" +
                          $"<td>{++index}</td>" +
                          $"<td>{column.DbColumnName}</td>" +
                          $"<td>{column.DataType}</td>" +
                          $"<td>{column.Length}</td>" +
                          $"<td>{column.IsNullable}</td>" +
                          $"<td>{column.DefaultValue}</td>" +
                          $"<td>{isIdentity} {column.ColumnDescription}</td>" +
                          $"</tr>");
                // Console.WriteLine(column.ColumnDescription);//输出列的信息 column.xxx 
            }

            tableHtml.Append(tableStart + caption + thead + tr + tabledEnd);
        }

        var html =
            "<div id='catalogueDiv'><p onclick='catalogueClick()'>目录</p><ul id='catalogueUl' style=\"list-style-type:disc;display:none;\">" +
            catalogueHtml + "</ul></div>" //目录
            + tableHtml; //表格
        return html;
    }
}