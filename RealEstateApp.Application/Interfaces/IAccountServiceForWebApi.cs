using RealEstateApp.Application.Dtos.Auth;

namespace RealEstateApp.Application.Interfaces
{
    public interface IAccountServiceForWebApi : IBaseAccountService
    {
        Task<Result<string>> AuthenticateAsync(LoginDto loginDto);
    }
}
