using NB.Application.IServices;

namespace NB.Application.Services;

public class TestService : ITest, ITransient
{
    //private MasterOrderDao _masterOrderDao;
    // private CoreCmsGoodsDao _coreCmsGoodsDao;


    private SqlSugarScope _sqlSugarScope;
    //private SimpleClient<TEntity> _simpleClient;


    public TestService(ISqlSugarClient sqlSugarClient)
    {
        _sqlSugarScope = sqlSugarClient as SqlSugarScope;
    }


    public void Test()
    {
        //var masterOrder = _masterOrderDao.SimpleClient.GetFirst(o => o.MasterOrderId > 0);
        //var goods = _coreCmsGoodsDao.SimpleClient.GetFirst(o => o.id > 0);
        //var masterOrder2 = _masterOrderDao.SimpleClient.GetFirst(o => o.ProductTypeId == 6);
        //var goods2 = _coreCmsGoodsDao.SimpleClient.GetFirst(o => o.id > 0);
        //var masterOrder3 = _masterOrderDao.GetMasterOrderByOrderNumber("20220411200256837545");
        //var goods3 = _coreCmsGoodsDao.SimpleClient.GetFirst(o => o.id > 0);
        //var masterOrder4 = _masterOrderDao.GetMasterOrderByOrderNumber("20220411200256837545");
    }
}