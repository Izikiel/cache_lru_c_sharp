using System;
using CacheLRU;

namespace CacheLRU
{
    class Program
    {
        static void Main(string[] args)
        {
            var cache = new CacheLRU<int, int>(10);
            for (int i = 0; i < 10; i++)
            {
                cache.Add(i, i * i);
            }

            Console.WriteLine($"0 in cache? {cache.ContainsKey(0)}");

            cache.Add(11, 11 * 11);

            Console.WriteLine($"0 in cache? {cache.ContainsKey(0)}");
            int res = 13;

            cache.Remove(3, out res);

            Console.WriteLine($"Removed 3 and got {res}");

            cache.Remove(3, out res);

            Console.WriteLine($"Tried to remove 3 again and got {res}");

            foreach (var item in cache)
            {
                Console.WriteLine($"Key: {item.Key}\tValue:{item.Value}");
            }

            var enumerator = cache.GetEnumerator();

        }
    }
}
