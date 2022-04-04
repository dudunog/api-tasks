using Microsoft.EntityFrameworkCore;
using TasksApi.Data;
using TasksApi.Helpers;
using TasksApi.Models;
using TasksApi.Models.Entities;
using TasksApi.Models.Requests;
using TasksApi.Models.Response;

namespace TasksApi.Services
{
    public class TokenService : ITokenService
    {
        private readonly TasksDbContext tasksDbContext;

        public TokenService(TasksDbContext tasksDbContext)
        {
            this.tasksDbContext = tasksDbContext;
        }

        public async Task<Tuple<string, string>> GenerateTokensAsync(int userId)
        {
            string accessToken = await TokenHelper.GenerateAccessToken(userId);
            string refreshToken = await TokenHelper.GenerateRefreshToken();

            User? userRecord = await tasksDbContext.Users
                                    .Include(u => u.RefreshTokens)
                                    .FirstOrDefaultAsync(u => u.Id == userId);

            if (userRecord is null)
                return null;

            byte[] salt = PasswordHelper.GetSecureSalt();

            string refreshTokenHashed = PasswordHelper.HashUsingPbkdf2(refreshToken, salt);

            if (userRecord.RefreshTokens is not null && userRecord.RefreshTokens.Any())
                await RemoveRefreshTokenAsync(userRecord);
            
            userRecord.RefreshTokens?.Add(new RefreshToken
            {
                ExpiryDate = DateTime.UtcNow.AddDays(30),
                Ts = DateTime.UtcNow,
                UserId = userId,
                TokenHash = refreshTokenHashed,
                TokenSalt = Convert.ToBase64String(salt)
            });

            await tasksDbContext.SaveChangesAsync();

            return new Tuple<string, string>(accessToken, refreshToken);
        }

        public async Task<bool> RemoveRefreshTokenAsync(User user)
        {
            User? userRecord = await tasksDbContext.Users
                                    .Include(u => u.RefreshTokens)
                                    .FirstOrDefaultAsync(u => u.Id == user.Id);

            if (userRecord is null)
                return false;
            
            if (userRecord.RefreshTokens != null && userRecord.RefreshTokens.Any())
            {
                RefreshToken currentRefreshToken = userRecord.RefreshTokens.First();

                tasksDbContext.RefreshTokens.Remove(currentRefreshToken);
            }

            return false;
        }

        public async Task<ValidateRefreshTokenResponse> ValidateRefreshTokenAsync(RefreshTokenRequest refreshTokenRequest)
        {
            RefreshToken? refreshToken = await tasksDbContext.RefreshTokens
                                    .FirstOrDefaultAsync(o => o.UserId == refreshTokenRequest.UserId);
 
            ValidateRefreshTokenResponse response = new ValidateRefreshTokenResponse();

            if (refreshToken == null)
            {
                response.Success = false;
                response.Error = "Invalid session or user is already logged out";
                response.ErrorCode = "R02";

                return response;
            }
 
            string refreshTokenToValidateHash = PasswordHelper.HashUsingPbkdf2(refreshTokenRequest.RefreshToken, 
                Convert.FromBase64String(refreshToken.TokenSalt));
 
            if (refreshToken.TokenHash != refreshTokenToValidateHash)
            {
                response.Success = false;
                response.Error = "Invalid refresh token";
                response.ErrorCode = "R03";
                return response;
            }
          
            if (refreshToken.ExpiryDate < DateTime.Now)
            {
                response.Success = false;
                response.Error = "Refresh token has expired";
                response.ErrorCode = "R04";
                return response;
            }
 
            response.Success = true;
            response.UserId = refreshToken.UserId;
 
            return response;
        }
    }
}
