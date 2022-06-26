namespace webapi.Cache
{
    public interface ICache<T>
    {
        int Capacity { get; set; }

        int TimeOut { get; set; }
        
        int Count { get; }

        void Add(string key, T value);

        void Add(int key, T value);

        bool ContainsKey(string key);

        bool ContainsKey(int key);

        T Get(string key);

        T Get(int key);
    }
}