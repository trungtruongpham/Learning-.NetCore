namespace EmailHandlerApi
{
    public class EmailConfig
    {
        public string FromName { get; set; }
        public string FromAddress { get; set; }
        public string MailServerAddress { get; set; }
        public string MailServerPort { get; set; }
        public string UserID { get; set; }
        public string UserPassword { get; set; }
    }
}