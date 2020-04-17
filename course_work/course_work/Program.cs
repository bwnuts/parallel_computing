using System;
using System.Collections.Generic;
using System.Linq;

namespace course_work
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Console.Write("Enter number of threads (N): ");
            int threadNumber = Convert.ToInt16(Console.ReadLine());
            Console.WriteLine("Processing...");
            
            IOrderedEnumerable<KeyValuePair<string, List<string>>>[] indexes=new IOrderedEnumerable<KeyValuePair<string, List<string>>>[threadNumber];
            long[] workTime = new long[threadNumber];
            long max = 0, min = long.MaxValue;
            int iMin = 0;
            int repeat = 1;
            for (int i = 0; i < threadNumber; i++)
            {
                for (int j = 0; j < repeat; j++)
                {
                    long start = DateTime.Now.Ticks;
                    indexes[i]=Indexer.GetIndex(i + 1);
                    workTime[i] += DateTime.Now.Ticks - start;
                }

                long average = workTime[i] / repeat;
                if (average > max)
                    max = average;
                if (average < min)
                {
                    min = average;
                    iMin = i;
                }
                
                Console.WriteLine($"Average {i+1}-thread work time - {average}");
            }
            
            Console.WriteLine("\n***Time comparision");
            int stars = 100;
            foreach (var time in workTime)
            {
                long curStars = (stars * time) / (max * repeat);
                for (int j = 0; j < curStars; j++)
                {
                    Console.Write("*");
                }
                Console.Write("\n");
            }
            
            Console.WriteLine($"Best performance - {iMin+1} threads");

            /*var first = indexes[0].ToArray();
            for (int i = 1; i < indexes.Length; i++)
            {
                var current = indexes[i].ToArray();
                for (int j = 0; j < current.Length; j++)
                {
                    if (first[j].Key == current[j].Key)
                    {
                        if (!Enumerable.SequenceEqual(first[j].Value.OrderBy(t=>t)
                            ,current[j].Value.OrderBy(t=>t)))
                        {
                            Console.WriteLine("NOT EQUAL");
                        }
                    }
                    else
                    {
                        Console.WriteLine("DIFFERENT KEYS");
                    }
                }
            }*/
        }
    }
}