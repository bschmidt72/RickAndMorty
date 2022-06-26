using System;
using System.Collections.Generic;
using System.Threading;

namespace webapi.Cache
{
    public class LfuCache<T> : ICache<T>
    {
        private const int MaxCapacity = 100;
        
        private const int DefaultTimeOut = 3000;
        
        public int Capacity { get; set; }

        public int TimeOut { get; set; }

        public int Count => _lfuList.Count;
        
        private readonly ReaderWriterLock _lock = new ReaderWriterLock();
        private readonly Dictionary<int, CacheEntry<T>> _cache;
        private readonly List<CacheEntry<T>> _lfuList;

        public LfuCache(int? capacity = MaxCapacity, int? timeout = DefaultTimeOut)
        {
            Capacity = capacity ?? MaxCapacity;
            TimeOut = timeout ?? DefaultTimeOut;
            if (Capacity == 0)
            {
                throw new ArgumentException("Capacity must be > 0");
            }
            _cache = new Dictionary<int, CacheEntry<T>>();
            _lfuList = new List<CacheEntry<T>>();
        }
        
        public void Add(string key, T value)
        {
            Add(key.GetHashCode(), value);
        }
        
        public void Add(int key, T value)
        {
            _lock.AcquireWriterLock(TimeOut);
            try
            {
                _cache.TryGetValue(key, out var cached);
                if (cached != null)
                {
                    cached.Value = value;
                    return;
                }
                CheckAndPerformEvict();
                var newItem = new CacheEntry<T>(key, value);
                var added = _cache.TryAdd(key, newItem);
                if (!added)
                {
                    return;
                }
                _lfuList.Add(newItem);
                _lfuList.Sort((a, b) => a.Accessed - b.Accessed);
            }
            finally
            {
                _lock.ReleaseWriterLock();
            }
        }

        private void CheckAndPerformEvict()
        {
            if (Count < Capacity)
            {
                return;
            }
            var lfuCandidate = _lfuList[0];
            _lfuList.Remove(lfuCandidate);
            _cache.Remove(lfuCandidate.Key);
        }

        public bool ContainsKey(string key)
        {
            return ContainsKey(key.GetHashCode());
        }
        
        public bool ContainsKey(int key)
        {
            _lock.AcquireReaderLock(TimeOut);
            try
            {
                return _cache.ContainsKey(key);
            }
            finally
            {
                _lock.ReleaseReaderLock();
            } 
        }
        
        public T Get(string key)
        {
            return Get(key.GetHashCode());
        }

        public T Get(int key)
        {
            _lock.AcquireReaderLock(TimeOut);
            try
            {
                if (!_cache.TryGetValue(key, out var cached))
                {
                    return default;
                }
                cached.Accessed += 1;
                return cached.Value;
            }
            finally
            {
                _lock.ReleaseReaderLock();
            }
        }

    }
}