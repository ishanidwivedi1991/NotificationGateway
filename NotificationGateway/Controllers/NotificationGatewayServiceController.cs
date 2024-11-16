using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static NotificationGateway.Application.NotificationGatewayService;

namespace NotificationGateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationGatewayServiceController : ControllerBase
    {
        private readonly IRateLimitingService _rateLimitingService;

        public NotificationGatewayServiceController(IRateLimitingService rateLimitingService)
        {
            _rateLimitingService = rateLimitingService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendNotification([FromBody] NotificationRequest request)
        {
            var canSend = await _rateLimitingService.CanSendNotification(request.PhoneNumber, request.AccountId);

            if (!canSend)
            {
                return StatusCode(429, "Rate limit exceeded. Try again later.");
            }

            // Logic to send the message via external API
            // var result = await _smsProvider.SendSms(request);

            return Ok("Message can be sent successfully.");
        }

        [HttpGet("business/{businessPhoneNumber}")]
        public async Task<IActionResult> GetBusinessRateAsync(string businessPhoneNumber, DateTime? startDate, DateTime? endDate)
        {
            var BusinessRates = await _rateLimitingService.GetBusinessNotification(businessPhoneNumber);

            // Filter the data based on the provided date range
            var rates = BusinessRates.Count > 0 ? BusinessRates : new List<DateTime>();

            if (startDate.HasValue)
                rates = rates.Where(t => t >= startDate.Value).ToList();
            if (endDate.HasValue)
                rates = rates.Where(t => t <= endDate.Value).ToList();

            var result = rates
                .GroupBy(t => t.ToString("yyyy-MM-dd HH:mm:ss"))
                .Select(g => new { Time = g.Key, Count = g.Count() })
                .ToList();

            return Ok(result);
        }

        [HttpGet("account/{accountId}")]
        public async Task<IActionResult> GetAccountRate(string accountId, DateTime? startDate, DateTime? endDate)
        {
            var AccountRates = await _rateLimitingService.GetAccountNotification(accountId);

            // Filter the data based on the provided date range
            var rates = AccountRates.Count > 0 ? AccountRates : new List<DateTime>();

            if (startDate.HasValue)
                rates = rates.Where(t => t >= startDate.Value).ToList();
            if (endDate.HasValue)
                rates = rates.Where(t => t <= endDate.Value).ToList();

            var result = rates
                .GroupBy(t => t.ToString("yyyy-MM-dd HH:mm:ss"))
                .Select(g => new { Time = g.Key, Count = g.Count() })
                .ToList();

            return Ok(result);
        }
    }

    public class NotificationRequest
    {
        public string PhoneNumber { get; set; }
        public string AccountId { get; set; }
        public string Message { get; set; }
    }
}
