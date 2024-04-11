/*
 * 4.	Write a program which recursively creates 10 threads.
 * Each thread should be with the same body and receive a state with integer number, decrement it,
 * print and pass as a state into the newly created thread.
 * Use Thread class for this task and Join for waiting threads.
 * 
 * Implement all of the following options:
 * - a) Use Thread class for this task and Join for waiting threads.
 * - b) ThreadPool class for this task and Semaphore for waiting threads.
 */

using System;
using System.Threading;

namespace MultiThreading.Task4.Threads.Join
{
    class Program
    {
        const int initialState = 10; 
        static Semaphore semaphore = new Semaphore(0, initialState);
        static void Main(string[] args)
        {
            Console.WriteLine("4.	Write a program which recursively creates 10 threads.");
            Console.WriteLine("Each thread should be with the same body and receive a state with integer number, decrement it, print and pass as a state into the newly created thread.");
            Console.WriteLine("Implement all of the following options:");
            Console.WriteLine();
            Console.WriteLine("- a) Use Thread class for this task and Join for waiting threads.");
            Console.WriteLine("- b) ThreadPool class for this task and Semaphore for waiting threads.");

            Console.WriteLine();

            Console.WriteLine("Thread + Join implementation: ");
            Thread initialThread = new Thread(JoinWorker);
            initialThread.Start(initialState);
            initialThread.Join();

            Console.WriteLine("ThreadPool + Semaphore implementation: ");
            ThreadPool.QueueUserWorkItem(SemaphoreWorker, initialState);
            semaphore.WaitOne();

            Console.ReadLine();
        }

        static void JoinWorker(object state)
        {
            int currentState = (int)state;
            Console.WriteLine(currentState);

            if (currentState > 0)
            {
                int newState = currentState - 1;
                Thread newThread = new Thread(JoinWorker);
                newThread.Start(newState);
                newThread.Join();
            }
        }

        static void SemaphoreWorker(object state)
        {
            int currentState = (int)state;
            Console.WriteLine(currentState);

            if (currentState > 0)
            {
                int newState = currentState - 1;
                ThreadPool.QueueUserWorkItem(SemaphoreWorker, newState);
            }

            semaphore.Release();
        }
    }
}
