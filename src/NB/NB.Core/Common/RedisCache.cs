using System;
using System.Collections.Generic;
using Furion;
using Furion.DependencyInjection;
using NB.Core.Options;
using ServiceStack.Redis;
using SqlSugar;

namespace NB.Core.Common;

public class RedisCache : ICacheService
{
    private readonly ServiceStackRedis service;
    //public RedisCache(string host, int port, string password, int poolTimeOutSeconds, long db, string prefixKey)
    //{
    //    service = new ServiceStackRedis(host, port, password, poolTimeOutSeconds, db, prefixKey);
    //}

    //public RedisCache(string host)
    //{
    //    service = new ServiceStackRedis(host);
    //}
    public RedisCache(ServiceStackRedis _service)
    {
        service = _service;
    }
    //public RedisCache()
    //{
    //    service = new ServiceStackRedis();
    //}

    public void Add<V>(string key, V value)
    {
        service.Set(key, value);
    }

    public void Add<V>(string key, V value, int cacheDurationInSeconds)
    {
        service.Set(key, value, cacheDurationInSeconds);
    }

    public bool ContainsKey<V>(string key)
    {
        return service.ContainsKey(key);
    }

    public V Get<V>(string key)
    {
        return service.Get<V>(key);
    }

    public IEnumerable<string> GetAllKey<V>()
    {
        return service.GetAllKeys();
    }

    public V GetOrCreate<V>(string cacheKey, Func<V> create, int cacheDurationInSeconds = int.MaxValue)
    {
        if (ContainsKey<V>(cacheKey)) return Get<V>(cacheKey);

        var result = create();
        Add(cacheKey, result, cacheDurationInSeconds);
        return result;
    }

    public void Remove<V>(string key)
    {
        service.Remove(key);
    }
}

public class ServiceStackRedis : ISingleton
{
    private readonly int _poolTimeOutSeconds = -1;
    private readonly string _prefixKey;

    /// <summary>
    ///     redis客户端连接池
    ///     设置为public
    ///     外部如果使用分布式锁
    ///     官网参考:https://docs.servicestack.net/redis/distributed-locking#example-acquiring-a-lock-with-time-out
    ///     其他参考(有源码):https://www.cnblogs.com/lhll/articles/10869153.html
    /// </summary>
    public readonly PooledRedisClientManager _redisClientManager;

    private readonly SerializeService _serializeService = new();

    public ServiceStackRedis()
    {
        var redisOption = App.GetOptions<RedisOptions>();
        _poolTimeOutSeconds = redisOption.PoolTimeOutSeconds;
        _prefixKey = redisOption.PrefixKey;
        var hosts = new[] { string.Format("{0}@{1}:{2}", redisOption.Password, redisOption.Host, redisOption.Port) };
        _redisClientManager =
            new PooledRedisClientManager(hosts, hosts, null, redisOption.DbIndex, 500, _poolTimeOutSeconds);
    }

    //public ServiceStackRedis(string host)
    //    : this(host, 6379, null, -1, 0, "")
    //{
    //}

    //public ServiceStackRedis(RedisOptions redisOptions)
    //    : this(redisOptions.Host, redisOptions.Port, redisOptions.Password, redisOptions.PoolTimeOutSeconds, redisOptions.DbIndex, redisOptions.PrefixKey)
    //{
    //}

    public bool Set(string key, object value)
    {
        if (key == null) throw new ArgumentNullException("key");
        if (_poolTimeOutSeconds != -1) return Set(key, value, _poolTimeOutSeconds);
        key = _prefixKey + key;
        var json = _serializeService.SerializeObject(value);
        using (var client = _redisClientManager.GetClient())
        {
            return client.Set(key, json);
        }
    }

    public bool Set(string key, object value, int duration)
    {
        if (key == null) throw new ArgumentNullException("key");
        key = _prefixKey + key;
        var json = _serializeService.SerializeObject(value);
        using (var client = _redisClientManager.GetClient())
        {
            if (duration == -1)
                return client.Set(key, json);
            return client.Set(key, json, DateTime.Now.AddSeconds(duration));
        }
    }

    public bool Set(string key, object value, DateTime expiry)
    {
        if (key == null) throw new ArgumentNullException("key");
        key = _prefixKey + key;
        var json = _serializeService.SerializeObject(value);
        using (var client = _redisClientManager.GetClient())
        {
            if (expiry <= DateTime.Now) return client.Remove(key);
            return client.Set(key, json, expiry);
        }
    }

    public T Get<T>(string key)
    {
        if (key == null) throw new ArgumentNullException("key");
        key = _prefixKey + key;
        string data;
        using (var client = _redisClientManager.GetClient())
        {
            data = client.Get<string>(key);
        }

        return data == null ? default : _serializeService.DeserializeObject<T>(data);
    }

    public bool Remove(string key)
    {
        key = _prefixKey + key;
        using (var client = _redisClientManager.GetClient())
        {
            return client.Remove(key);
        }
    }

