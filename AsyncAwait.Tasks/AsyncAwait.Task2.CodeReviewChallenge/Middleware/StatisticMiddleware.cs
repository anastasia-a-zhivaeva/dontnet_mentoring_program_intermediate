using System;
using System.Threading;
using System.Threading.Tasks;
using AsyncAwait.Task2.CodeReviewChallenge.Headers;
using CloudServices.Interfaces;
using Microsoft.AspNetCore.Http;

namespace AsyncAwait.Task2.CodeReviewChallenge.Middleware;

public class StatisticMiddleware
{
    private readonly RequestDelegate _next;

    private readonly IStatisticService _statisticService;

    public StatisticMiddleware(RequestDelegate next, IStatisticService statisticService)
    {
        _next = next;
        _statisticService = statisticService ?? throw new ArgumentNullException(nameof(statisticService));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        string path = context.Request.Path;

        /*
         * 1. Task.Run() should be used for CPU bound tasks and not I/O bound. In StatisticMiddleware case, all tasks are I/O bound (requests to the backend server). Instead, I can await on Tasks returned from _statisticService.
         * 2. If I remove Thread.Sleep(3000), the exception "System.ObjectDisposedException: 'IFeatureCollection has been disposed." is thrown. I think it happens because continuation created using OnComplete method starts after HttpContext is disposed.
⁠         * 3. ConfigureAwait(false) should not be here. It tells that it does not need the context, so the code can be run in any thread. But continuation can also be run without context and it uses HttpContext. It's not common, but it can happen and lead to unexpected errors.
         */

        /*
        var staticRegTask = Task.Run(
            () => _statisticService.RegisterVisitAsync(path)
                .ConfigureAwait(false)
                .GetAwaiter().OnCompleted(UpdateHeaders));
        Console.WriteLine(staticRegTask.Status); // just for debugging purposes

        void UpdateHeaders()
        {
            context.Response.Headers.Add(
                CustomHttpHeaders.TotalPageVisits,
                _statisticService.GetVisitsCountAsync(path).GetAwaiter().GetResult().ToString());
        }

        Thread.Sleep(3000); // without this the statistic counter does not work
        */

        await _statisticService.RegisterVisitAsync(path);
        var visitsCount = await _statisticService.GetVisitsCountAsync(path);
        context.Response.Headers.Add(CustomHttpHeaders.TotalPageVisits, visitsCount.ToString());
        await _next(context);
    }
}
