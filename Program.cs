using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HopscotchHashMap
{
    static class Program
    {
        static void Main(string[] args)
        {
            //for (int i = 0; i < 10000000; i += 169)
            //{
            //    try
            //    {
            //        ComplexTestInit(i);
            //    }
            //    catch (Exception)
            //    {
            //        Console.WriteLine(i);
            //        Console.ReadLine();
            //    }

            //}
            ComplexTestInit(543);
            Console.ReadLine();
        }

        static void SimpleTest()
        {
            var map = new HopscotchMap<int, int>(100, 4);
            var usedKeys = new HashSet<int>();

            Random random = new Random(0);
            for (int i = 0; i < 10; i++)
            {
                int key = random.Next(1000);
                map.PutIfAbsent(key, key);
                usedKeys.Add(key);
                Console.WriteLine($"Added {key}");
            }

            foreach (int key in usedKeys)
            {
                int value;
                if (map.Remove(key, out value))
                    Console.WriteLine($"Removed {key}: {value}");
                else
                    Console.WriteLine($"Failed to remove {key}");
            }

            Console.WriteLine($"End count: {map.ApproximateCount}");
        }

        static void ComplexTestInit(int idShift)
        {
            var map = new HopscotchMap<int, int>(10000, 8);
            var bag = new ConcurrentBag<int>();
            var completitionSources = new TaskCompletionSource<bool>[8];
            for (int i = 0; i < completitionSources.Length; i++)
            {
                int j = i;
                completitionSources[j] = new TaskCompletionSource<bool>();
                new Thread(() =>
                {
                    ComplexTest(map, bag, j + idShift);
                    completitionSources[j].SetResult(true);
                }).Start();
            }
            Task[] tasks = completitionSources.Select(t => t.Task).ToArray();
            Task.WaitAll(tasks);
            bool faulted = false;
            foreach (var task in tasks)
            {
                if (task.IsFaulted)
                {
                    faulted = true;
                    Console.WriteLine(task.Exception);
                }
            }
            if (!faulted)
            {
                Console.WriteLine($"map left: {map.ApproximateCount}, bag left: {bag.Count}");
            }
            Console.WriteLine("Finished");
        }

        static void ComplexTest(HopscotchMap<int, int> map, ConcurrentBag<int> bag, int id)
        {
            var random = new Random(id);
            var clone = map;
            for (int i = 0; i < map.Capacity * 1000; i++)
            {
                //lock (map)
                {
                    // (1/3) -> remove, (2/3) -> insert
                    if (random.Next(100) < 40 || bag.Count > map.Capacity * 2 / 3)
                    {
                        int element;
                        if (bag.TryTake(out element))
                        {
                            int data;
                            if (!map.Remove(element, out data))
                            {
                                throw new InvalidOperationException("Cannot remove element");
                                //var h = map.table.Select((x, ii) => new { x, ii }).Where(z =>
                                //    z.x.hash >= 0 && z.x.hash == (element.GetHashCode() & 0x7FFFFFFF)).ToArray();
                                //var b = 1;
                            }
                            if (data != element)
                                throw new InvalidOperationException("Data doesn't match key");
                        }
                    }
                    else
                    {
                        int key = random.Next(map.Capacity * 3);
                        //var origninal = map.Clone();
                        var result = map.PutIfAbsent(key, key);

                        if (result == PutResult.Success)
                        {
                            if (!map.ContainsKey(key))
                            {
                                //origninal.PutIfAbsent(key, Stringify(key));
                                //origninal.ContainsKey(key);
                                //var h = map.table.Select((x, ii) => new { x, ii }).Where(z =>
                                //    z.x.hash >= 0 && z.x.hash == (key.GetHashCode() & 0x7FFFFFFF)).ToArray();
                                //var b = 1;
                                throw new InvalidOperationException("no key after insert");
                            }
                            bag.Add(key);
                        }
                    }
                }
            }
        }
    }
}
