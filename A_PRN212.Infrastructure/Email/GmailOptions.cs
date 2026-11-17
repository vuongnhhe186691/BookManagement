namespace A_PRN212.Infrastructure.Email
{
    public class GmailOptions
    {
        public string Email { get; set; } = string.Empty;
        public string Host { get; set; } = "smtp.gmail.com";
        public string Hosts { get; set; } = string.Empty;
        public int Port { get; set; } = 587;
        public string Password { get; set; } = string.Empty;

        public string EffectiveHost => !string.IsNullOrWhiteSpace(Host) ? Host :
            (!string.IsNullOrWhiteSpace(Hosts) ? Hosts : "smtp.gmail.com");
    }
}
