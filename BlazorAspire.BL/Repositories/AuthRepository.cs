using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using BlazorAspire.Database.Data;
using BlazorAspire.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlazorAspire.BL.Repositories;


public interface IAuthRepository
{
    Task AddRefreshTokenModelAsync(RefreshTokenModel refreshTokenModel);
    Task<RefreshTokenModel> GetRefreshTokenModelAsync(string refreshToken);
    Task<UserModel> GetUserByLoginAsync(string userName, string password);
    Task RemoveRefreshTokenByUserIdAsync(int userId);
}

public class AuthRepository(AppDbContext appDbContext) : IAuthRepository
{
    public async Task AddRefreshTokenModelAsync(RefreshTokenModel refreshTokenModel)
    {
        await appDbContext.RefreshTokens.AddAsync(refreshTokenModel);
        await appDbContext.SaveChangesAsync();
    }

    public Task<RefreshTokenModel> GetRefreshTokenModelAsync(string refreshToken)
    {
        return appDbContext.RefreshTokens
            .Include(u => u.User) //navigation property
            .ThenInclude(r => r.UserRoles) //navigation property
            .ThenInclude(ur => ur.Role) //navigation property
            .FirstOrDefaultAsync(rt => rt.RefreshToken == refreshToken);
    }

    public Task<UserModel> GetUserByLoginAsync(string userName, string password)
    {
        return appDbContext.Users
            .Include(r => r.UserRoles)  //navigation property
            .ThenInclude(ur => ur.Role) //navigation property
            .FirstOrDefaultAsync(u => u.UserName == userName && u.Password == password);
    }

    public async Task RemoveRefreshTokenByUserIdAsync(int userId)
    {
        var refreshToken = await  appDbContext.RefreshTokens.FirstOrDefaultAsync(rt => rt.UserId == userId);
        if (refreshToken != null)
        {
            appDbContext.RemoveRange(refreshToken);
            await appDbContext.SaveChangesAsync();
        }
    }
}
