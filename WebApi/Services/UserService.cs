using Microsoft.EntityFrameworkCore;
using TasksApi.Data;
using TasksApi.Helpers;
using TasksApi.Models;
using TasksApi.Models.Entities;
using TasksApi.Models.Requests;
using TasksApi.Models.Response;

namespace TasksApi.Services
{
    public class UserService : IUserService
    {
        private readonly TasksDbContext tasksDbContext;
        private readonly ITokenService tokenService;

        public UserService(TasksDbContext tasksDbContext, ITokenService tokenService)
        {
            this.tasksDbContext = tasksDbContext;
            this.tokenService = tokenService;
        }

        public async Task<TokenResponse> LoginAsync(LoginRequest loginRequest)
        {
            User? user = tasksDbContext.Users.SingleOrDefault(user => user.Active 
                                        && user.Email == loginRequest.Email);
 
            if (user is null)
            {
                return new TokenResponse
                {
                    Success = false,
                    Error = "Email not found",
                    ErrorCode = "L02"
                };
            }

            string passwordHash = PasswordHelper.HashUsingPbkdf2(loginRequest.Password, 
                Convert.FromBase64String(user.PasswordSalt));
 
            if (user.Password != passwordHash)
            {
                return new TokenResponse
                {
                    Success = false,
                    Error = "Invalid Password",
                    ErrorCode = "L03"
                };
            }
 
            Tuple<string, string> token = await System.Threading.Tasks.Task.Run(() => 
                tokenService.GenerateTokensAsync(user.Id));
 
            return new TokenResponse
            {
                Success = true,
                AccessToken = token.Item1,
                RefreshToken = token.Item2
            };
        }

        public async Task<LogoutResponse> LogoutAsync(int userId)
        {
            RefreshToken? refreshToken = await tasksDbContext.RefreshTokens
                                .FirstOrDefaultAsync(o => o.UserId == userId);
 
            if (refreshToken is null)
            {
                return new LogoutResponse { Success = true };
            }
 
            tasksDbContext.RefreshTokens.Remove(refreshToken);
 
            int saveResponse = await tasksDbContext.SaveChangesAsync();
 
            if (saveResponse >= 0)
            {
                return new LogoutResponse { Success = true };
            }
 
            return new LogoutResponse { Success = false, Error = "Unable to logout user", ErrorCode = "L04" };
        }

        public async Task<SignupResponse> SignupAsync(SignupRequest signupRequest)
        {
            User? existingUser = await tasksDbContext.Users.SingleOrDefaultAsync(user => user.Email == signupRequest.Email);
 
            if (existingUser is not null)
            {
                return new SignupResponse
                {
                    Success = false,
                    Error = "User already exists with the same email",
                    ErrorCode = "S02"
                };
            }
 
            if (signupRequest.Password != signupRequest.ConfirmPassword) {
                return new SignupResponse
                {
                    Success = false,
                    Error = "Password and confirm password do not match",
                    ErrorCode = "S03"
                };
            }
 
            if (signupRequest.Password.Length <= 7) // This can be more complicated than only length, you can check on alphanumeric and or special characters
            {
                return new SignupResponse
                {
                    Success = false,
                    Error = "Password is weak",
                    ErrorCode = "S04"
                };
            }
 
            byte[] salt = PasswordHelper.GetSecureSalt();
            string passwordHash = PasswordHelper.HashUsingPbkdf2(signupRequest.Password, salt);
 
            var user = new User
            {
                Email = signupRequest.Email,
                Password = passwordHash,
                PasswordSalt = Convert.ToBase64String(salt),
                FirstName = signupRequest.FirstName,
                LastName = signupRequest.LastName,
                Ts = signupRequest.Ts,
                Active = true
            };
 
            await tasksDbContext.Users.AddAsync(user);
 
            int saveResponse = await tasksDbContext.SaveChangesAsync();
 
            if (saveResponse >= 0)
            {
                return new SignupResponse { Success = true, Email = user.Email };
            }
 
            return new SignupResponse
            {
                Success = false,
                Error = "Unable to save the user",
                ErrorCode = "S05"
            };
        }
    }
}
