using VT.Data.Entities;

namespace VT.Services.DTOs
{
    public class LoginResponse : BaseResponse
    {
        public CompanyWorker CompanyWorker { get; set; }
    }
}
