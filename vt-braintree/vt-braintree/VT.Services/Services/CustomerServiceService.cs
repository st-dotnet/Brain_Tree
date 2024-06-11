using System;
using System.Collections.Generic;
using System.Linq;
using VT.Data.Context;
using VT.Services.DTOs;
using VT.Services.Interfaces;

namespace VT.Services.Services
{
    public class CustomerServiceService : ICustomerServiceService
    {
        #region Field(s)

        private readonly IVerifyTechContext _context;

        #endregion

        #region Constructor

        public CustomerServiceService(IVerifyTechContext context)
        {
            _context = context;
        }

        #endregion

        #region Interface implementation

        public SaveCustomerServiceResponse SaveCustomerService(SaveCustomerServiceRequest request)
        {
            var response = new SaveCustomerServiceResponse();

            var customerService = (request.CustomerServiceId > 0)
                ? _context.CustomerServices.FirstOrDefault(x => x.CustomerServiceId == request.CustomerServiceId)
                : new Data.Entities.CustomerService();

            if (customerService == null)
            {
                return new SaveCustomerServiceResponse
                {
                    Message = "Customer Service does not exist."
                };
            }

            customerService.Name = request.Name;
            customerService.Description = request.Description;
            customerService.Cost = request.Price;
            customerService.CustomerId = request.CustomerId;
            customerService.CompanyServiceId = request.CompanyServiceId;

            try
            {
                if (customerService.CustomerServiceId == 0)
                {

                    var checkForExistingRecord = _context.CustomerServices.FirstOrDefault(x => x.CustomerId == customerService.CustomerId && x.CompanyServiceId == customerService.CompanyServiceId);

                    if (checkForExistingRecord != null)
                    {
                        checkForExistingRecord.Name = request.Name;
                        checkForExistingRecord.Description = request.Description;
                        checkForExistingRecord.Cost = request.Price;
                        checkForExistingRecord.IsDeleted = false;
                    }
                    else
                    {
                        _context.CustomerServices.Add(customerService);
                    }                    
                }
                    

                _context.SaveChanges();
                response.Success = true;
            }
            catch (Exception exception)
            {
                response.Message = exception.ToString();
            }
            return response;
        }

        public IList<Data.Entities.CustomerService> GetCustomerServices(int customerId)
        {
            return _context.CustomerServices.Where(x => !x.IsDeleted && x.CustomerId == customerId).ToList();
        }

        public Data.Entities.CustomerService GetCustomerService(int customerServiceId)
        {
            return _context.CustomerServices.FirstOrDefault(x => !x.IsDeleted && x.CustomerServiceId == customerServiceId);
        }

        public Data.Entities.CustomerService GetCustomerServiceByCompanyServiceId(int companyServiceId)
        {
            return _context.CustomerServices.FirstOrDefault(x => !x.IsDeleted && x.CompanyServiceId == companyServiceId);
        }

        public BaseResponse DeleteCustomerService(int id)
        {
            var customerService =
                _context.CustomerServices.FirstOrDefault(x => !x.IsDeleted &&  x.CustomerServiceId == id);

            if (customerService == null)
            {
                return new BaseResponse {Message = "Customer Service does not exists."};
            }
            else
            {
                customerService.IsDeleted = true;
                _context.SaveChanges();
                return new BaseResponse { Success = true};
            }
        }

        public BaseResponse DeleteCustomerServices(List<int> ids)
        {
            var customerServices =
                _context.CustomerServices.Where(x => !x.IsDeleted && ids.Contains(x.CustomerServiceId)).ToList();

            foreach (var customerService in customerServices)
            {
                customerService.IsDeleted = true;
            }

            if (customerServices.Any())
            {
                _context.SaveChanges();
                return new BaseResponse { Success = true };
            }
            else
            {
                return new BaseResponse { Success = false, Message = "Select al least one customer service to delete"};
            }
        }

        #endregion

    }
}
