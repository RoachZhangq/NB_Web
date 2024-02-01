namespace NB.Application.IServices;

public interface IGenerateService
{
    /// <summary>
    ///     生成实体
    /// </summary>
    /// <param name="dbName"></param>
    /// <param name="tableNames"></param>
    void GenerateEntity(string dbName, string tableNames);


    /// <summary>
    ///     生成Dao部分类
    /// </summary>
    /// <param name="partialClass"></param>
    /// <param name="dbName"></param>
    /// <param name="tableNames"></param>
    void GenerateDao(bool partialClass, string dbName, string tableNames);

    /// <summary>
    ///     生成Dto部分类
    /// </summary>
    /// <param name="partialClass"></param>
    /// <param name="dbName"></param>
    /// <param name="tableNames"></param>
    void GenerateDto(bool partialClass, string dbName, string tableNames);

    /// <summary>
    ///     生成接口方法所需的入参和出参类
    /// </summary>
    /// <param name="interfaceNames"></param>
    void GenerateIServiceDto(string interfaceNames);


    /// <summary>
    ///     生成控制器代码(实现指定接口)
    /// </summary>
    /// <param name="controllerName"></param>
    /// <param name="interfaceName"></param>
    void GenerateImplementationController(string controllerName, string interfaceName);

    /// <summary>
    ///     获取表信息
    /// </summary>
    /// <param name="configId"></param>
    /// <param name="tableNames"></param>
    /// <returns></returns>
    string GetTableInfo(string configId, string tableNames = "");
}