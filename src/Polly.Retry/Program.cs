using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Polly.Retry
{
    class Program
    {
        static void Main(string[] args)
        {
            // Only for tests.
            Retry().GetAwaiter().GetResult();

            Console.ReadKey();
        }

        static async Task Retry()
        {
            var retryPolicy = Policy.Handle<Exception>()
                .WaitAndRetryAsync(retryCount: 5, 
                                   sleepDurationProvider: retryAgain => TimeSpan.FromSeconds(1));

            try
            {
                await retryPolicy.ExecuteAsync(async () =>
                {
                    // Request invalid URL to simulate retry.
                    await Request("foo.bar");
                });
            }
            catch (Exception)
            {
                Console.WriteLine("Retry max attempts exceeded.");

                // Request real URL to ensure success.
                await Request("http://httpbin.org/get");
            }
        }

        static async Task Request(string url)
        {
            Console.WriteLine($"Request URL: {url}");

            var client = new HttpClient();
            var response = await client.GetStringAsync(url);

            Console.WriteLine($"Response: {response}");
        }
    }
}
