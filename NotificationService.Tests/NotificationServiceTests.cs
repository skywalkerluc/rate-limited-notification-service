
using Moq;
using NotificationService.Gateways;
using NotificationService.Services;

namespace NotificationService.Tests
{
    public class NotificationServiceTests
    {
        [Fact]
        public void ShouldSendNotificationWhenBelowLimit()
        {
            var gatewayMock = new Mock<IGateway>();
            var service = new NotificationServiceImpl(gatewayMock.Object);

            service.Send("news", "user", "news 1");

            gatewayMock.Verify(g => g.Send("user", "news 1"), Times.Once);
        }

        [Fact]
        public void ShouldNotSendNotificationWhenAboveLimit()
        {
            var gatewayMock = new Mock<IGateway>();
            var service = new NotificationServiceImpl(gatewayMock.Object);

            service.Send("news", "user", "news 1");
            service.Send("news", "user", "news 2");

            gatewayMock.Verify(g => g.Send("user", "news 2"), Times.Never);
        }

        [Fact]
        public void ShouldResetCountAfterTimeWindow()
        {
            var gatewayMock = new Mock<IGateway>();
            var service = new NotificationServiceImpl(gatewayMock.Object);

            service.Send("news", "user", "news 1");

            var rule = service.GetRateLimitRules()["news"]; 
            Thread.Sleep((int)rule.TimeWindow.TotalMilliseconds + 1000); 

            service.Send("news", "user", "news 2");

            gatewayMock.Verify(g => g.Send("user", "news 2"), Times.Once);
        }
    }
}