using Microsoft.AspNetCore.Identity;
using FluentValidation.Results;
using MyHealthProfile.Common.Exceptions;
//using System.ComponentModel.DataAnnotations;

namespace MyHealthProfile.Common.Extensions
{
    public static class IdentityResultExtensions
    {
        public static ValidationException ToValidationException(this IdentityResult result)
        {
            if (!result.Succeeded && result.Errors.Any())
            {
                var errors = result.Errors.Select(e => new { e.Code, e.Description });
                var failuers = new List<ValidationFailure>();
                foreach (var e in errors) failuers.Add(new ValidationFailure(e.Code, e.Description));
                return new ValidationException(failuers);
            }
            return null;
        }
        public static ValidationException ToValidationException(this string message, string code)
            => new ValidationException(new List<ValidationFailure> { new ValidationFailure(code, message) });
    }
}

