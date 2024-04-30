using System;
using System.Net.Http;
using System.Threading.Tasks;
using CloudServices.Interfaces;

namespace AsyncAwait.Task2.CodeReviewChallenge.Models.Support;

public class ManualAssistant : IAssistant
{
    private readonly ISupportService _supportService;

    public ManualAssistant(ISupportService supportService)
    {
        _supportService = supportService ?? throw new ArgumentNullException(nameof(supportService));
    }

    /*
     * Same problems like in StatisticMiddleware
     */
    public async Task<string> RequestAssistanceAsync(string requestInfo)
    {
        /*
         * try
        {
            var t = _supportService.RegisterSupportRequestAsync(requestInfo);
            Console.WriteLine(t.Status); // this is for debugging purposes
            Thread.Sleep(5000); // this is just to be sure that the request is registered
            return await _supportService.GetSupportInfoAsync(requestInfo)
                .ConfigureAwait(false);
        }
        catch (HttpRequestException ex)
        {
            return await Task.Run(async () =>
                await Task.FromResult($"Failed to register assistance request. Please try later. {ex.Message}"));
        }
         */
        try
        {
            await _supportService.RegisterSupportRequestAsync(requestInfo);
            return await _supportService.GetSupportInfoAsync(requestInfo);
        }
        catch (HttpRequestException ex)
        {
            return $"Failed to register assistance request. Please try later. {ex.Message}";
        }
    }
}
