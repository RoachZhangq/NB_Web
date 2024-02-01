namespace NB.Core.Generate;

public interface IEntityGenerate
{
    /// <summary>
    ///     生成实体
    /// </summary>
    /// <param name="relativePath"></param>
    /// <param name="namespacePath"></param>
    /// <param name="configId"></param>
    /// <param name="tableName"></param>
    void GenerateEntity(string relativePath, string namespacePath, string configId, string tableName);


    /// <summary>
    ///     生成Dao层
    /// </summary>
    /// <param name="relativePath"></param>
    /// <param name="namespacePath"></param>
    /// <param name="entityNamespacePath"></param>
    /// <param name="configId"></param>
    /// <param name="folderName"></param>
    /// <param name="tableName"></param>
    void GenerateDao(string relativePath, string namespacePath, string entityNamespacePath, string configId,
        string folderName, string tableName);


    /// <summary>
    ///     生成Dao层Partial
    /// </summary>
    /// <param name="relativePath"></param>
    /// <param name="namespacePath"></param>
    /// <param name="entityNamespacePath"></param>
    /// <param name="configId"></param>
    /// <param name="folderName"></param>
    /// <param name="tableName"></param>
    void GeneratePartialDao(string relativePath, string namespacePath, string entityNamespacePath, string configId,
        string folderName, string tableName);


    /// <summary>
    ///     生成Dao层
    /// </summary>
    /// <param name="relativePath"></param>
    /// <param name="namespacePath"></param>
    /// <param name="configId"></param>
    /// <param name="folderName"></param>
    /// <param name="tableName"></param>
    void GenerateDto(string relativePath, string namespacePath, string configId, string folderName, string tableName);

    /// <summary>
    ///     生成Dao层Partial
    /// </summary>
    /// <param name="relativePath"></param>
    /// <param name="namespacePath"></param>
    /// <param name="configId"></param>
    /// <param name="folderName"></param>
    /// <param name="tableName"></param>
    void GeneratePartialDto(string relativePath, string namespacePath, string configId, string folderName,
        string tableName);


    /// <summary>
    ///     生成请求和响应类
    /// </summary>
    /// <param name="relativePath"></param>
    /// <param name="namespacePath"></param>
    /// <param name="folderName"></param>
    /// <param name="className"></param>
    void GenerateIServiceDto(string relativePath, string namespacePath, string folderName, string className);


    /// <summary>
    ///     生成控制器代码(实现指定接口)
    /// </summary>
    /// <param name="controllerName"></param>
    /// <param name="interfaceName"></param>
    /// <param name="apiPath"></param>
    /// <param name="namespacePath"></param>
    void GenerateImplementationController(string controllerName, string interfaceName, string apiPath,
        string namespacePath);
}