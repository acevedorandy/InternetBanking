

using AutoMapper;
using InternetBanking.Application.Dtos.dbo;
using InternetBanking.Application.Dtos.dbo.avance;
using InternetBanking.Application.Dtos.dbo.pagos.beneficiario;
using InternetBanking.Application.Dtos.dbo.pagos.cuenta;
using InternetBanking.Application.Dtos.dbo.pagos.expreso;
using InternetBanking.Application.Dtos.dbo.pagos.prestamo;
using InternetBanking.Application.Dtos.dbo.pagos.tarjeta;
using InternetBanking.Domain.Entities.dbo;

namespace InternetBanking.Application.Mapping.dbo
{
    public class DboMapping : Profile
    {
        public DboMapping()
        {
            CreateMap<Beneficiarios, BeneficiariosDto>()
                .ReverseMap();

            CreateMap<CuentasAhorro, CuentasAhorroDto>()
                .ReverseMap();

            CreateMap<HistorialTransacciones, HistorialTransaccionesDto>()
                .ReverseMap();

            CreateMap<Prestamos, PrestamosDto>()
                .ReverseMap();

            CreateMap<TarjetasCredito, TarjetasCreditoDto>()
                .ReverseMap();

            CreateMap<Transacciones, TransaccionesDto>()
                .ReverseMap();

            CreateMap<Usuarios, UsuariosDto>()
                .ReverseMap();

            // Mapping de los diferentes tipos de transferencias 

            CreateMap<Transacciones, ConfirmarPagoExpresoDto>()
                .ForMember(x => x.CuentaOrigenID, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<Transacciones, AvanceEfectivoDto>()
                .ForMember(x => x.NumeroTarjeta, opt => opt.Ignore())
                .ForMember(x => x.TarjetaID, opt => opt.Ignore())
                .ForMember(x => x.NumeroCuenta, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<Transacciones, PagoTarjetaDto>()
                .ForMember(x => x.TarjetaID, opt => opt.Ignore())
                .ForMember(x => x.SaldoDisponible, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<Transacciones, PagoPrestamoDto>()
                .ForMember(x => x.PrestamoID, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<Transacciones, PagoBeneficiarioDto>()
                .ForMember(x => x.CuentaBeneficiarioID, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<Transacciones, PagoCuentaDto>()
                .ForMember(dest => dest.CuentaDestinoID, opt => opt.Ignore()) 
                .ForMember(dest => dest.CuentaOrigenID, opt => opt.MapFrom(src => src.CuentaID)) 
                .ReverseMap()
                .ForMember(dest => dest.CuentaID, opt => opt.MapFrom(src => src.CuentaOrigenID)); 

        }
    }
}