    public bool RemoveAll()
    {
        using (var client = _redisClientManager.GetClient())
        {
            try
            {
                client.FlushDb();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    public bool ContainsKey(string key)
    {
        key = _prefixKey + key;
        using (var client = _redisClientManager.GetClient())
        {
            return client.ContainsKey(key);
        }
    }

    public List<string> GetAllKeys()
    {
        using (var client = _redisClientManager.GetClient())
        {
            return client.SearchKeys(_prefixKey + "SqlSugarDataCache.*");
        }
    }

    public bool PushItemToList(string key, string value)
    {
        key = _prefixKey + key;
        using (var client = _redisClientManager.GetClient())
        {
            try
            {
                client.PushItemToList(key, value);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    public string PopItemFromList(string key)
    {
        key = _prefixKey + key;
        string data;
        using (var client = _redisClientManager.GetClient())
        {
            data = client.PopItemFromList(key);
        }

        return data == null ? "" : data;
    }

    public List<string> GetRangeFromList(string key, int startingFrom, int endingAt)
    {
        key = _prefixKey + key;
        using (var client = _redisClientManager.GetClient())
        {
            return client.GetRangeFromList(key, startingFrom, endingAt);
        }
    }

    public List<string> GetRangeFromSortedList(string key, int startingFrom, int endingAt)
    {
        key = _prefixKey + key;
        using (var client = _redisClientManager.GetClient())
        {
            return client.GetRangeFromSortedList(key, startingFrom, endingAt);
        }
    }

    public long RemoveItemFromList(string key, string value)
    {
        key = _prefixKey + key;
        using (var client = _redisClientManager.GetClient())
        {
            return client.RemoveItemFromList(key, value);
        }
    }

    public bool AddItemToSortedSet(string key, string value, double score)
    {
        key = _prefixKey + key;
        using (var client = _redisClientManager.GetClient())
        {
            return client.AddItemToSortedSet(key, value, score);
        }
    }

    public List<string> GetAllItemsFromSortedSet(string key)
    {
        key = _prefixKey + key;
        using (var client = _redisClientManager.GetClient())
        {
            return client.GetAllItemsFromSortedSet(key);
        }
    }

    public List<string> GetAllItemsFromSortedSetDesc(string key)
    {
        key = _prefixKey + key;
        using (var client = _redisClientManager.GetClient())
        {
            return client.GetAllItemsFromSortedSetDesc(key);
        }
    }

    public List<string> GetRangeFromSortedSetByHighestScore(string key, long fromScore, long toScore, int? skip,
        int? take)
    {
        key = _prefixKey + key;
        using (var client = _redisClientManager.GetClient())
        {
            return client.GetRangeFromSortedSetByHighestScore(key, fromScore, toScore, skip, take);
        }
    }

    public long GetSortedSetCount(string key, long fromScore, long toScore)
    {
        key = _prefixKey + key;
        using (var client = _redisClientManager.GetClient())
        {
            return client.GetSortedSetCount(key, fromScore, toScore);
        }
    }

    public bool RemoveItemFromSortedSet(string key, string value)
    {
        key = _prefixKey + key;
        using (var client = _redisClientManager.GetClient())
        {
            return client.RemoveItemFromSortedSet(key, value);
        }
    }

    public long RemoveRangeFromSortedSet(string key, int minRank, int maxRank)
    {
        key = _prefixKey + key;
        using (var client = _redisClientManager.GetClient())
        {
            return client.RemoveRangeFromSortedSet(key, minRank, maxRank);
        }
    }

    public bool SortedSetContainsItem(string key, string value)
    {
        key = _prefixKey + key;
        using (var client = _redisClientManager.GetClient())
        {
            return client.SortedSetContainsItem(key, value);
        }
    }

    public double IncrementItemInSortedSet(string key, string value, double incrementBy)
    {
        key = _prefixKey + key;
        using (var client = _redisClientManager.GetClient())
        {
            return client.IncrementItemInSortedSet(key, value, incrementBy);
        }
    }

    public double IncrementItemInSortedSet(string key, string value, long incrementBy)
    {
        key = _prefixKey + key;
        using (var client = _redisClientManager.GetClient())
        {
            return client.IncrementItemInSortedSet(key, value, incrementBy);
        }
    }


    public List<T> GetValues<T>(List<string> keys)
    {
        for (var i = 0; i < keys.Count; i++) keys[i] = _prefixKey + keys[i];
        using (var client = _redisClientManager.GetClient())
        {
            return client.GetValues<T>(keys);
        }
    }

    public long Increment(string key, DateTime expiry)
    {
        long seed = 0;
        using (var client = _redisClientManager.GetClient())
        {
            key = _prefixKey + key;
            seed = client.Increment(key, 1);
            client.ExpireEntryAt(key, expiry);
        }

        return seed;
    }
}