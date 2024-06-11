using System.Collections.Generic;
using VT.Data.Entities;
using VT.Services.DTOs;

namespace VT.Services.Interfaces
{
    public interface ICompanyService
    {
        CompanySaveResponse Save(CompanySaveRequest request);
        Company GetCompany(int companyId);
        IList<Company> GetOranizationList();
        IList<CompanyViewResponse> GetAllCompanies();
        BaseResponse DeleteCompany(int companyId);
        BaseResponse DeleteOrgs(List<int> ids);
        BaseResponse IsOrgNameExist(string name);
        OrganizationDetailResponse GetOrganizationDetail(int companyId);
        CompanySaveResponse SavePreferences(CompanyPreferencesRequest request); 
    }
}
