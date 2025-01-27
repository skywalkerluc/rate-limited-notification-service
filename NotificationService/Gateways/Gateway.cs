namespace NotificationService.Gateways
{
    public class Gateway : IGateway
    {
        public void Send(string userId, string message)
        {
            Console.WriteLine($"Enviando mensagem para o usuário {userId}: {message}");
        }
    }
}