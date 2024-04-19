/*
 * 5. Write a program which creates two threads and a shared collection:
 * the first one should add 10 elements into the collection and the second should print all elements
 * in the collection after each adding.
 * Use Thread, ThreadPool or Task classes for thread creation and any kind of synchronization constructions.
 */
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading.Task5.Threads.SharedCollection
{
    class Program
    {
        const int MaxNumberOfElements = 10;
        static List<int> sharedCollection = new List<int>();
        static Mutex mutex = new Mutex();
        static async Task Main(string[] args)
        {
            Console.WriteLine("5. Write a program which creates two threads and a shared collection:");
            Console.WriteLine("the first one should add 10 elements into the collection and the second should print all elements in the collection after each adding.");
            Console.WriteLine("Use Thread, ThreadPool or Task classes for thread creation and any kind of synchronization constructions.");
            Console.WriteLine();

            new Thread(() =>
            {
                while (sharedCollection.Count < MaxNumberOfElements)
                {
                    mutex.WaitOne();
                    UpdateCollection();
                    mutex.ReleaseMutex();
                }
            }).Start();

            new Thread(() =>
            {
                while (sharedCollection.Count < MaxNumberOfElements)
                {
                    mutex.WaitOne();
                    PrintCollection();
                    mutex.ReleaseMutex();
                }

                PrintCollection();
            }).Start();

            Console.ReadLine();
        }

        static void UpdateCollection()
        {
            Console.WriteLine("Adding element into shared collection...");
            sharedCollection.Add(sharedCollection.Count);
        }

        static void PrintCollection()
        {
            Console.WriteLine("Printing collection:");
            sharedCollection.ForEach(x => Console.Write(x));
            Console.WriteLine();
        }

    }
}
