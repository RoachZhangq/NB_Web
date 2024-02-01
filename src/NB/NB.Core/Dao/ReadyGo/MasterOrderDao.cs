/* 本文件自动生成
* 可能被覆盖，请勿修改
*/ 
using Furion.DependencyInjection;
using SqlSugar;
namespace NB.Core.Dao.ReadyGo
{
    ///<summary>
    ///
    ///</summary>
    public partial class MasterOrderDao : DbContext<NB.Core.Entity.ReadyGo.MasterOrder>, ITransient
    {		 
		 
		public MasterOrderDao(ISqlSugarClient sqlSugarClient) : base(sqlSugarClient)
        {			 
            //此处请勿修改，去此类的partial类里新增代码！
        }		   

    }
}