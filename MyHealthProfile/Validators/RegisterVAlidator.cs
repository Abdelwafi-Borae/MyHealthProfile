using Data.ModelViews;
using FluentValidation;
using Microsoft.AspNetCore.Identity.Data;
using MyHealthProfile.Models.Dtos;

namespace Validators
{
    public class RegisterVAlidato : AbstractValidator<RegisterDto>
    {
        public RegisterVAlidato()
        {
            RuleFor(p => p.Email)
                .NotEmpty()
                .EmailAddress()
                    .WithMessage("Invalid Email Address.");

            RuleFor(p => p.Password)
                .NotEmpty().MinimumLength(6).MaximumLength(12);

        }
    }
    public class LoginVAlidator : AbstractValidator<LoginDto>
    {
        public LoginVAlidator()
        {
            RuleFor(p => p.Email)
                .NotEmpty()
                .EmailAddress()
                    .WithMessage("Invalid Email Address.");

            RuleFor(p => p.Password)
                .NotEmpty().MinimumLength(6).MaximumLength(12);

        }
    }
    public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequestDto>
    {
        public ResetPasswordRequestValidator()
        { 
            RuleFor(p => p.Email).NotEmpty().EmailAddress();
            RuleFor(p => p.Password).NotEmpty().MinimumLength(8);
            RuleFor(p => p.ConfirmPassword).NotEmpty().MinimumLength(8).Equal(x => x.Password);
            RuleFor(p => p.OPT).NotEmpty();
        }

    }
}


