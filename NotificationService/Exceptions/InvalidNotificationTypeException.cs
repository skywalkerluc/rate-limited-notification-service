namespace NotificationService.Exceptions;

public class InvalidNotificationTypeException : Exception
{
    public InvalidNotificationTypeException(string message) : base(message) { }
}
