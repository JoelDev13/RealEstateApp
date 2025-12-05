using RealEstateApp.Application.Dtos.Email;

namespace RealEstateApp.Application.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendAsync(EmailRequestDto emailRequestDto);
    }
}

