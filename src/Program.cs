using System;
using CacheLRU;
using System.Linq;
using System.Diagnostics;

namespace CacheLRU
{
    class Program
    {
        delegate void UpdateToTest(int key, int value);

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

            cache.Add(123, 321);

            var keys = cache.Keys.ToArray();
            UpdateToTest updates =  cache.Update;

            foreach (var up in updates.GetInvocationList())
            {
                MeasureUpdate(keys, (UpdateToTest) up);
            }

            foreach (var item in cache)
            {
                Console.WriteLine($"Key: {item.Key}\tValue:{item.Value}");
            }

            var enumerator = cache.GetEnumerator();

        }

        private static void MeasureUpdate(int[] keys, UpdateToTest up)
        {
            Stopwatch sw = new Stopwatch();

            var rand_key = new Random(37);
            var rand_val = new Random(3);
            const uint total_iterations = 100_000_000;

            sw.Start();
            for (uint i = 0; i < total_iterations; i++)
            {
                var index = keys[rand_key.Next(keys.Length)];
                up(index, rand_val.Next());
            }

            sw.Stop();

            Console.WriteLine($"{up.Method}: Ellapsed time in ms {sw.ElapsedMilliseconds}");
        }
    }
}
