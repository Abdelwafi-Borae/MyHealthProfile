using Data.ModelViews;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using MyHealthProfile.Extensions;
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
        public async Task<GenericResult<RegisterResponseDto>> RegisterAsync(RegisterDto request)
        {
            var results = await _identityService.RegisterAsync(request);
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
        public IActionResult Test(   )
        {
            return Ok("true");
        }


        // [HttpPut("ForgetPassword")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(GenericResult<object>), StatusCodes.Status404NotFound)]
        //[ProducesResponseType(typeof(GenericResult<object>), StatusCodes.Status422UnprocessableEntity)]
        //[ProducesResponseType(typeof(GenericResult<object>), StatusCodes.Status500InternalServerError)]
        //public async Task<GenericResult<ForgotPasswordResponse>> ForgetPasswordAsync(ForgotPasswordRequest request)
        //    {
        //        return (await _identityService.ForgetPasswordAsync(request)).ToSuccessResult();
        //    }
        //    [HttpPut("ResetPassword")]
        //    [ProducesResponseType(StatusCodes.Status204NoContent)]
        //    [ProducesResponseType(typeof(GenericResult<object>), StatusCodes.Status404NotFound)]
        //    [ProducesResponseType(typeof(GenericResult<object>), StatusCodes.Status422UnprocessableEntity)]
        //    [ProducesResponseType(typeof(GenericResult<object>), StatusCodes.Status500InternalServerError)]
        //    public async Task<IActionResult> ResetPasswordAsync(ResetPasswordRequest request)
        //    {
        //        await _identityService.ResetPasswordAsync(request);
        //        return NoContent();
        //    }
    }
}
