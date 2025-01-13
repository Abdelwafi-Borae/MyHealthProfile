namespace MyHealthProfile.Services.Interfaces
{
    public interface IFileManager
    {
        public string CreateFile(IFormFile file);
        public string UpdateFile(IFormFile file, string OldFile);
    }
}
