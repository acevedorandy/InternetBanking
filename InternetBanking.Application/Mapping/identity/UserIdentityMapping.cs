

using InternetBanking.Application.Dtos.identity.account;
using InternetBanking.Application.Dtos.identity;
using AutoMapper;
using InternetBanking.Domain.Entities.dbo;
using InternetBanking.Identity.Shared.Entities;
using InternetBanking.Application.Dtos.dbo;

namespace InternetBanking.Application.Mapping.identity
{
    public class UserIdentityMapping : Profile
    {
        public UserIdentityMapping()
        {
            CreateMap<AuthenticationRequest, LoginDto>()
                .ForMember(x => x.HasError, opt => opt.Ignore())
                .ForMember(x => x.Error, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<RegisterRequest, RegisterDto>()
                .ForMember(x => x.HasError, opt => opt.Ignore())
                .ForMember(x => x.Error, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<ForgotPasswordRequest, ForgotPasswordDto>()
                .ForMember(x => x.HasError, opt => opt.Ignore())
                .ForMember(x => x.Error, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<ResetPasswordRequest, ResetPasswordDto>()
                .ForMember(x => x.HasError, opt => opt.Ignore())
                .ForMember(x => x.Error, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<RegisterRequest, RegisterDto>()
                .ReverseMap();

            CreateMap<RegisterBasicUserRequest, RegisterBasicUserDto>()
                .ForMember(x => x.NumeroCuenta, opt => opt.Ignore())
                .ForMember(x => x.Saldo, opt => opt.Ignore())
                .ForMember(x => x.Principal, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<CuentasAhorro, RegisterBasicUserDto>()
                .ForMember(x => x.NumeroCuenta, opt => opt.Ignore())
                .ForMember(x => x.Saldo, opt => opt.Ignore())
                .ForMember(x => x.Principal, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.UsuarioID))
                .ReverseMap()
                .ForMember(dest => dest.UsuarioID, opt => opt.MapFrom(src => src.Id));

            CreateMap<ApplicationUser, UsuariosDto>()
                .ReverseMap();
        }
    }
}
