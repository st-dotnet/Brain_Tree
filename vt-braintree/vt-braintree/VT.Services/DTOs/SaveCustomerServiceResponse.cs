using VT.Data.Entities;

namespace VT.Services.DTOs
{
    public class SaveCustomerServiceResponse : BaseResponse
    {
        public CustomerService CustomerService { get; set; }
    }
}
