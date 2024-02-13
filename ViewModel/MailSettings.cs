using Microsoft.AspNetCore.Identity;

namespace FitnessPro.ViewModel
{
    public class MailSettings
    {
        //configure and use smtp server from google 

        public string? Email { get; set; }
        public string? DisplayName { get; set; }
        public string? Password { get; set; }
        public string? Host {get;set;}
        public int Port { get; set; }
    }
}
