using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Furion.DependencyInjection;
using Furion.FriendlyException;
using Furion.LinqBuilder;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SqlSugar;

namespace NB.Core.Generate;

public class EntityGenerate : IEntityGenerate, ITransient
{
    private readonly ISqlSugarClient _sqlSugarClient;

    public EntityGenerate(ISqlSugarClient sqlSugarClient)
    {
        _sqlSugarClient = sqlSugarClient;
    }


    /// <summary>
    ///     生成实体
    /// </summary>
    /// <param name="relativePath"></param>
    /// <param name="namespacePath"></param>
    /// <param name="configId"></param>
    /// <param name="tableName"></param>
    public void GenerateEntity(string relativePath, string namespacePath, string configId, string tableName)
    {
        if (string.IsNullOrEmpty(relativePath))
            return;
        var path = GetPath(relativePath, configId);

        var client = _sqlSugarClient.DbFirst;
        if (!string.IsNullOrEmpty(tableName))
            client.Where(tableName);
        client.SettingClassTemplate(old =>
            {
                #region 模板

                var temp = @"{GenerateDescription}
" + old;
                temp = temp.Replace("{GenerateDescription}", @"/* 本文件自动生成
* 可能被覆盖，请勿修改
*/ ");

                #endregion

                return temp;
            })
            .SettingClassDescriptionTemplate(old =>
            {
                var temp = old + @"
    {TenantAttribute}";

                temp = temp.Replace("{TenantAttribute}", $@"[TenantAttribute(""{configId}"")]");
                return temp;
            })
            .SettingNamespaceTemplate(old => //命名空间 
            {
                return old;
            })
            .SettingPropertyDescriptionTemplate(old => //备注
            {
                var temp = """
                                      /// <summary>
                                      /// {PropertyDescription}
                                      /// </summary>
                           """;
                return temp;
            })
            .SettingPropertyTemplate(old => //属性
            {
                return old;
            })
            .SettingConstructorTemplate(old => //构造函数
            {
                return old;
            })
            .IsCreateAttribute()
            .CreateClassFile(path, namespacePath + "." + configId);
    }


    /// <summary>
    ///     生成dao
    /// </summary>
    /// <param name="relativePath"></param>
    /// <param name="namespacePath"></param>
    /// <param name="entityNamespacePath"></param>
    /// <param name="configId"></param>
    /// <param name="folderName"></param>
    /// <param name="tableName"></param>
    public void GenerateDao(string relativePath, string namespacePath, string entityNamespacePath, string configId,
        string folderName, string tableName)
    {
        if (string.IsNullOrEmpty(relativePath))
            return;
        var path = GetPath(relativePath, configId, folderName);

        var client = _sqlSugarClient.DbFirst;
        if (!string.IsNullOrEmpty(tableName))
            client.Where(tableName);
        client.SettingClassTemplate(old =>
            {
                #region 模板

                var temp = @"{GenerateDescription}
{using}
namespace {Namespace}
{
{ClassDescription}{SugarTable}
    public partial class {ClassName}Dao : DbContext<{EntityNamespacePath}{ClassName}>, ITransient
    {		 
		 
		public {ClassName}Dao(ISqlSugarClient sqlSugarClient) : base(sqlSugarClient)
        {			 
            //此处请勿修改，去此类的partial类里新增代码！
        }		   

    }
}";
                temp = temp.Replace("{configId}", configId)
                    .Replace("{EntityNamespacePath}", $"{entityNamespacePath + "." + configId}.")
                    .Replace("{GenerateDescription}", @"/* 本文件自动生成
* 可能被覆盖，请勿修改
*/ ");

                #endregion

                return temp;
            })
            .SettingNamespaceTemplate(old => //命名空间 
            {
                #region 模板

                var temp = @"using Furion.DependencyInjection;
using SqlSugar;";

                #endregion

                return temp;
            })
            .SettingPropertyDescriptionTemplate(old => //备注
            {
                return "";
            })
            .SettingPropertyTemplate(old => //属性
            {
                return "";
            })
            .SettingConstructorTemplate(old => //构造函数
            {
                return "";
            })
            .FormatFileName(o => o + "Dao")
            .CreateClassFile(path, namespacePath + "." + configId);
    }

    /// <summary>
    ///     生成dao的部分类
    /// </summary>
    /// <param name="relativePath"></param>
    /// <param name="namespacePath"></param>
    /// <param name="entityNamespacePath"></param>
    /// <param name="configId"></param>
    /// <param name="folderName"></param>
    /// <param name="tableName"></param>
    public void GeneratePartialDao(string relativePath, string namespacePath, string entityNamespacePath,
        string configId, string folderName, string tableName)
    {
        if (string.IsNullOrEmpty(relativePath))
            return;
        var path = GetPath(relativePath, configId, folderName);

        var client = _sqlSugarClient.DbFirst;
        if (!string.IsNullOrEmpty(tableName))
            client.Where(tableName);
        client.SettingClassTemplate(old =>
            {
                #region 模板

                var temp = @"{using}
namespace {Namespace}
{
{ClassDescription}{SugarTable}
    public partial class {ClassName}Dao 
    {
		
		   

    }
}";

                #endregion

                return temp;
            })
            .SettingNamespaceTemplate(old => //命名空间 
            {
                #region 模板

                var temp = $@"using SqlSugar;
using {entityNamespacePath + "." + configId};
";

                #endregion

                return temp;
            })
            .SettingPropertyDescriptionTemplate(old => //备注
            {
                return "";
            })
            .SettingPropertyTemplate(old => //属性
            {
                return "";
            })
            .SettingConstructorTemplate(old => //构造函数
            {
                return "";
            })
            .FormatFileName(o => o + "Dao")
            .CreateClassFile(path, namespacePath + "." + configId);
    }

    /// <summary>
    ///     生成dto
    /// </summary>
    /// <param name="relativePath"></param>
    /// <param name="namespacePath"></param>
    /// <param name="configId"></param>
    /// <param name="folderName"></param>
    /// <param name="tableName"></param>
    public void GenerateDto(string relativePath, string namespacePath, string configId, string folderName,
        string tableName)
    {
        if (string.IsNullOrEmpty(relativePath))
            return;
        var path = GetPath(relativePath, configId);

        var client = _sqlSugarClient.DbFirst;
        if (!string.IsNullOrEmpty(tableName))
            client.Where(tableName);
        client.SettingClassTemplate(old =>
            {
                #region 模板

                var temp = @"{using}
namespace {Namespace}
{
{ClassDescription}{SugarTable}
    public partial class {ClassName}Dto 
    {
{PropertyName}
		   

    }
}";


                temp = temp.Replace("using SqlSugar;", "");

                #endregion

                return temp;
            })
            .SettingClassDescriptionTemplate(old => { return old; })
            .SettingNamespaceTemplate(old => //命名空间 
            {
                return old;
            })
            .SettingPropertyDescriptionTemplate(old => //备注
            {
                var temp = """
                                      /// <summary>
                                      /// {PropertyDescription}
                                      /// </summary>
                           """;
                return temp;
            })
            .SettingPropertyTemplate(old => //属性
            {
                return old;
            })
            .SettingConstructorTemplate(old => //构造函数
            {
                return "";
            })
            .FormatFileName(o => o + "Dto")
            //.IsCreateAttribute()
            .CreateClassFile(path, namespacePath + "." + configId);
    }

    /// <summary>
    ///     生成dto的部分类
    /// </summary>
    /// <param name="relativePath"></param>
    /// <param name="namespacePath"></param>
    /// <param name="configId"></param>
    /// <param name="folderName"></param>
    /// <param name="tableName"></param>
    public void GeneratePartialDto(string relativePath, string namespacePath, string configId, string folderName,
        string tableName)
    {
        if (string.IsNullOrEmpty(relativePath))
            return;
        var path = GetPath(relativePath, configId, folderName);

        var client = _sqlSugarClient.DbFirst;
        if (!string.IsNullOrEmpty(tableName))
            client.Where(tableName);
        client.SettingClassTemplate(old =>
            {
                #region 模板

                var temp = @"{using}
namespace {Namespace}
{
{ClassDescription}{SugarTable}
    public partial class {ClassName}Dto 
    {
		
		   

    }
}";

                #endregion

                temp = temp.Replace("using SqlSugar;", "");

                return temp;
            })
            .SettingNamespaceTemplate(old => //命名空间 
            {
                return old;
            })
            .SettingPropertyDescriptionTemplate(old => //备注
            {
                return "";
            })
            .SettingPropertyTemplate(old => //属性
            {
                return "";
            })
            .SettingConstructorTemplate(old => //构造函数
            {
                return "";
            })
            .FormatFileName(o => o + "Dto")
            .CreateClassFile(path, namespacePath + "." + configId);
    }

    /// <summary>
    ///     生成请求和响应类
    /// </summary>
    /// <param name="relativePath"></param>
    /// <param name="namespacePath"></param>
    /// <param name="folderName"></param>
    /// <param name="className"></param>
    public void GenerateIServiceDto(string relativePath, string namespacePath, string folderName, string className)
    {
        var directoryPath = relativePath + "\\" + folderName;

        var path = Directory.GetCurrentDirectory();
        var i = path.LastIndexOf("\\"); //获取字符串最后一个斜杠的位置

        if (i == -1) //应对不同系统
        {
            i = path.LastIndexOf("/");
            directoryPath = directoryPath.Replace("\\", "/");
        }

        //向上退一级
        path = path.Substring(0, i); //

        path += directoryPath;
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        //目录分隔符
        var separatorChar = Path.DirectorySeparatorChar.ToString();


        var filePath = path.TrimEnd('\\').TrimEnd('/') + string.Format(separatorChar + "{0}.cs", className);

        if (File.Exists(filePath)) return;

        var classTemplate = """
                            namespace {Namespace};

                            public partial class {ClassName}
                            {
                                
                            }
                            """;
        classTemplate = classTemplate.Replace("{Namespace}", namespacePath + "." + folderName)
            .Replace("{ClassName}", className);

        using (var fileStream = new FileInfo(filePath).Create())
        {
            using (var streamWriter = new StreamWriter(fileStream, Encoding.UTF8))
            {
                streamWriter.Write(classTemplate);
                streamWriter.Flush();
            }
        }
    }

    /// <summary>
    ///     生成控制器代码(实现指定接口)
    /// </summary>
    /// <param name="controllerName"></param>
    /// <param name="interfaceName"></param>
    /// <param name="apiPath"></param>
    /// <param name="namespacePath"></param>
    public void GenerateImplementationController(string controllerName, string interfaceName, string apiPath,
        string namespacePath)
    {
        if (controllerName.Contains("Controller"))
            controllerName = controllerName.Replace("Controller", "");

        var controllerDirectory = apiPath;
        var interfacePath = Const.InterfacePath;
        var path = Directory.GetCurrentDirectory();
        var i = path.LastIndexOf("\\"); //获取字符串最后一个斜杠的位置
        if (i == -1) //应对不同系统
        {
            i = path.LastIndexOf("/");
            controllerDirectory = controllerDirectory.Replace("\\", "/");
            interfacePath = interfacePath.Replace("\\", "/");
        }

        //向上退一级
        path = path.Substring(0, i);
        controllerDirectory = path + controllerDirectory;
        interfacePath = path + interfacePath;

        if (!Directory.Exists(controllerDirectory))
            Directory.CreateDirectory(controllerDirectory);

        //目录分隔符
        var separatorChar = Path.DirectorySeparatorChar.ToString();

        var filePath = controllerDirectory.TrimEnd('\\').TrimEnd('/') +
                       string.Format(separatorChar + "{0}.cs", controllerName + "Controller");

        //控制器内容
        var controllerStr = string.Empty;
        //控制器存在的方法
        var controllerMethodList =
            new List<(string _comments, bool _isTask, string _returnType, string _methodName,
                string _parameterStr)>();
        //控制器存在的字段
        var controllerFields = new List<string>();

        //控制器参数的个数
        var controllerConstructorParametersCount = 0;

        //控制器已存在
        var controllerIsExist = false;
        //接口变量别名(首字母小写---去掉首字母I)
        var interfaceAliasName = char.ToLower(interfaceName[1]) + interfaceName.Substring(2);

        #region 获取控制器文件,存在就读取,不存在就生成新文件

        if (File.Exists(filePath))
        {
            controllerIsExist = true;
            //读取内容
            controllerStr = File.ReadAllText(filePath);
            //获取控制器Type
            // controllerType = AppDomain.CurrentDomain
            //     .GetAssemblies()
            //     .SelectMany(s => s.GetTypes())
            //     .FirstOrDefault(p => p.IsClass
            //                          && p.Namespace == namespacePath
            //                          && p.Name == controllerName + "Controller");
            //解析方法
            (controllerConstructorParametersCount, controllerFields) =
                GetMethodInfoByClass(controllerStr, controllerMethodList);
        }
        else
        {
            controllerStr = """
                            namespace {namespace};

                            /// <summary>
                            ///     {name}
                            /// </summary>
                            [ApiDescriptionSettings("{name}")]
                            public class {name}Controller : IDynamicApiController
                            {
                                //Mark-Fields-Start
                              
                                //Mark-Fields-End
                                
                                /// <summary>
                                ///     {name}构造函数
                                /// </summary>
                                public {name}Controller() //Mark-Constructor-Bracket-End
                                {
                                } //Mark-Constructor-End

                            } //Mark-Class-End
                            """;
            controllerStr = controllerStr.Replace("{name}", controllerName)
                .Replace("{namespace}", namespacePath);
        }

        #endregion

        #region 生成接口方法(Roslyn，它是一个由Microsoft开发的.NET编译器平台，可以用于分析和操作C#代码)

        //接口存的方法
        var interfaceMethodList =
            new List<(string _comments, bool _isTask, string _returnType, string _methodName,
                string _parameterStr,
                List<(string paramType, string paramName)> _parameters)>();
        var interface_filePath = interfacePath.TrimEnd('\\').TrimEnd('/') +
                                 string.Format(separatorChar + "{0}.cs", interfaceName);
        if (!File.Exists(interface_filePath))
            throw Oops.Oh("接口文件不存在");
        var interfaceStr = File.ReadAllText(interface_filePath);


        //解析方法
        GetMethodInfoByInterface(interfaceStr, interfaceMethodList);
        if (interfaceMethodList.Count == 0)
            throw Oops.Oh("接口中没有定义方法");

        foreach (var t in interfaceMethodList)
        {
            //****判断控制器已经存在就忽略
            if (controllerMethodList.Any(o =>
                    o._returnType == t._returnType && o._methodName == t._methodName &&
                    o._parameterStr == t._parameterStr))
                continue;


            var methodTemp = """
                             {comment}
                                 public {returnType} {methodName}({parameters})
                                 {
                                     {return}_{interfaceAliasName}.{methodName}({inParameters});
                                 }
                             """;
            //注释
            methodTemp = methodTemp.Replace("{comment}", t._comments);

            //返回类型
            if (t._isTask)
                methodTemp = methodTemp.Replace("{returnType}", "async " + t._returnType);
            else
                methodTemp = methodTemp.Replace("{returnType}", t._returnType);

            //方法名
            methodTemp = methodTemp.Replace("{methodName}", t._methodName);

            //方法参数
            methodTemp = methodTemp.Replace("{parameters}", t._parameterStr);

            //return 关键词
            if (t._isTask && t._returnType.IndexOf("<") > -1)
                methodTemp = methodTemp.Replace("{return}", "return await ");
            else if (t._isTask && t._returnType.IndexOf("void") == -1)
                methodTemp = methodTemp.Replace("{return}", "await ");
            else if (t._isTask == false && t._returnType.IndexOf("void") == -1)
                methodTemp = methodTemp.Replace("{return}", "return ");
            else
                methodTemp = methodTemp.Replace("{return}", "");

            //接口变量别名
            methodTemp = methodTemp.Replace("{interfaceAliasName}", interfaceAliasName);

            //参数
            var inParameters = string.Empty;
            if (!t._parameters.IsNullOrEmpty())
                inParameters = string.Join(",", t._parameters.Select(o => o.paramName).ToArray());

            methodTemp = methodTemp.Replace("{inParameters}", inParameters);

            // 追加新方法
            var classEnd = """


                           } //Mark-Class-End
                           """;
            controllerStr = controllerStr.Replace("} //Mark-Class-End", methodTemp + classEnd);
        }

        #endregion

        #region 生成字段(构造函数注入)

        var isExistFiled = false;

        if (controllerIsExist)
            if (controllerFields.Any(o => o == "_" + interfaceAliasName))
                isExistFiled = true;

        if (isExistFiled == false)
        {
            #region 添加新变量

            var variable = """
                               private readonly {interfaceName} _{interfaceAliasName};
                           
                               //Mark-Fields-End
                           """;
            controllerStr = controllerStr.Replace("    //Mark-Fields-End", variable);

            #endregion

            #region 构造函数的注释

            var constructorSummary = new StringBuilder();
            var constructorSummaryStart = """
                                              /// <summary>
                                              ///     {name}构造函数
                                              /// </summary>

                                          """;
            constructorSummary.Append(constructorSummaryStart.Replace("{name}", controllerName));

            var constructorSummaryItem = """
                                             /// <param name="{fieldName}"></param>

                                         """;
            foreach (var controllerField in controllerFields)
                constructorSummary.Append(constructorSummaryItem.Replace("{fieldName}", controllerField.Substring(1)));
            var oldSummary = constructorSummary.ToString();
            constructorSummary.Append(constructorSummaryItem.Replace("{fieldName}", interfaceAliasName));
            var newSummary = constructorSummary.ToString();
            controllerStr = controllerStr.Replace(oldSummary, newSummary);

            #endregion

            #region 变量注入到构造函数

            // 获取构造函数信息

            var separator = controllerConstructorParametersCount >= 1 ? ", " : "";
            var variableIO = separator + "{interfaceName} {interfaceAliasName}) //Mark-Constructor-Bracket-End";

            controllerStr = controllerStr.Replace(") //Mark-Constructor-Bracket-End", variableIO);

            #endregion

            #region 构造函数里的变量

            var constructorVariable = """
                                          _{interfaceAliasName} = {interfaceAliasName};
                                          } //Mark-Constructor-End
                                      """;
            controllerStr = controllerStr.Replace("} //Mark-Constructor-End", constructorVariable);

            #endregion

            controllerStr = controllerStr.Replace("{interfaceName}", interfaceName)
                .Replace("{interfaceAliasName}", interfaceAliasName);
        }

        #endregion

        using var fileStream = new FileInfo(filePath).Create();
        using var streamWriter = new StreamWriter(fileStream, Encoding.UTF8);
        streamWriter.Write(controllerStr);
        streamWriter.Flush();
    }

    /// <summary>
    ///     解析获取代码中的方法
    /// </summary>
    /// <param name="controllerStr"></param>
    /// <param name="controllerMethodList"></param>
    /// <returns></returns>
    private (int constructorParametersCount, List<string> fields) GetMethodInfoByClass(string controllerStr,
        List<(string _comments, bool _isTask, string _returnType, string _methodName, string _parameterStr)>
            controllerMethodList)
    {
        //构造器参数数量
        var constructorParametersCount = 0;
        //字段
        var fields = new List<string>();

        var syntaxTree = CSharpSyntaxTree.ParseText(controllerStr);
        var root = syntaxTree.GetRoot();

        // 查找所有类声明节点
        var classDeclarations = root.DescendantNodes().OfType<ClassDeclarationSyntax>();
        if (classDeclarations.Count() > 1)
            throw Oops.Oh("控制器文件中包含多个Class,无法解析");

        foreach (var classDeclaration in classDeclarations)
        {
            // 查找类中的字段声明
            var fieldDeclarations = classDeclaration.DescendantNodes().OfType<FieldDeclarationSyntax>();

            foreach (var fieldDeclaration in fieldDeclarations)
            foreach (var variable in fieldDeclaration.Declaration.Variables)
            {
                // 获取字段类型
                //string fieldType = fieldDeclaration.Declaration.Type.ToString();
                // 获取字段名
                var fieldName = variable.Identifier.Text;
                fields.Add(fieldName);
            }

            // 查找类中的构造函数
            var constructorDeclarations = classDeclaration.DescendantNodes().OfType<ConstructorDeclarationSyntax>();
            // 获取构造函数参数个数
            foreach (var constructorDeclaration in constructorDeclarations)
                constructorParametersCount = constructorDeclaration.ParameterList.Parameters.Count;

            // 获取类名
            var className = classDeclaration.Identifier.Text;

            // 查找类中的方法
            var methodDeclarations = classDeclaration.DescendantNodes().OfType<MethodDeclarationSyntax>();

            foreach (var methodDeclaration in methodDeclarations)
            {
                // 获取方法名
                var methodName = methodDeclaration.Identifier.Text;

                // 获取方法返回类型
                var returnType = methodDeclaration.ReturnType.ToString();

                // 获取方法参数
                var parameters = methodDeclaration.ParameterList.Parameters;
                var parameterList = string.Join(", ",
                    parameters.Select(param => param.Type + " " + param.Identifier.Text));

                // 获取方法注释
                var methodComment = methodDeclaration.GetLeadingTrivia().ToString();

                // 打印提取的信息
                //Console.WriteLine($"Class: {className}, Method: {methodName}");
                //Console.WriteLine($"Return Type: {returnType}");
                //Console.WriteLine($"Parameters: {parameterList}");
                //Console.WriteLine($"Comment: {methodComment}");

                controllerMethodList.Add((methodComment, returnType.IndexOf("Task") > -1, returnType, methodName,
                    parameterList));
            }
        }

        return (constructorParametersCount, fields);
    }

    /// <summary>
    ///     解析获取代码中的方法
    /// </summary>
    /// <param name="code"></param>
    /// <param name="methodList"></param>
    private void GetMethodInfoByInterface(string code,
        List<(string _comments, bool _isTask, string _returnType, string _methodName, string _parameterStr,
            List<(string paramType, string paramName)> _parameters)> methodList)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var root = (CompilationUnitSyntax)syntaxTree.GetRoot();

        var interfaceDeclaration = root.DescendantNodes()
            .OfType<InterfaceDeclarationSyntax>()
            .FirstOrDefault();

        if (interfaceDeclaration != null)
            foreach (var methodDeclaration in interfaceDeclaration.Members.OfType<MethodDeclarationSyntax>())
            {
                var methodName = methodDeclaration.Identifier.ValueText;
                var returnType = methodDeclaration.ReturnType.ToString();
                var parameters = string.Join(", ",
                    methodDeclaration.ParameterList.Parameters.Select(p => p.ToString()));
                var comment = methodDeclaration.GetLeadingTrivia().ToString();

                var paramParts = parameters.Split(',');
                var _parametersList =
                    new List<(string paramType, string paramName)>();
                foreach (var paramPart in paramParts)
                {
                    var paramComponents = paramPart.Trim().Split(' ');
                    if (paramComponents.Length >= 2)
                    {
                        var paramType = paramComponents[0];
                        var paramName = paramComponents[1];
                        _parametersList.Add((paramType, paramName));
                    }
                }

                if (string.IsNullOrWhiteSpace(comment))
                    comment = "";
                if (string.IsNullOrWhiteSpace(parameters))
                    parameters = "";
                methodList.Add((comment, returnType.IndexOf("Task") > -1, returnType, methodName,
                    parameters, _parametersList));
            }
    }

    /// <summary>
    ///     获取路径
    /// </summary>
    /// <param name="relativePath"></param>
    /// <param name="configId"></param>
    /// <param name="folderName"></param>
    /// <returns></returns>
    private string GetPath(string relativePath, string configId, string folderName = "")
    {
        _sqlSugarClient.AsTenant().ChangeDatabase(configId);

        relativePath += "\\" + (string.IsNullOrEmpty(folderName) ? configId : folderName);

        var path = Directory.GetCurrentDirectory();
        var i = path.LastIndexOf("\\"); //获取字符串最后一个斜杠的位置

        if (i == -1) //应对不同系统
        {
            i = path.LastIndexOf("/");
            relativePath = relativePath.Replace("\\", "/");
        }

        path = path.Substring(0, i); //
        path += relativePath;
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        return path;
    }
}