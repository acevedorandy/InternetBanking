

namespace InternetBanking.Application.Core
{
    public class ServiceResponse
    {
        public ServiceResponse()
        {
            this.IsSuccess = true;
        }
        public bool IsSuccess { get; set; }
        public string? Messages { get; set; }
        public dynamic? Model { get; set; }
    }
}
