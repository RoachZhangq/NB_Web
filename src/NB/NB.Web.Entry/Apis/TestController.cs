using System.Diagnostics;
using Newtonsoft.Json;
using NB.Core.Common;
using NB.Core.Entity.ReadyGo;

namespace NB.Web.Entry.Apis;

/// <summary>
///     测试
/// </summary>
[ApiDescriptionSettings("Test")]
public class TestService : IDynamicApiController
{
    private readonly IEventPublisher _eventPublisher;
    private readonly ITest _test;

    /// <summary>
    ///     Test
    /// </summary>
    /// <param name="test"></param>
    /// <param name="eventPublisher"></param>
    public TestService(ITest test, IEventPublisher eventPublisher)
    {
        _test = test;
        _eventPublisher = eventPublisher;
    }

    /// <summary>
    ///     TEST
    /// </summary>
    [LoggingMonitor]
    public void Test()
    {
        _test.Test();
    }

    /// <summary>
    ///     测试事件
    /// </summary>
    /// <returns></returns>
    public async Task CreateDoTo(string name)
    {
        //App.PrintToMiniProfiler("分类", "状态", "要打印的消息");
        await _eventPublisher.PublishAsync("ToDo:Create", name);
    }

    /// <summary>
    ///     测试ServiceStack.Redis读取次数(测试1万次)
    /// </summary>
    /// <returns></returns>
    public void TestRedis()
    {
        var _serviceStackRedis = App.GetService<ServiceStackRedis>();

        _serviceStackRedis.Set("TestKey1", "Hello Redis", 1800);

        for (var i = 0; i < 10000; i++)
            try
            {
                var value = _serviceStackRedis.Get<string>("TestKey1");
                Console.WriteLine($"第{i + 1}次读取:{value}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"第{i + 1}次读取失败====================={ex.Message}");
            }
    }

    /// <summary>
    ///     测试ABC
    /// </summary>
    [Obsolete("TestABC() 已经过时，请调用 TestABC**() 替代")]
    public void TestABC()
    {
    }

    /// <summary>
    ///     测试对象深拷贝(两种方式,各一百万次)
    /// </summary>
    public void TestObjectCopy()
    {
        var m = new MasterOrder
        {
            MasterOrderId = 1,
            MasterOrderNumber = "10001",
            AddTime = DateTime.Now
        };
        //Console.WriteLine(m.GetHashCode());
        var stopWatch = new Stopwatch();
        stopWatch.Start();
        for (var i = 0; i < 1000000; i++)
        {
            var mm = DeepCopy<MasterOrder, MasterOrder>.Trans(m);
            //Console.WriteLine(mm.GetHashCode());
        }

        stopWatch.Stop();
        Console.WriteLine("第一个耗时:" + stopWatch.Elapsed.TotalMilliseconds);

        stopWatch.Restart();
        for (var i = 0; i < 1000000; i++)
        {
            var mm = JsonConvert.DeserializeObject<MasterOrder>(JsonConvert.SerializeObject(m));
            //Console.WriteLine(mm.GetHashCode());
        }

        stopWatch.Stop();
        Console.WriteLine("第二个耗时:" + stopWatch.Elapsed.TotalMilliseconds);
    }
}