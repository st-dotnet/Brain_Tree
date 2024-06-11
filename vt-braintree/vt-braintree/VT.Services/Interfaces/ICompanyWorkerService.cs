using System.Collections.Generic;
using VT.Data.Entities;
using VT.Services.DTOs;

namespace VT.Services.Interfaces
{
    public interface ICompanyWorkerService
    {
        LoginResponse AuthenticateUser(LoginRequest request);
        IList<CompanyWorker> GetAllUsers(int? companyId);
        BaseResponse DeleteUsers(List<int> ids);
        CompanyWorkerSaveResponse AddUser(CompanyWorkerSaveRequest request);
        CompanyWorkerResponse GetCompanyWorker(int companyWorkerId);
        BaseResponse IsEmailAlreadyExist(string email);
        BaseResponse ResetUserPassword(ResetPasswordRequest request);
        List<string> GetAllActiveUsersEmail();
    }
}
