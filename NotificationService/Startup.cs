using Microsoft.Extensions.DependencyInjection;
using NotificationService.Gateways;
using NotificationService.Services;

namespace NotificationService
{
    public class Startup
    {
        public static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();
            services.AddTransient<IGateway, Gateway>();
            services.AddTransient<INotificationService, NotificationServiceImpl>();

            return services.BuildServiceProvider();
        }
    }
}