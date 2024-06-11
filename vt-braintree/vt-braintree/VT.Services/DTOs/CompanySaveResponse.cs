using VT.Data.Entities;

namespace VT.Services.DTOs
{
    public class CompanySaveResponse : BaseResponse
    {
        public Company Company { get; set; } 
    }
}
