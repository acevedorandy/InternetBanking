

using Microsoft.AspNetCore.Identity;

namespace InternetBanking.Identity.Shared.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Cedula { get; set; }
        public bool IsActive { get; set; } = true;

    }
}
