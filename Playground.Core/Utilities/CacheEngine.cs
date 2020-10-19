using System;
using System.Linq;
using System.Runtime.Caching;

namespace Playground.Core.Utilities
{
    public class CacheEngine
    {
        private CacheEngine()
        {
        }

        public static readonly CacheEngine Instance = new CacheEngine();

        public void AddItem<T>(string key, T item) where T : class
        {
            lock (this)
            {
                MemoryCache.Default.Add(key, item, DateTimeOffset.Now.AddHours(1)); //caching for 1 hour!
            }
        }

        public T GetItem<T>(string key) where T : class
        {
            try
            {
                lock (this)
                {
                    return (T)MemoryCache.Default[key];
                }
            }
            catch
            {
                return null;
            }
        }

        public void RemoveItem(string key)
        {
            lock (this)
            {
                MemoryCache.Default.Remove(key);
            }
        }

        public void RemoveAllItems()
        {
            lock (this)
            {
                var cacheKeys = MemoryCache.Default.Select(kvp => kvp.Key).ToList();

                foreach (string cacheKey in cacheKeys)
                {
                    MemoryCache.Default.Remove(cacheKey);
                }
            }
        }
    }

    #region Not-Used
    /*
    public class CacheEngine
    {
        private CacheEngine()
        {
        }

        public static CacheEngine Instance { get; } = new CacheEngine();

        public void AddItem<T>(string key, T item) where T : class
        {
            lock (this)
            {
                MemoryCache.Default.Add(key, item, DateTimeOffset.Now.AddHours(3)); //caching for three hour!
            }
        }

        public T GetItem<T>(string key) where T : class
        {
            try
            {
                lock (this)
                {
                    return (T)MemoryCache.Default[key];
                }
            }
            catch
            {
                return null;
            }
        }

        public void RemoveItem(string key)
        {
            lock (this)
            {
                MemoryCache.Default.Remove(key);
            }
        }

        public void RemoveAllItems()
        {
            lock (this)
            {
                var cacheKeys = MemoryCache.Default.Select(kvp => kvp.Key).ToList();

                foreach (string cacheKey in cacheKeys)
                {
                    MemoryCache.Default.Remove(cacheKey);
                }
            }
        }
    }
    */
    #endregion

    /*Not used!
    public abstract class CachingBase
    {
        protected ObjectCache Cache = MemoryCache.Default;

        static readonly object _lock = new object();

        protected virtual void AddItem<T>(string key, T item) where T : class
        {
            lock (_lock)
            {
                Cache.Add(key, item, DateTimeOffset.Now.AddHours(1)); //caching for one hour!
            }
        }

        protected virtual T GetItem<T>(string key) where T : class
        {
            try
            {
                lock (_lock)
                {
                    return (T)Cache[key];
                }
            }
            catch
            {
                return null;
            }
        }

        protected virtual void RemoveItem(string key)
        {
            lock (_lock)
            {
                Cache.Remove(key);
            }
        }

        protected virtual void RemoveAllItems()
        {
            lock (_lock)
            {
                var cacheKeys = Cache.Select(kvp => kvp.Key).ToList();

                if (cacheKeys.Count == 0) return;

                foreach (string cacheKey in cacheKeys)
                {
                    Cache.Remove(cacheKey);
                }
            }
        }
    }
    */
}
