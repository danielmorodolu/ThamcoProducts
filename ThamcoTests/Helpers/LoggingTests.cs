using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Polly;
using Xunit;
using ThamcoProducts.ProductRepository; 


namespace ThamcoTests.Helpers
{
    public class FakeProductServiceTests
    {
        private readonly ILogger<FakeProductServiceTests> _logger;

        public FakeProductServiceTests()
        {
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            _logger = loggerFactory.CreateLogger<FakeProductServiceTests>();
        }

        [Fact]
        public async Task TestRetryPolicyWithLogging()
        {
            // Arrange: Create a fake service with 3 transient failures
            var fakeService = new FakeProductService(maxFailuresBeforeSuccess: 3);

            var retryPolicy = Policy
                .Handle<HttpRequestException>()
                .WaitAndRetryAsync(
                    retryCount: 5,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(1),
                    onRetry: (exception, timespan, retryCount, context) =>
                    {
                        _logger.LogWarning(
                            "Retry {RetryCount} after {Delay}s due to error: {ExceptionMessage}",
                            retryCount,
                            timespan.TotalSeconds,
                            exception.Message
                        );
                    });

            // Act
            var result = await retryPolicy.ExecuteAsync(() => fakeService.GetProductsAsync());

            // Assert: Verify the results
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            _logger.LogInformation("Test completed successfully.");
        }
    }
}
