using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;

namespace FitnessPro.Services
{
    public interface IFitnessEmailSender : IEmailSender
    {
        Task sendContactEmailAsync(string emailFrom, string name, string subject,string htmlMessage);
    }
}