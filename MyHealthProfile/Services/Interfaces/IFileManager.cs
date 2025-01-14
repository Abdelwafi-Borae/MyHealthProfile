namespace MyHealthProfile.Services.Interfaces
{
    public interface IFileManager
    {
       
        public string CreateUpdateFile(IFormFile file, string OldFile);
    }
}
