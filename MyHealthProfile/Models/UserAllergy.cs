namespace MyHealthProfile.Models
{
    public class UserAllergy
    {
        public int Id { get; set; } 
        public string UserId { get; set; }  
        public int AllergyId { get; set; } 
        public string Remarks { get; set; }  

        // Navigation Properties
        public Patient User { get; set; }  
        public Allergy Allergy { get; set; }   
    }
}
