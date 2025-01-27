namespace NotificationService.Gateways
{
    public class Gateway : IGateway
    {
        public void Send(string userId, string message)
        {
            Console.WriteLine($"Sending a message to the user {userId}: {message}");
        }
    }
}