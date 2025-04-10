

using InternetBanking.Domain.Repositories;
using InternetBanking.Domain.Result;
using InternetBanking.Persistance.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace InternetBanking.Persistance.Base
{
    public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        private readonly InternetBankingContext _internetBankingContext;
        private DbSet<TEntity> entities;

        public BaseRepository(InternetBankingContext internetBankingContext)
        {
            _internetBankingContext = internetBankingContext;
            this.entities = internetBankingContext.Set<TEntity>();
        }


        public virtual async Task<OperationResult> Remove(TEntity entity)
        {
            OperationResult result = new OperationResult();

            try
            {
                entities.Remove(entity);
                await _internetBankingContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                result.Success = false;
                result.Message = "Hubo un error eliminando la entidad.";
            }
            return result;
        }

        public virtual async Task<bool> Exists(Expression<Func<TEntity, bool>> filter)
        {
            return await this.entities.AnyAsync(filter);
        }

        public virtual async Task<OperationResult> GetAll()
        {
            OperationResult result = new OperationResult();

            try
            {
                var datos = await this.entities.ToListAsync();
                result.Data = datos;
            }
            catch (Exception)
            {
                result.Success = false;
                result.Message = "Hubo un error obteniendo las entidades.";
            }
            return result;
        }

        public virtual async Task<OperationResult> GetById(int id)
        {
            OperationResult result = new OperationResult();

            try
            {
                var entity = await this.entities.FindAsync(id);
                result.Data = entity;
            }
            catch (Exception)
            {
                result.Success = false;
                result.Message = "hubo un error obteniendo la entidad.";
            }
            return result;
        }

        public virtual async Task<OperationResult> Save(TEntity entity)
        {
            OperationResult result = new OperationResult();

            try
            {
                entities.Add(entity);
                await _internetBankingContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                result.Success = false;
                result.Message = "Hubo un error guardando la entidad.";
            }
            return result;
        }

        public virtual async Task<OperationResult> Update(TEntity entity)
        {
            OperationResult result = new OperationResult();

            try
            {
                entities.Update(entity);
                await _internetBankingContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                result.Success = false;
                result.Message = "Hubo un error actualizando la entidad.";
            }
            return result;
        }
    }
}
