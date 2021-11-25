namespace Domain.Configuration
{
    public class EMailSettings
    {
        public string FromMail { get; set; }
        public string DisplayName { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
    }
}