using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 
using VT.Data.Context;
using VT.Data.Entities;
using VT.Services.DTOs;
using VT.Services.Interfaces;

namespace VT.Services.Services
{
    public class CompanyServiceService : ICompanyServiceService
    {
        #region Field(s)

        private readonly IVerifyTechContext _context;

        #endregion

        #region Constructor

        public CompanyServiceService(IVerifyTechContext context)
        {
            _context = context;
        }

        #endregion

        #region Interface implementation

        public CompanySaveResponse Save(CompanySaveRequest request)
        {
            var response = new CompanySaveResponse();

            if (request.OrganizationId > 0) //Edit
            {
                var company = _context.Companies.FirstOrDefault(x => x.CompanyId == request.OrganizationId);

                if (company == null)
                {
                    response.Message = "This organization does not exist";
                    return response;
                }

                company.Name = request.Name;
                _context.SaveChanges();
            }
            else //Add
            {
                _context.Companies.Add(new Company
                {
                    Name = request.Name
                });
                _context.SaveChanges();
            }
            response.Success = true;
            return response;
        }

        public Company GetCompany(int companyId)
        {
            return _context.Companies.FirstOrDefault(x => x.CompanyId == companyId);
        }

        public IList<Company> GetAllCompanies()
        {
            return _context.Companies.ToList();
        }

        public BaseResponse DeleteCompany(int companyId)
        {
            var response = new BaseResponse();

            var company = _context.Companies.FirstOrDefault(x => x.CompanyId == companyId);

            if (company == null)
            {
                response.Message = "This organization does not exist";
                return response;
            }

            _context.Companies.Remove(company);
            _context.SaveChanges();
            response.Success = true;
            return response;
        }

        public BaseResponse DeleteCompanyServices(List<int> ids)
        {
            var response = new BaseResponse();
            try
            {
                var records = _context.CustomerServices.Where(x => ids.Contains(x.CompanyServiceId) && !x.IsDeleted).ToList();

                if (records.Count == 0)
                {
                    var companyServices =
                        _context.CompanyServices.Where(x => !x.IsDeleted && ids.Contains(x.CompanyServiceId)).ToList();

                    foreach (var companyService in companyServices)
                    {
                        companyService.IsDeleted = true;
                        companyService.Name = string.Format("{0}_DELETED_{1}", companyService.Name,
                            DateTime.UtcNow.ToString("O"));
                    }

                    _context.SaveChanges();
                    response.Success = true;
                }
                else
                {
                    response.Success = false;
                    var services = new List<string>();

                    var message = new StringBuilder("No services have been deleted because the following services are in use by some customers. Please issue a request to delete services that are not in use by anyone. <br/><br/>");
                     
                    foreach (var customerService in records.Where(customerService => 
                    !services.Contains(customerService.CompanyService.Name))) 
                        services.Add(customerService.CompanyService.Name);

                    foreach (var service in services) 
                        message.Append(service + "<br/>"); 

                    response.Message = message.ToString();
                }
            }
            catch (Exception exception)
            {
                response.Message = exception.ToString();
            }
            return response;
        }

        public Data.Entities.CompanyService GetCompanyService(int companyServiceId)
        {
            return _context.CompanyServices.FirstOrDefault(x => !x.IsDeleted && x.CompanyServiceId == companyServiceId);
        }

        public IList<Data.Entities.CompanyService> GetAllCompanyServices(int? companyId = null)
        {
            return companyId == null
                ? _context.CompanyServices.Where(x => !x.IsDeleted).ToList()
                : _context.CompanyServices.Where(x => x.CompanyId == companyId && !x.IsDeleted).ToList();
        }

        public SaveCompanyServiceResponse SaveCompanyService(SaveCompanyServiceRequest request)
        {
            var response = new SaveCompanyServiceResponse();
            if (request.CompanyServiceId > 0) // Edit
            {
                var companyService = _context.CompanyServices.FirstOrDefault(x => !x.IsDeleted && x.CompanyServiceId == request.CompanyServiceId);
                if (companyService == null)
                {
                    response.Message = "This service does not exist in the database";
                    return response;
                }
                else
                {
                    companyService.CompanyId = request.CompanyId;
                    companyService.Name = request.Name;
                    companyService.Description = request.Description;
                }
                _context.SaveChanges();
                response.Success = true;
            }
            else
            {
                _context.CompanyServices.Add(new Data.Entities.CompanyService
                {
                    CompanyId = request.CompanyId,
                    Description = request.Description,
                    Name = request.Name
                });
                _context.SaveChanges();
                response.Success = true;
            }
            return response;
        }

        public IList<Data.Entities.CompanyService> GetFilteredCompanyServices(int companyId, int customerId)
        {
            var companyServiceIds =  _context.CustomerServices.Where(x => !x.IsDeleted && x.CustomerId == customerId)
                .Select(x => x.CompanyServiceId).ToList();
            return _context.CompanyServices.Where(x => !x.IsDeleted && !companyServiceIds.Contains(x.CompanyServiceId) && x.CompanyId == companyId).ToList();
        }

        #endregion
    }
}
