using MyHealthProfile.Services.Interfaces;

namespace MyHealthProfile.Services
{
    public class FileManager : IFileManager
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _config;
        public FileManager(IWebHostEnvironment environment, IConfiguration config)
        {
            _environment = environment;
            _config = config;
        }



        public string CreateUpdateFile(IFormFile file, string? OldFile)
        {

            string wwwrootPath = _environment.WebRootPath ?? throw new InvalidOperationException("WebRootPath is not set.");
            if (file != null)
            {
                var uploads = Path.Combine(wwwrootPath, "PatientPhoto");
                var extension = Path.GetExtension(file.FileName);

                // Validate the file type
                if (!IsValidFileType(extension))
                {
                    throw new NotSupportedException("File type is not supported.");
                }

                // Sanitize the file name
                string sanitizedFileName = SanitizeFileName(Guid.NewGuid().ToString());
                if (!Directory.Exists(uploads))
                {
                    Directory.CreateDirectory(uploads);
                }
                if (!string.IsNullOrEmpty(OldFile))
                {
                    try
                    {
                        // Extract the relative path from the URL
                        var uri = new Uri(OldFile); // Converts URL to Uri
                        var relativePath = uri.LocalPath.TrimStart('/'); // Get the path after the domain (e.g., PatientPhoto/...jpg)

                        // Combine with WebRootPath to get the physical file path
                        var oldFilePath = Path.Combine(wwwrootPath, relativePath.Replace("/", Path.DirectorySeparatorChar.ToString()));

                        // Check if the file exists in the file system
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            // Delete the file
                            System.IO.File.Delete(oldFilePath);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log the error (optional)
                        Console.WriteLine($"Error deleting file: {ex.Message}");
                        throw new Exception("Failed to delete the old file.", ex);
                    }
                }

                try
                {
                    // Combine sanitized filename with extension for the full file path
                    using (var filestream = new FileStream(Path.Combine(uploads, sanitizedFileName + extension), FileMode.Create))
                    {
                        file.CopyTo(filestream);
                    }
                }
                catch (Exception ex)
                {
                    // Handle exceptions (e.g., log them)
                    throw new Exception("File upload failed", ex);
                }
                string imageUrl = @"/PatientPhoto/" + sanitizedFileName + extension;
                string fullLogoUrl = _config["BaseUrl"].ToString() + imageUrl;
                return fullLogoUrl;
            }
            return null;

        }
        private readonly List<string> _allowedExtensions = new List<string>
{
    ".jpg", ".jpeg", ".png" // Add other allowed extensions as needed
};

        private bool IsValidFileType(string extension)
        {
            return _allowedExtensions.Contains(extension.ToLowerInvariant());
        }
        private string SanitizeFileName(string fileName)
        {
            // Remove invalid characters and replace spaces with underscores
            var invalidChars = Path.GetInvalidFileNameChars();
            var sanitizedFileName = string.Join("_", fileName.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries));
            return sanitizedFileName;
        }
    }
}

