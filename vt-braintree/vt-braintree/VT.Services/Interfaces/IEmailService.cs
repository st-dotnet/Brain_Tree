using VT.Services.DTOs;

namespace VT.Services.Interfaces
{
    public interface IEmailService
    {
        SendEmailResponse SendEmail(SendEmailRequest request);
        SendEmailResponse SendEmail(SendCustomerEmailRequest request);
        SendEmailResponse SendEmail(SendMultipleServicesEmailRequest request);
    }
}
