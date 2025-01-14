using Data.ModelViews;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using MyHealthProfile.Extensions;
using MyHealthProfile.Models;
using MyHealthProfile.Models.Dtos;
using MyHealthProfile.Repositories.Account;

namespace MyHealthProfile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IIdentityService _identityService;

        public AccountController(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        [HttpPost("Register")]
        [ProducesResponseType(typeof(GenericResult<RegisterResponseDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(GenericResult<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(GenericResult<object>), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(GenericResult<object>), StatusCodes.Status500InternalServerError)]
        public async Task<GenericResult<RegisterResponseDto>> RegisterAsync([FromForm] RegisterDto request, IFormFile? file)
        {
            var results = await _identityService.RegisterAsync(request,file);
            Response.StatusCode = StatusCodes.Status201Created;
            return results.ToCreatedResult();
        }
        [HttpPut("ConfirmEmail/{userId}/{code}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(GenericResult<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(GenericResult<object>), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(GenericResult<object>), StatusCodes.Status500InternalServerError)]
        public async Task<GenericResult<string>> ConfirmEmailAsync(string userId, string code)
        {
            var results = await _identityService.AccountVerivicationAsync(userId, code);
            Response.StatusCode = StatusCodes.Status201Created;
            // return NoContent();
            return results.ToCreatedResult();
        }


        [HttpPost("Token")]
        [ProducesResponseType(typeof(GenericResult<TokenResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(GenericResult<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(GenericResult<object>), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(GenericResult<object>), StatusCodes.Status500InternalServerError)]
        public async Task<GenericResult<TokenResponse>> LogInAsync(LoginDto request)
        {
            var result = await _identityService.LoginAsync(request);
            Response.StatusCode = StatusCodes.Status201Created;
            return result.ToCreatedResult();
        }

        [Authorize]
        [HttpGet]
        public IActionResult Test()
        {
            return Ok("true");
        }


        [HttpPut("ForgetPassword")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GenericResult<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(GenericResult<object>), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(GenericResult<object>), StatusCodes.Status500InternalServerError)]
        public async Task<GenericResult<string>> ForgetPasswordAsync(string Email)
        {
            var result = await _identityService.ForgetPasswordAsync(Email);
            return result.ToSuccessResult();

        }
        [HttpPut("SetPassword")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(GenericResult<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(GenericResult<object>), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(GenericResult<object>), StatusCodes.Status500InternalServerError)]
        public async Task<GenericResult<string>> SetPasswordAsync(ResetPasswordRequestDto request)
        {
            var result = await _identityService.setNewPasswordAsync(request);
            return result.ToSuccessResult();
        }
        [Authorize]
        [HttpGet("GetProfile")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(GenericResult<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(GenericResult<object>), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(GenericResult<object>), StatusCodes.Status500InternalServerError)]
        public async Task<GenericResult<PatientDto>> GetProfile()
        {
            var result = await _identityService.GetUserProfile();
            return result.ToSuccessResult();
        }
        [Authorize]
        [HttpPut("UpdateProfile")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(GenericResult<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(GenericResult<object>), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(GenericResult<object>), StatusCodes.Status500InternalServerError)]
        public async Task<GenericResult<PatientDto>> UpdateProfile([FromForm] PatientUpdateDTO request, IFormFile? file)
        {
            var result = await _identityService.UpdateUserProfile(request,file);
            return result.ToSuccessResult();
        }
    }
}
