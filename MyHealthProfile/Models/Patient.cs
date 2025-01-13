using Microsoft.AspNetCore.Identity;

namespace MyHealthProfile.Models
{
    public class Patient : IdentityUser
    {
        public string Name { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string Nationality { get; set; }
        public string? Address { get; set; }
        public string? PhotoUrl { get; set; }
        public ICollection<UserAllergy>? UserAllergies { get; set; }
        //for confirmation
        public string? EmailOtp { get; set; }
        public DateTime? EmailOtpExpiration { get; set; }
        public bool IsEmailVerified { get; set; } = false;
        //for confirmation password reset
        public string? PasswordOtp { get; set; }
        public DateTime? PasswordOtpExpiration { get; set; }
        public bool IsPaswordSet { get; set; } = true;
    }

}
