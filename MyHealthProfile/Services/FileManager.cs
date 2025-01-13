using MyHealthProfile.Services.Interfaces;

namespace MyHealthProfile.Services
{
    public class FileManager: IFileManager
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _config;
        public FileManager(IWebHostEnvironment environment, IConfiguration config)
        {
            _environment = environment;
            _config = config;
        }

        public string CreateFile(IFormFile file)
        {
            string wwwrootPath = _environment.WebRootPath ?? throw new InvalidOperationException("WebRootPath is not set.");


            if (file != null)
            {
                var uploads = Path.Combine(wwwrootPath, @"Images\company");
                var extension = Path.GetExtension(file.FileName);

                // Validate the file type
                if (!IsValidFileType(extension))
                {
                    throw new NotSupportedException("File type is not supported.");
                }

                // Sanitize the file name
                string sanitizedFileName = SanitizeFileName(Guid.NewGuid().ToString());

                // Ensure the directory exists
                if (!Directory.Exists(uploads))
                {
                    Directory.CreateDirectory(uploads);
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

                // Construct the URL to the uploaded image
                string imageUrl = @"/Images/company/" + sanitizedFileName + extension;
                string fullLogoUrl = _config["BaseUrl"].ToString() + imageUrl;
                return fullLogoUrl;
            }

            return null;
        }

        public  string UpdateFile(IFormFile file, string OldFile)
        {

            string wwwrootPath = _environment.WebRootPath ?? throw new InvalidOperationException("WebRootPath is not set.");
            if (file != null)
            {
                var uploads = Path.Combine(wwwrootPath, "Images", "company");
                var extension = Path.GetExtension(file.FileName);

                // Validate the file type
                if (!IsValidFileType(extension))
                {
                    throw new NotSupportedException("File type is not supported.");
                }

                // Sanitize the file name
                string sanitizedFileName = SanitizeFileName(Guid.NewGuid().ToString());
                if (OldFile != null)
                {
                    //var oldimagepath = Path.Combine(wwwrootPath, obj.product.ImageUrl.TrimStart('\\'));
                    if (System.IO.File.Exists(OldFile))
                    {

                        System.IO.File.Delete(OldFile);
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
                string imageUrl = @"/Images/company/" + sanitizedFileName + extension;
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

