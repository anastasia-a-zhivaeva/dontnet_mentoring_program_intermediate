using System.Threading.Tasks;

namespace AsyncAwait.Task2.CodeReviewChallenge.Services;

public class PrivacyDataService : IPrivacyDataService
{
    /*
     * Using ValueTask<T> here didn't make any sense because it was converted to Task anyway
     * The method return value should be ValueTask<T> (it has some limitation, for example, it can't be awaited more that once and it's size is larger than size of Task because it contains 2 fields instead of 1)
     * Or use Task.FromResult() (I prefer this option)
     */
    public Task<string> GetPrivacyDataAsync()
    {
        /*
         * return new ValueTask<string>("This Policy describes how async/await processes your personal data," +
         *                           "but it may not address all possible data processing scenarios.").AsTask();
         */
        return Task.FromResult("This Policy describes how async/await processes your personal data," +
                                     "but it may not address all possible data processing scenarios.");
    }
}
