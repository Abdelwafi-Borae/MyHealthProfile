namespace MyHealthProfile.Models.Dtos
{
    public class PatientDto
    {
        public string? Name { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Nationality { get; set; }
        public string? Address { get; set; }
        public string? PhotoUrl { get; set; }
        public bool IsEmailVerified { get; set; } = false;

    }
}
