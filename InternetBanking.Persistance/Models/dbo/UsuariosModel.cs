

using System.Collections.Specialized;

namespace InternetBanking.Persistance.Models.dbo
{
    public class UsuariosModel
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string UserName { get; set; }
        public string Cedula { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }
        public string Rol { get; set; }
        public bool IsActive { get; set; } 


    }
}
