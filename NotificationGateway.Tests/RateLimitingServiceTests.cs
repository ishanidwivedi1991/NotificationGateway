using static NotificationGateway.Application.NotificationGatewayService;

namespace NotificationGateway.Tests
{
    public class RateLimitingServiceTests
    {
        private readonly IRateLimitingService _rateLimitingService;

        public RateLimitingServiceTests()
        {
            _rateLimitingService = new RateLimitingService();
        }

        [Fact]
        public async Task CanSendNotification_ShouldReturnTrue_WhenLimitsNotExceeded()
        {
            var result = await _rateLimitingService.CanSendNotification("1234567890", "account1");
            Assert.True(result);
        }

        [Fact]
        public async Task CanSendNotification_ShouldReturnFalse_WhenPerNumberLimitExceeded()
        {
            // Simulate multiple calls within the same second
            for (int i = 0; i < 5; i++)
            {
                await _rateLimitingService.CanSendNotification("1234567890", "account1");
            }

            var result = await _rateLimitingService.CanSendNotification("1234567890", "account1");
            Assert.False(result);  // Should exceed the per-number limit
        }

        [Fact]
        public async Task CanSendNotification_ShouldReturnFalse_WhenPerAccountLimitExceeded()
        {
            // Simulate multiple calls within the same second
            for (int i = 0; i < 10; i++)
            {
                await _rateLimitingService.CanSendNotification("1234567890", "account1");
            }

            var result = await _rateLimitingService.CanSendNotification("1234567890", "account1");
            Assert.False(result);  // Should exceed the per-account limit
        }
    }
}