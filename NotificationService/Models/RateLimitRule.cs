namespace NotificationService.Models
{
    public class RateLimitRule
    {
        public int Limit { get; set; }
        public TimeSpan TimeWindow { get; set; }
    }
}