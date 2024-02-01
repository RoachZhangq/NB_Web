using Furion;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NB.Core.Dao;

/// <summary>
/// 
/// </summary>
public class DbContext<TEntity> where TEntity : class, new()
{
    private SqlSugarScope _sqlSugarScope;
    //private SimpleClient<TEntity> _simpleClient;


    public DbContext(ISqlSugarClient sqlSugarClient)
    {
        this._sqlSugarScope = sqlSugarClient as SqlSugarScope;
        //_simpleClient = db.GetSimpleClient<TEntity>();
    }

    /// <summary>
    /// 封装简单的CRUD
    /// </summary>
    public SimpleClient<TEntity> SimpleClient
    {
        get
        {

            //var sqlSugarProvider = this.db.AsTenant().GetConnectionScopeWithAttr<TEntity>();
            //return sqlSugarProvider.GetSimpleClient<TEntity>();

            var simpleClient = DB.GetSimpleClient<TEntity>();

            return simpleClient;
        }
    }
    /// <summary>
    /// 核心对象：拥有完整的SqlSugar全部功能 --- 推荐操作
    /// </summary>
    protected SqlSugarScopeProvider DB
    {
        get
        {
            //var sqlSugarProvider = this.db.AsTenant().GetConnectionScopeWithAttr<TEntity>();

            var sqlSugarScopeProvider = _sqlSugarScope.GetConnectionScopeWithAttr<TEntity>();
            return sqlSugarScopeProvider;
        }
    }
    #region 补充方法

    public int UpdateExecuteCommand(TEntity entity, Expression<Func<TEntity, object>> columns)
    {

        return DB.Updateable(entity).UpdateColumns(columns).ExecuteCommand();
    }
    public int UpdateExecuteCommand(List<TEntity> entitys, Expression<Func<TEntity, object>> columns)
    {
        return DB.Updateable(entitys).UpdateColumns(columns).ExecuteCommand();
    }

    public int UpdateExecuteCommand(TEntity entity, Expression<Func<TEntity, object>> updateColumns, Expression<Func<TEntity, object>> whereColumns)
    {
        return DB.Updateable(entity).UpdateColumns(updateColumns).WhereColumns(whereColumns).ExecuteCommand();
    }

    public int UpdateExecuteCommand(List<TEntity> entitys, Expression<Func<TEntity, object>> updateColumns, Expression<Func<TEntity, object>> whereColumns)
    {
        return DB.Updateable(entitys).UpdateColumns(updateColumns).WhereColumns(whereColumns).ExecuteCommand();
    }


    #endregion
}
