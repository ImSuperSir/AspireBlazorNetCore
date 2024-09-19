using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using BlazorAspire.BL.Repositories;
using BlazorAspire.Model.Entities;

namespace BlazorAspire.BL.Services;



public interface IAuthService
{
    Task<UserModel> GetUserByLoginAsync(string userName, string password);
    Task AddRefreshTokenModelAsync(RefreshTokenModel refreshTokenModel);
    Task<RefreshTokenModel> GetRefreshTokenModelAsync(string refreshToken);
}
public class AuthService(IAuthRepository authRepository) : IAuthService
{
    public async Task AddRefreshTokenModelAsync(RefreshTokenModel refreshTokenModel)
    {
        await authRepository.RemoveRefreshTokenByUserIdAsync(refreshTokenModel.UserId);
        await authRepository.AddRefreshTokenModelAsync(refreshTokenModel);
    }

    public async Task<UserModel> GetUserByLoginAsync(string userName, string password)
    {
        return await authRepository.GetUserByLoginAsync(userName, password);
    }

    public async Task<RefreshTokenModel> GetRefreshTokenModelAsync(string refreshToken)
    {
        return await authRepository.GetRefreshTokenModelAsync(refreshToken);
    }
}
