namespace MyHealthProfile.Models
{
    public class Allergy
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<UserAllergy> UserAllergies { get; set; }
    }
}
