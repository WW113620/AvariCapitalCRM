
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace AvariCapitalCRM.Models.Utils
{
    public class RedisHelper : IDisposable
    {
        private readonly RedisOptions _redisOptions;
        private ConcurrentDictionary<string, ConnectionMultiplexer> _connections;

        public RedisHelper(RedisOptions redisOptions = null)
        {
            this._redisOptions = redisOptions;
            if (redisOptions == null) this._redisOptions = new RedisOptions();
            this._connections = new ConcurrentDictionary<string, ConnectionMultiplexer>();
        }

        /// <summary>
        /// 获取ConnectionMultiplexer
        /// </summary>
        /// <returns></returns>
        private ConnectionMultiplexer GetConnect()
        {
            return _connections.GetOrAdd(this._redisOptions.InstanceName, p => ConnectionMultiplexer.Connect(this._redisOptions.Connection));
        }

        /// <summary>
        /// 获取数据库
        /// </summary>
        /// <param name="configName"></param>
        /// <param name="db">默认为0：优先代码的db配置，其次config中的配置</param>
        /// <returns></returns>
        public IDatabase GetDatabase()
        {
            return GetConnect().GetDatabase(this._redisOptions.DefaultDB);
        }

        #region func
        public T Do<T>(Func<IDatabase, T> func)
        {
            var database = GetDatabase();
            return func(database);
        }

        public string ConvertJson<T>(T value)
        {
            string result = value is string ? value.ToString() : JsonConvert.SerializeObject(value);
            return result;
        }

        public T ConvertObj<T>(RedisValue value)
        {
            if (value.IsNull)
                return default(T);
            return JsonConvert.DeserializeObject<T>(value);
        }

        public string AddSysCustomKey(string old)
        {
            var fixkey = this._redisOptions.RedisKey;
            return fixkey + old;
        }

        public string StringGet(string key)
        {
            key = AddSysCustomKey(key);
            return Do(db => db.StringGet(key));
        }

        public async Task<string> StringGetAsync(string key)
        {
            key = AddSysCustomKey(key);
            return await Do(db => db.StringGetAsync(key));
        }

        public bool StringSet(string key, string val, TimeSpan? exp = default(TimeSpan?))
        {
            key = AddSysCustomKey(key);
            return Do(db => db.StringSet(key, val, exp));
        }

        public async Task<bool> StringSetAsync(string key, string val, TimeSpan? exp = default(TimeSpan?))
        {
            key = AddSysCustomKey(key);
            return await Do(db => db.StringSetAsync(key, val, exp));
        }

        public T Get<T>(string key)
        {
            key = AddSysCustomKey(key);
            var val = Do(db => db.StringGet(key));
            return ConvertObj<T>(val);
        }

        public async Task<T> GetAsync<T>(string key)
        {
            key = AddSysCustomKey(key);
            string result = await Do(db => db.StringGetAsync(key));
            return ConvertObj<T>(result);
        }

        public bool Set<T>(string key, T obj, TimeSpan? exp = default(TimeSpan?))
        {
            key = AddSysCustomKey(key);
            string json = ConvertJson(obj);
            return Do(db => db.StringSet(key, json, exp));
        }

        public async Task<bool> SetAsync<T>(string key, T obj, TimeSpan? exp = default(TimeSpan?))
        {
            key = AddSysCustomKey(key);
            string json = ConvertJson(obj);
            return await Do(db => db.StringSetAsync(key, json, exp));
        }


        public bool Remove(string key)
        {
            key = AddSysCustomKey(key);
            return Do(db => db.KeyDelete(key));
        }

        public async Task<bool> RemoveAsync(string key)
        {
            key = AddSysCustomKey(key);
            return await Do(db => db.KeyDeleteAsync(key));
        }

        public long ListLength(string key)
        {
            key = AddSysCustomKey(key);
            return Do(db => db.ListLength(key));
        }


        public async Task<long> ListLengthAsync(string key)
        {
            key = AddSysCustomKey(key);
            return await Do(db => db.ListLengthAsync(key));
        }
        #endregion

        public IServer GetServer(string configName = null, int endPointsIndex = 0)
        {
            var confOption = ConfigurationOptions.Parse(this._redisOptions.Connection);
            return GetConnect().GetServer(confOption.EndPoints[endPointsIndex]);
        }

        public ISubscriber GetSubscriber(string configName = null)
        {
            return GetConnect().GetSubscriber();
        }

        public void Dispose()
        {
            if (_connections != null && _connections.Count > 0)
            {
                foreach (var item in _connections.Values)
                {
                    item.Close();
                }
            }
        }

    }

    public class RedisOptions
    {
        public string Connection { get; set; } = ConfigHelper.GetConfigValue("RedisConnection");
        public string InstanceName { get; set; } = ConfigHelper.GetConfigValue("RedisInstanceName");
        public int DefaultDB { get; set; } = ConfigHelper.GetConfigValue("RedisDefaultDB").ToInt(0);
        public string RedisKey { get; set; } = ConfigHelper.GetConfigValue("RedisKey");
    }


}
