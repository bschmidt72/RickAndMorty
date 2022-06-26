using System;

namespace webapi.Cache
{
    public class CacheEntry<T>
    {
        public int Key { get; set; }
        public T Value { get; set; }
            
        public int Accessed { get; set; }

        public DateTime LastAccessed { get; set; }
        
        public CacheEntry(int key, T value)
        {
            Key = key;
            Value = value;
            Accessed = 0;
            LastAccessed = DateTime.Now;
        }
    }
}