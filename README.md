# RateLimitedNotificationService

This project implements a rate-limited notification service in C# with .NET Core. It ensures that recipients don't receive too many email notifications, preventing spam and protecting against abuse.

## Features

* Limits the number of emails sent to each recipient based on configurable rules.
* Supports different notification types with different rate limits (e.g., news, status updates, marketing).
* Uses a gateway to send the notifications (can be easily adapted for different gateways).
* Includes unit tests to ensure code quality.

## How to run

1. Clone the repository: `git clone git@github.com:skywalkerluc/rate-limited-notification-service.git`
2. Navigate to the project folder: `cd RateLimitedNotificationService`
3. Build the project: `dotnet build`
4. Run the tests: `dotnet test`

## Project structure

* **NotificationService:** Contains the implementation of the notification service.
    * **Models:** Defines the model classes, such as `RateLimitRule`.
    * **Services:** Contains the `INotificationService` interface and the `NotificationServiceImpl` implementation.
    * **Gateways:** Defines the `IGateway` interface and the `Gateway` implementation.
* **NotificationService.Tests:** Contains the unit tests for the notification service.

## Contributing

Contributions are welcome! Feel free to open issues or pull requests.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.