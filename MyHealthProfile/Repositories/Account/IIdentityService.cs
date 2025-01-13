
using Data.ModelViews;
using MyHealthProfile.Models;
using MyHealthProfile.Models.Dtos;

namespace MyHealthProfile.Repositories.Account
{
    public interface IIdentityService
    {

        public Task<RegisterResponseDto> RegisterAsync(RegisterDto register);
        public Task<string> AccountVerivicationAsync(string userId, string otp);
        public Task<TokenResponse> LoginAsync(LoginDto model);
        public Task<TokenResponse> GetUserProfile(LoginDto register);
        public Task<TokenResponse> UpdateUserProfile(LoginDto register);
        public Task<bool> ForgetPasswordAsync(string Email);
        public Task<bool> setNewPasswordAsync(ResetPasswordRequestDto request);





    }
}
