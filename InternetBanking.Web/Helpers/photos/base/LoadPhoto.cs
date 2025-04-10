namespace InternetBanking.Web.Helpers.photos.@base
{
    public class LoadPhoto
    {
        private readonly IWebHostEnvironment _webHost;

        public LoadPhoto(IWebHostEnvironment webHostEnvironment)
        {
            _webHost = webHostEnvironment;
        }

        public async Task<string> SaveFileAsync(IFormFile Foto)
        {

            if (Foto != null && Foto.Length > 0)
            {
                string uploadsFolder = Path.Combine(_webHost.WebRootPath, "images", "app");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string fileExtension = Path.GetExtension(Foto.FileName);
                string uniqueFileName = Guid.NewGuid().ToString() + fileExtension;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await Foto.CopyToAsync(fileStream);
                }

                return "/images/app/" + uniqueFileName;
            }

            return null;
        }
    }
}
