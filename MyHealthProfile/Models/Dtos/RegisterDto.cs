using System.ComponentModel.DataAnnotations;

namespace MyHealthProfile.Models.Dtos
{
    public class RegisterDto
    {
        [Required]
       
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        
         
        public string Name { get; set; } = string.Empty;

        [Required]
        public string UserName { get; set; } = string.Empty;
        [Required]
        public string Nationality { get; set; }
        [Required]
        public string Email {  get; set; } = string.Empty ;

        [Required]
        public string Password { get; set; } = string.Empty;

        
        public string PhoneNumber { get; set; } = string.Empty;

     
        public string Address {  get; set; } = string.Empty;

         
    }

    
}
