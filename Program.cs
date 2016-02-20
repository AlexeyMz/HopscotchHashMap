using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HopscotchHashMap
{
    static class Program
    {
        static void Main(string[] args)
        {
            
            Console.ReadLine();
        }

        static void SimpleTest()
        {
            var map = new HopscotchMap<int, string>(100, 4);
            var usedKeys = new HashSet<int>();

            Random random = new Random(0);
            for (int i = 0; i < 10; i++)
            {
                int key = random.Next(1000);
                map.PutIfAbsent(key, Stringify(key));
                usedKeys.Add(key);
                Console.WriteLine($"Added {key}");
            }

            foreach (int key in usedKeys)
            {
                string value;
                if (map.Remove(key, out value))
                    Console.WriteLine($"Removed {key}: {value}");
                else
                    Console.WriteLine($"Failed to remove {key}");
            }

            Console.WriteLine($"End count: {map.Count}");
        }

        static string Stringify(int value)
        {
            return "s" + value.ToString();
        }
    }
}
