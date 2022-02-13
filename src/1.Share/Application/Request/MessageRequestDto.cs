namespace Application.Request
{
    public interface INotifyMessageRequest
    {
        
    }

    public class CommonNotifyMessageReqeust
    {
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
    }
}