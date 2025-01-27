namespace NotificationService.Gateways
{
    public interface IGateway
    {
        void Send(string userId, string message);
    }
}