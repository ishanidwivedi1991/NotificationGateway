using System.Collections.Concurrent;

namespace NotificationGateway.Application
{
    public class NotificationGatewayService
    {
        public interface IRateLimitingService
        {
            Task<bool> CanSendNotification(string phoneNumber, string accountId);
            Task<List<DateTime>> GetBusinessNotification(string businessPhoneNumber);

            Task<List<DateTime>> GetAccountNotification(string businessPhoneNumber);
        }

        public class RateLimitingService : IRateLimitingService
        {
            // In-memory stores for tracking
            private readonly ConcurrentDictionary<string, List<DateTime>> _numberRateLimits = new ConcurrentDictionary<string, List<DateTime>>();
            private readonly ConcurrentDictionary<string, List<DateTime>> _accountRateLimits = new ConcurrentDictionary<string, List<DateTime>>();

            // The rate limit thresholds
            // This can be fetched from external provide API in real-time projects
            private const int MaxMessagesPerSecondPerNumber = 5;
            private const int MaxMessagesPerSecondPerAccount = 10;

            public async Task<bool> CanSendNotification(string phoneNumber, string accountId)
            {
                var currentTime = DateTime.UtcNow;

                // Check the business phone number rate limit
                if (!CheckRateLimit(_numberRateLimits, phoneNumber, currentTime, MaxMessagesPerSecondPerNumber))
                    return false;

                // Check the account-wide rate limit
                if (!CheckRateLimit(_accountRateLimits, accountId, currentTime, MaxMessagesPerSecondPerAccount))
                    return false;

                // Record the new message for both the phone number and account
                RecordMessage(_numberRateLimits, phoneNumber, currentTime);
                RecordMessage(_accountRateLimits, accountId, currentTime);

                return true;
            }

            private bool CheckRateLimit(ConcurrentDictionary<string, List<DateTime>> rateLimits, string key, DateTime currentTime, int maxMessagesPerSecond)
            {
                if (!rateLimits.ContainsKey(key))
                    rateLimits[key] = new List<DateTime>();

                // Clean up old timestamps (older than 10 second)
                rateLimits[key] = rateLimits[key].Where(t => (currentTime - t).TotalSeconds < 10).ToList();

                // Check if the number of messages exceeds the limit
                return rateLimits[key].Count < maxMessagesPerSecond;
            }

            private void RecordMessage(ConcurrentDictionary<string, List<DateTime>> rateLimits, string key, DateTime currentTime)
            {
                if (!rateLimits.ContainsKey(key))
                    rateLimits[key] = new List<DateTime>();

                rateLimits[key].Add(currentTime);
            }

            public Task<List<DateTime>> GetBusinessNotification(string businessPhoneNumber)
            {               
                if (_numberRateLimits.ContainsKey(businessPhoneNumber))
                    return Task.FromResult(_numberRateLimits[businessPhoneNumber]);

                return Task.FromResult(new List<DateTime> { });
            }

            public Task<List<DateTime>> GetAccountNotification(string accountId)
            {
                if (_accountRateLimits.ContainsKey(accountId))
                    return Task.FromResult(_accountRateLimits[accountId]);

                return Task.FromResult(new List<DateTime> { });
            }
        }
    }
}
