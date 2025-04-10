

using AutoMapper;
using InternetBanking.Application.Contracts.dbo;
using InternetBanking.Application.Core;
using InternetBanking.Application.Dtos.dbo;
using InternetBanking.Domain.Entities.dbo;
using InternetBanking.Persistance.Interfaces.dbo;
using Microsoft.Extensions.Logging;

namespace InternetBanking.Application.Services.dbo
{
    public class HistorialTransaccionesService : IHistorialTransaccionesService
    {
        private readonly IHistorialTransaccionesRepository _historialTransaccionesRepository;
        private readonly ILogger<HistorialTransaccionesService> _logger;
        private readonly IMapper _mapper;

        public HistorialTransaccionesService(IHistorialTransaccionesRepository historialTransaccionesRepository,
                                            ILogger<HistorialTransaccionesService> logger, IMapper mapper)
        {
            _historialTransaccionesRepository = historialTransaccionesRepository;
            _logger = logger;
            _mapper = mapper;
        }
        public async Task<ServiceResponse> GetAllAsync()
        {
            ServiceResponse response = new ServiceResponse();

            try
            {
                var result = await _historialTransaccionesRepository.GetAll();

                if (!result.Success)
                {
                    result.Success = response.IsSuccess;
                    result.Message = response.Messages;

                    return response;
                }
                response.Model = result.Data;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Messages = "Ha ocurrido un error obteniendo los hitoriales de transacciones.";
                _logger.LogError(response.Messages, ex.ToString());
            }
            return response;
        }

        public async Task<ServiceResponse> GetByIDAsync(int id)
        {
            ServiceResponse response = new ServiceResponse();

            try
            {
                var result = await _historialTransaccionesRepository.GetById(id);

                if (!result.Success)
                {
                    result.Success = response.IsSuccess;
                    result.Message = response.Messages;

                    return response;
                }
                response.Model = result.Data;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Messages = "Ha ocurrido un error obteniendo el hitoriales de transacciones.";
                _logger.LogError(response.Messages, ex.ToString());
            }
            return response;
        }

        public async Task<ServiceResponse> RemoveAsync(HistorialTransaccionesDto dto)
        {
            ServiceResponse response = new ServiceResponse();

            try
            {
                HistorialTransacciones historial = new HistorialTransacciones();

                historial.HistorialID = dto.HistorialID;
                var result = await _historialTransaccionesRepository.Remove(historial);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Messages = "Ha ocurrido un error eliminando el hitoriales de transacciones.";
                _logger.LogError(response.Messages, ex.ToString());
            }
            return response;
        }

        public async Task<ServiceResponse> SaveAsync(HistorialTransaccionesDto dto)
        {
            ServiceResponse response = new ServiceResponse();

            try
            {
                var historial = _mapper.Map<HistorialTransacciones>(dto);
                var result = await _historialTransaccionesRepository.Save(historial);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Messages = "Ha ocurrido un error guardando el hitoriales de transacciones.";
                _logger.LogError(response.Messages, ex.ToString());
            }
            return response;
        }

        public async Task<ServiceResponse> UpdateAsync(HistorialTransaccionesDto dto)
        {
            ServiceResponse response = new ServiceResponse();

            try
            {
                var resultGetBy = await _historialTransaccionesRepository.GetById(dto.HistorialID);

                if (!resultGetBy.Success)
                {
                    response.IsSuccess = resultGetBy.Success;
                    response.Messages = resultGetBy.Message;

                    return response;
                }

                var historial = _mapper.Map<HistorialTransacciones>(dto);
                var result = await _historialTransaccionesRepository.Update(historial);
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Messages = "Ha ocurrido un error actualizando el hitoriales de transacciones.";
                _logger.LogError(response.Messages, ex.ToString());
            }
            return response;
        }
    }
}
