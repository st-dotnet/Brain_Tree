using System.Collections.Generic;
using VT.Data.Entities;
using VT.Services.DTOs;

namespace VT.Services.Interfaces
{
    public interface ICompanyServiceService
    {
        CompanyService GetCompanyService(int companyServiceId);
        IList<CompanyService> GetAllCompanyServices(int? companyId = null);
        SaveCompanyServiceResponse SaveCompanyService(SaveCompanyServiceRequest request);
        BaseResponse DeleteCompanyServices(List<int> ids);
        IList<CompanyService> GetFilteredCompanyServices(int companyId, int customerId);
    }
}
