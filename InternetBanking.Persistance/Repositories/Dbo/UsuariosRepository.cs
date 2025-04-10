

using InternetBanking.Domain.Result;
using InternetBanking.Identity.Shared.Context;
using InternetBanking.Identity.Shared.Entities;
using InternetBanking.Persistance.Context;
using InternetBanking.Persistance.Interfaces.dbo;
using InternetBanking.Persistance.Models.dbo;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InternetBanking.Persistance.Repositories.Dbo
{
    public sealed class UsuariosRepository : IUsuariosRepository
    {
        private readonly InternetBankingContext _internetBankingContext;
        private readonly ILogger<UsuariosRepository> _logger;
        private readonly IdentityContext _identityContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public UsuariosRepository(InternetBankingContext internetBankingContext, IdentityContext identityContext,
                                  ILogger<UsuariosRepository> logger, UserManager<ApplicationUser> userManager)
        {
            _internetBankingContext = internetBankingContext;
            _logger = logger;
            _identityContext = identityContext;
            _userManager = userManager;
        }

        public async Task<OperationResult> UpdateIdentityUser(ApplicationUser user, decimal? monto)
        {
            OperationResult result = new OperationResult();

            OperationResult SetError(string errorMessage)
            {
                result.Success = false;
                result.Message = errorMessage;
                return result;
            }

            using (var transaction = await _identityContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var roles = await _userManager.GetRolesAsync(user);

                    if (roles.Contains("Admin"))
                    {
                        ApplicationUser? userToUpdate = await _identityContext.Users.FindAsync(user.Id);
                        if (userToUpdate == null)
                        {
                            return SetError("Usuario no encontrado.");
                        }

                        userToUpdate.Nombre = user.Nombre;
                        userToUpdate.Apellido = user.Apellido;
                        userToUpdate.Cedula = user.Cedula;
                        userToUpdate.Email = user.Email;
                        userToUpdate.UserName = user.UserName;
                        userToUpdate.PasswordHash = user.PasswordHash; 

                        var updateResult = await _userManager.UpdateAsync(userToUpdate);

                        if (!updateResult.Succeeded)
                            return SetError("Ha ocurrido un error actualizando el usuario.");

                        await transaction.CommitAsync();
                        result.Success = true;
                        result.Message = "Usuario tipo Admin actualizado correctamente.";
                    }
                    else if (roles.Contains("Basic"))
                    {
                        var checkAccount = await _internetBankingContext.CuentasAhorros
                            .FirstOrDefaultAsync(c => c.UsuarioID == user.Id);

                        ApplicationUser? userToUpdate = await _identityContext.Users.FindAsync(user.Id);
                        if (userToUpdate == null)
                        {
                            return SetError("Usuario no encontrado.");
                        }

                        userToUpdate.Nombre = user.Nombre;
                        userToUpdate.Apellido = user.Apellido;
                        userToUpdate.Cedula = user.Cedula;
                        userToUpdate.Email = user.Email;
                        userToUpdate.UserName = user.UserName;
                        userToUpdate.PasswordHash = user.PasswordHash; 

                        if (checkAccount != null && monto.HasValue)
                        {
                            checkAccount.Saldo += monto.Value;
                            await _internetBankingContext.SaveChangesAsync();
                        }

                        var updateResult = await _userManager.UpdateAsync(userToUpdate);

                        if (!updateResult.Succeeded)
                            return SetError("Ha ocurrido un error actualizando el usuario.");

                        await transaction.CommitAsync();
                    }
                    else
                    {
                        return SetError("Rol de usuario no reconocido.");
                    }
                }
                catch (Exception ex)
                {
                    result.Success = false;
                    result.Message = "Hubo un error inesperado: " + ex.Message;
                }
                return result;
            }
        }

        public async Task<OperationResult> GetIdentityUserBy(string userId)
        {
            OperationResult result = new OperationResult();

            try
            {
                var usuarios = await _identityContext.Users.ToListAsync();
                var roles = await _identityContext.Roles.ToListAsync();
                var usuarioRoles = await _identityContext.UserRoles.ToListAsync();

                var datos = (from usuario in usuarios
                             join usuarioRol in usuarioRoles on usuario.Id equals usuarioRol.UserId
                             join rol in roles on usuarioRol.RoleId equals rol.Id

                             where usuario.Id == userId

                             select new UsuariosModel()
                             {
                                 Id = usuario.Id,
                                 Nombre = usuario.Nombre,
                                 Apellido = usuario.Apellido,
                                 UserName = usuario.UserName,
                                 Cedula = usuario.Cedula,
                                 Correo = usuario.Email,
                                 Telefono = usuario.PhoneNumber,
                                 Rol = rol.Name,
                                 IsActive = usuario.IsActive

                             }).FirstOrDefault();

                result.Data = datos;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Ha ocurrido un error obteniendo los usuarios.";
                _logger.LogError(result.Message, ex.ToString());
            }
            return result;
        }

        public async Task<OperationResult> GetIdentityUserAll()
        {
            OperationResult result = new OperationResult();

            try
            {
                var usuarios = await _identityContext.Users.ToListAsync();
                var roles = await _identityContext.Roles.ToListAsync();
                var usuarioRoles = await _identityContext.UserRoles.ToListAsync();

                var datos = (from usuario in usuarios
                             join usuarioRol in usuarioRoles on usuario.Id equals usuarioRol.UserId
                             join rol in roles on usuarioRol.RoleId equals rol.Id

                             select new UsuariosModel()
                             {
                                 Id = usuario.Id,
                                 Nombre = usuario.Nombre,  
                                 Apellido = usuario.Apellido,
                                 UserName = usuario.UserName,
                                 Cedula = usuario.Cedula,
                                 Correo = usuario.Email,  
                                 Telefono = usuario.PhoneNumber,  
                                 Rol = rol.Name,
                                 IsActive = usuario.IsActive

                             }).ToList();

                result.Data = datos;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Ha ocurrido un error obteniendo los usuarios.";
                _logger.LogError(result.Message, ex.ToString());
            }
            return result;
        }

        public async Task<OperationResult> GetUserByRol(string rol)
        {
            OperationResult result = new OperationResult();

            try
            {
                var usuarios = await _identityContext.Users.ToListAsync();
                var roles = await _identityContext.Roles.ToListAsync();
                var usuarioRoles = await _identityContext.UserRoles.ToListAsync();

                var datos = (from usuario in usuarios
                             join usuarioRol in usuarioRoles on usuario.Id equals usuarioRol.UserId
                             join rolDb in roles on usuarioRol.RoleId equals rolDb.Id 

                             where rolDb.Name == rol 

                             select new UsuariosModel()
                             {
                                 Id = usuario.Id,
                                 Nombre = usuario.Nombre,
                                 Apellido = usuario.Apellido,
                                 Cedula = usuario.Cedula,
                                 Correo = usuario.Email,
                                 Telefono = usuario.PhoneNumber,
                                 Rol = rolDb.Name,
                                 IsActive = usuario.IsActive

                             }).ToList(); 

                result.Data = datos;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Ha ocurrido un error obteniendo los usuarios.";
                _logger.LogError(result.Message, ex.ToString());
            }
            return result;
        }

        public async Task<OperationResult> GetUserByCedula(string cedula)
        {
            OperationResult result = new OperationResult();

            try
            {
                var usuarios = await _identityContext.Users.ToListAsync();
                var roles = await _identityContext.Roles.ToListAsync();
                var usuarioRoles = await _identityContext.UserRoles.ToListAsync();

                var datos = (from usuario in usuarios
                             join usuarioRol in usuarioRoles on usuario.Id equals usuarioRol.UserId
                             join rolDb in roles on usuarioRol.RoleId equals rolDb.Id

                             where usuario.Cedula == cedula

                             select new UsuariosModel()
                             {
                                 Id = usuario.Id,
                                 Nombre = usuario.Nombre,
                                 Apellido = usuario.Apellido,
                                 Cedula = usuario.Cedula,
                                 Correo = usuario.Email,
                                 Telefono = usuario.PhoneNumber,
                                 Rol = rolDb.Name,
                                 IsActive = usuario.IsActive

                             }).ToList();

                result.Data = datos;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Ha ocurrido un error obteniendo el usuario.";
                _logger.LogError(result.Message, ex.ToString());
            }
            return result;
        }

        public async Task<OperationResult> ActivarOrDesactivar(string userId)
        {
            OperationResult result = new OperationResult();

            try
            {
                var usuario = await _identityContext.Users.FindAsync(userId);

                if (usuario == null)
                {
                    result.Success = false;
                    result.Message = "Usuario no encontrado.";
                    return result;
                }

                usuario.IsActive = !usuario.IsActive;

                _identityContext.Update(usuario);
                await _identityContext.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Error al procesar la solicitud: {ex.Message}";
            }
            return result;
        }

    }
}

