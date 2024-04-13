/*
*  Create a Task and attach continuations to it according to the following criteria:
   a.    Continuation task should be executed regardless of the result of the parent task.
   b.    Continuation task should be executed when the parent task finished without success.
   c.    Continuation task should be executed when the parent task would be finished with fail and parent task thread should be reused for continuation
   d.    Continuation task should be executed outside of the thread pool when the parent task would be cancelled
   Demonstrate the work of the each case with console utility.
*/
using System;
using System.Threading.Tasks;
using System.Threading;

namespace MultiThreading.Task6.Continuation
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Create a Task and attach continuations to it according to the following criteria:");
            Console.WriteLine("a.    Continuation task should be executed regardless of the result of the parent task.");
            Console.WriteLine("b.    Continuation task should be executed when the parent task finished without success.");
            Console.WriteLine("c.    Continuation task should be executed when the parent task would be finished with fail and parent task thread should be reused for continuation.");
            Console.WriteLine("d.    Continuation task should be executed outside of the thread pool when the parent task would be cancelled.");
            Console.WriteLine("Demonstrate the work of the each case with console utility.");
            Console.WriteLine();

            var failedParentTask = Task.Run(() =>
                {
                    Task.Delay(2000).Wait();
                    throw new Exception("Parent task failed.");
                });

            var regardlessResult = failedParentTask.ContinueWith(_ => Console.WriteLine("Continuation task executed regardless of parent task result."));
            var withoutSuccess = failedParentTask.ContinueWith(_ => Console.WriteLine("Continuation task executed when the parent task finishes without success."), TaskContinuationOptions.NotOnRanToCompletion);
            var failedAndReused = failedParentTask.ContinueWith(_ => Console.WriteLine("Continuation task executed when the parent task fails (reusing parent task thread)."), TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously);

            try
            {
                Task.WaitAll(failedParentTask, regardlessResult, withoutSuccess, failedAndReused);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            var parentCancellationTokenSource = new CancellationTokenSource();
            var parentCancellationToken = parentCancellationTokenSource.Token;

            Task cancelledParentTask = Task.Run(() =>
            {
                Task.Delay(2000).Wait();
                // simulate long running operation
                while (true)
                {
                    if (parentCancellationToken.IsCancellationRequested)
                    {
                        break;
                    }
                }
            }, parentCancellationToken);


            var continuationCancellationTokenSource = new CancellationTokenSource();
            var continuationCancellationToken = continuationCancellationTokenSource.Token;

            var cancelledAndOutside = cancelledParentTask.ContinueWith(_ =>
            {
                Console.WriteLine("Continuation task executed outside of the thread pool when parent task is canceled.");
            }, continuationCancellationToken, TaskContinuationOptions.OnlyOnCanceled, TaskScheduler.Default);

            parentCancellationTokenSource.Cancel();

            try
            {
                Task.WaitAll(cancelledParentTask, cancelledAndOutside);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ReadLine();
        }
    }
}
