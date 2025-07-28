namespace InventoryAPI.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<FileService> _logger;

        public FileService(IWebHostEnvironment webHostEnvironment, ILogger<FileService> logger)
        {
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }

        public async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            if (imageFile.ContentType.ToLower() != "image/png")
            {
                throw new ArgumentException("Only .png files are allowed.");
            }

            try
            {
                var contentPath = _webHostEnvironment.WebRootPath;
                var path = Path.Combine(contentPath, "images");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                var ext = Path.GetExtension(imageFile.FileName);
                var fileName = $"{Guid.NewGuid()}{ext}";
                var fileNameWithPath = Path.Combine(path, fileName);

                using var stream = new FileStream(fileNameWithPath, FileMode.Create);
                await imageFile.CopyToAsync(stream);

                _logger.LogInformation("Successfully saved image to {Path}", fileNameWithPath);
                return $"/images/{fileName}"; 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving image file.");
                throw;
            }
        }

        public void DeleteImage(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
                return;

            try
            {
                var contentPath = _webHostEnvironment.WebRootPath;
                var fullPath = Path.Combine(contentPath, imagePath.TrimStart('/'));

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    _logger.LogInformation("Successfully deleted image at {Path}", fullPath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting image file at {Path}", imagePath);
            }
        }
    }
}