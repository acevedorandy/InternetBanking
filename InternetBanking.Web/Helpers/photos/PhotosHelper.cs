using InternetBanking.Application.Dtos.dbo;
using InternetBanking.Application.Dtos.identity.account;
using InternetBanking.Web.Helpers.photos.@base;

namespace InternetBanking.Web.Helpers.photos
{
    public class PhotosHelper
    {
        private readonly IWebHostEnvironment _webHost;
        private readonly LoadPhoto _loadPhoto;

        public PhotosHelper(IWebHostEnvironment webHostEnvironment, LoadPhoto loadPhoto)
        {
            _webHost = webHostEnvironment;
            _loadPhoto = loadPhoto;
        }
        public async Task<TarjetasCreditoDto> LoadPhoto(TarjetasCreditoDto dto, IFormFile Foto)
        {
            // Solo borrar la foto si se sube una nueva
            if (Foto != null && Foto.Length > 0)
            {
                if (!string.IsNullOrEmpty(dto.Icono))
                {
                    string oldPhotoPath = dto.Icono;
                    string fullOldPhotoPath = Path.Combine(_webHost.WebRootPath, oldPhotoPath.TrimStart('/'));
                    if (File.Exists(fullOldPhotoPath))
                    {
                        File.Delete(fullOldPhotoPath);
                    }
                }

                string filePath = await _loadPhoto.SaveFileAsync(Foto);

                if (!string.IsNullOrEmpty(filePath))
                {
                    dto.Icono = filePath;
                }
            }

            return dto;
        }
    }
}
