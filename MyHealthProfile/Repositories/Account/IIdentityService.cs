
using Data.ModelViews;
using MyHealthProfile.Models;
using MyHealthProfile.Models.Dtos;

namespace MyHealthProfile.Repositories.Account
{
    public interface IIdentityService
    {

        public Task<RegisterResponseDto> RegisterAsync(RegisterDto register, IFormFile file);
        public Task<string> AccountVerivicationAsync(string userId, string otp);
        public Task<TokenResponse> LoginAsync(LoginDto model);
        public Task<PatientDto> GetUserProfile();
        public Task<PatientDto> UpdateUserProfile(PatientUpdateDTO request, IFormFile? file);
        public Task<string> ForgetPasswordAsync(string Email);
        public Task<string> setNewPasswordAsync(ResetPasswordRequestDto request);





    }
}
