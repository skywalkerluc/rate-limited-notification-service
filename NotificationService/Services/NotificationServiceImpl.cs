using NotificationService.Exceptions;
using NotificationService.Gateways;
using NotificationService.Models;
using Serilog;

namespace NotificationService.Services
{
    public class NotificationServiceImpl : INotificationService
    {
        private readonly IGateway _gateway;
        private Dictionary<string, RateLimitRule> _rateLimitRules;
        private readonly Dictionary<string, Dictionary<string, int>> _notificationCounts;
        public Dictionary<string, RateLimitRule> GetRateLimitRules()
        {
            return _rateLimitRules;
        }

        public void SetRateLimitRulesForTest(Dictionary<string, RateLimitRule> rules)
        {
            _rateLimitRules = rules;
        }

        public NotificationServiceImpl(IGateway gateway)
        {
            _gateway = gateway;
            _rateLimitRules = new Dictionary<string, RateLimitRule>
            {
                { "status", new RateLimitRule { Limit = 2, TimeWindow = TimeSpan.FromMinutes(1) } },
                { "news", new RateLimitRule { Limit = 1, TimeWindow = TimeSpan.FromHours(1) } },
                { "marketing", new RateLimitRule { Limit = 3, TimeWindow = TimeSpan.FromDays(1) } }
            };
            _notificationCounts = new Dictionary<string, Dictionary<string, int>>();
        }

        public void Send(string type, string userId, string message)
        {
            Log.Information("Sending {Type} notifications to the user {UserId}", type, userId);
            if (!_rateLimitRules.ContainsKey(type))
            {
                Log.Error($"Invalid notification type: {type}");
                throw new InvalidNotificationTypeException($"Invalid notification type: {type}");
            }

            var rule = _rateLimitRules[type];
            var key = $"{userId}:{type}";

            if (!_notificationCounts.ContainsKey(key))
            {
                _notificationCounts[key] = new Dictionary<string, int>
                {
                    { "count", 0 },
                    { "lastSent", 0 }
                };
            }

            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var lastSent = _notificationCounts[key]["lastSent"];
            var count = _notificationCounts[key]["count"];

            if (now - lastSent > rule.TimeWindow.TotalSeconds)
            {
                count = 0;
            }

            if (count < rule.Limit)
            {
                _gateway.Send(userId, message);
                _notificationCounts[key]["count"] = count + 1;
                _notificationCounts[key]["lastSent"] = (int)now;

                Log.Information("Notification sent successfully!");
            }
            else
            {
                Log.Error($"Rate limit exceeded for this type of notification: {type}");
                throw new RateLimitExceededException($"Rate limit exceeded for this type of notification: {type}");
            }
        }
    }
}