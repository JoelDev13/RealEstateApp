using RealEstateApp.Application.Dtos.Email;

namespace RealEstateApp.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendAsync(EmailRequestDto emailRequestDto);
    }
}

