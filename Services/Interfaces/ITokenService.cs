using Serai.AuthApi.Entities;

namespace Serai.AuthApi.Services.Interfaces;

public interface ITokenService
{
    string CreateAccessToken(AppUser user);
}
