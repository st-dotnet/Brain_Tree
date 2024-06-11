using System.Collections.Generic;
using VT.Data.Entities;
using VT.Services.DTOs;

namespace VT.Services.Interfaces
{
    public interface ICustomerServiceService
    {
        SaveCustomerServiceResponse SaveCustomerService(SaveCustomerServiceRequest request);
        IList<CustomerService> GetCustomerServices(int customerId);
        CustomerService GetCustomerService(int customerServiceId);
        CustomerService GetCustomerServiceByCompanyServiceId(int companyServiceId);
        BaseResponse DeleteCustomerServices(List<int> ids);
        BaseResponse DeleteCustomerService(int id);
    }
}
