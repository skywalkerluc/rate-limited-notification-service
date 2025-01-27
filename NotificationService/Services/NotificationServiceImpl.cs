using NotificationService.Gateways;
using NotificationService.Models;

namespace NotificationService.Services
{
    public class NotificationServiceImpl : INotificationService
    {
        private readonly IGateway _gateway;
        private readonly Dictionary<string, RateLimitRule> _rateLimitRules;
        private readonly Dictionary<string, Dictionary<string, int>> _notificationCounts;
        public Dictionary<string, RateLimitRule> GetRateLimitRules()
        {
            return _rateLimitRules;
        }

        public NotificationServiceImpl(IGateway gateway)
        {
            _gateway = gateway;
            _rateLimitRules = new Dictionary<string, RateLimitRule>
            {
                { "status", new RateLimitRule { Limit = 2, TimeWindow = TimeSpan.FromMinutes(1) } },
                { "news", new RateLimitRule { Limit = 1, TimeWindow = TimeSpan.FromDays(1) } },
                { "marketing", new RateLimitRule { Limit = 3, TimeWindow = TimeSpan.FromHours(1) } }
            };
            _notificationCounts = new Dictionary<string, Dictionary<string, int>>();
        }

        public void Send(string type, string userId, string message)
        {
            if (!_rateLimitRules.ContainsKey(type))
            {
                throw new ArgumentException($"Tipo de notificação inválido: {type}");
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
            }
            else
            {
                Console.WriteLine($"Limite de taxa excedido para o tipo de notificação: {type}");
            }
        }
    }
}