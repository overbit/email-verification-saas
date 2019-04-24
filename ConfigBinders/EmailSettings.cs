namespace EmailVerificationService.ConfigBinders
{
    public class EmailSettings
    {
        public string SmtpClient { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FromAccount { get; set; }
    }
}