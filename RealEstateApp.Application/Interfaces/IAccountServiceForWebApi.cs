using RealEstateApp.Application.Dtos.Auth;
using RealEstateApp.Application.Interfaces.Services;

namespace RealEstateApp.Application.Interfaces
{
    public interface IAccountServiceForWebApi : IBaseAccountService
    {
        Task<Result<string>> AuthenticateAsync(LoginDto loginDto);
    }
}
