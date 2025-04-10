namespace InternetBanking.Web.Helpers.photos.@base
{
    public interface ILoadPhoto<Dto, T>
    {
        Task<Dto> LoadPhoto(Dto dto, T entity);
    }
}
