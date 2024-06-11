using System.Collections.Generic;
using VT.Data.Entities;
using VT.Services.DTOs;

namespace VT.Services.Interfaces
{
    public interface ICustomerService
    {
        IList<Customer> GetAllCustomers(int? companyId);
        IList<Customer> GetAllCustomersForUser(int userId);
        Customer GetCustomer(int customerId);
        Customer GetCustomer(string token);
        CustomerDetailResponse GetCustomerDetail(int customerId);
        CustomerSaveResponse SaveCustomer(CustomerSaveRequest request);
        BaseResponse DeleteCustomers(List<int> ids);
        BaseResponse SetExpireTokenForCustomer(CustomerSetExpireTokenRequest request);
        IList<string> GetCustomerContactEmails();
        IList<string> GetCustomerNames();
        BaseResponse BatchSave(CustomerBatchRequest request);
        BaseResponse UserCustomerAccess(UserCustomerAccessRequest request);
        List<CompanyWorkerCustomer> GetUserCustomerAccess(int companyWorkerId);
        IList<Customer> GetUserCustomers(int companyWorkerId);
    }
}
