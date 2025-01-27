using Microsoft.Extensions.DependencyInjection;
using Moq;
using NotificationService.Exceptions;
using NotificationService.Gateways;
using NotificationService.Models;
using NotificationService.Services;

namespace NotificationService.Tests
{
    public class NotificationServiceTests
    {
        private readonly IServiceProvider _serviceProvider;

        public NotificationServiceTests()
        {
            _serviceProvider = Startup.ConfigureServices();
        }

        [Fact]
        public void ShouldSendNotificationWhenBelowLimit()
        {
            var gatewayMock = new Mock<IGateway>();

            var services = new ServiceCollection();
            services.AddTransient<IGateway>(sp => gatewayMock.Object);
            services.AddTransient<INotificationService, NotificationServiceImpl>();
            var serviceProvider = services.BuildServiceProvider();

            var service = serviceProvider.GetRequiredService<INotificationService>();

            service.Send("news", "user", "news 1");

            gatewayMock.Verify(g => g.Send("user", "news 1"), Times.Once);
        }

        [Fact]
        public void ShouldNotSendNotificationWhenAboveLimit()
        {
            var gatewayMock = new Mock<IGateway>();

            var services = new ServiceCollection();
            services.AddTransient<IGateway>(sp => gatewayMock.Object);
            services.AddTransient<INotificationService, NotificationServiceImpl>();
            var serviceProvider = services.BuildServiceProvider();

            var service = serviceProvider.GetRequiredService<INotificationService>();

            service.Send("news", "user", "news 1");
            Assert.Throws<RateLimitExceededException>(() => service.Send("news", "user", "news 2")); 
        }

        [Fact]
        public void ShouldResetCountAfterTimeWindow()
        {

            var gatewayMock = new Mock<IGateway>();

            var services = new ServiceCollection();
            services.AddTransient<IGateway>(sp => gatewayMock.Object);
            services.AddTransient<INotificationService, NotificationServiceImpl>();
            var serviceProvider = services.BuildServiceProvider();

            var service = serviceProvider.GetRequiredService<INotificationService>();

            var testRules = new Dictionary<string, RateLimitRule>
            {
                { "news", new RateLimitRule { Limit = 1, TimeWindow = TimeSpan.FromMilliseconds(10) } }
            };
            var serviceImpl = (NotificationServiceImpl)service;
            serviceImpl.SetRateLimitRulesForTest(testRules);

            service.Send("news", "user", "news 1");

            var rule = serviceImpl.GetRateLimitRules()["news"];

            Thread.Sleep((int)rule.TimeWindow.TotalMilliseconds + 1000);

            service.Send("news", "user", "news 2");

            gatewayMock.Verify(g => g.Send("user", "news 2"), Times.Once);
        }

        [Fact]
        public void ShouldThrowRateLimitExceededExceptionForStatus()
        {
            var gatewayMock = new Mock<IGateway>();

            var services = new ServiceCollection();
            services.AddTransient<IGateway>(sp => gatewayMock.Object);
            services.AddTransient<INotificationService, NotificationServiceImpl>();
            var serviceProvider = services.BuildServiceProvider();

            var service = serviceProvider.GetRequiredService<INotificationService>();

            var testRules = new Dictionary<string, RateLimitRule>
            {
                { "status", new RateLimitRule { Limit = 2, TimeWindow = TimeSpan.FromMilliseconds(10) } }
            };
            var serviceImpl = (NotificationServiceImpl)service;
            serviceImpl.SetRateLimitRulesForTest(testRules);

            service.Send("status", "user", "status 1");
            service.Send("status", "user", "status 2");

            Assert.Throws<RateLimitExceededException>(() => service.Send("status", "user", "status 3"));
        }

        [Fact]
        public void ShouldHandleDifferentUsers()
        {
            var gatewayMock = new Mock<IGateway>();

            var services = new ServiceCollection();
            services.AddTransient<IGateway>(sp => gatewayMock.Object);
            services.AddTransient<INotificationService, NotificationServiceImpl>();
            var serviceProvider = services.BuildServiceProvider();

            var service = serviceProvider.GetRequiredService<INotificationService>();

            service.Send("news", "user1", "news 1");
            service.Send("news", "user2", "news 1");

            gatewayMock.Verify(g => g.Send("user1", "news 1"), Times.Once);
            gatewayMock.Verify(g => g.Send("user2", "news 1"), Times.Once);
        }
    }
}