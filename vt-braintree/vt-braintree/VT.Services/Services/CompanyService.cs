using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using VT.Common;
using VT.Data.Context;
using VT.Data.Entities;
using VT.Services.DTOs;
using VT.Services.Interfaces;

namespace VT.Services.Services
{
    public class CompanyService : ICompanyService
    {
        #region Field(s)

        private readonly IVerifyTechContext _context;

        #endregion

        #region Constructor

        public CompanyService(IVerifyTechContext context)
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
                var company = _context.Companies
                    .Include(x => x.Addresses)
                    .Include(x => x.ContactPersons)
                    .FirstOrDefault(x => x.CompanyId == request.OrganizationId);

                if (company == null)
                {
                    response.Message = "This organization does not exist";
                    return response;
                }

                company.Name = request.Name;
                company.ServiceFeePercentage = request.ServiceFeePercentage;
                company.PaymentGatewayType = request.PaymentGatewayType;

                //company.IsGpsOn = request.IsGpsOn;
                //company.Threshold = request.Threshold;
                var address = company.Addresses.FirstOrDefault();
                var contactPerson = company.ContactPersons.FirstOrDefault();

                //Address
                if (address == null)
                {
                    AddAddress(company, request);
                }
                else
                {
                    address.StreetAddress = request.Address;
                    address.City = request.City;
                    address.Territory = request.State;
                    address.Country = request.Country;
                    address.PostalCode = request.PostalCode;
                }

                //Contact Person
                if (contactPerson == null)
                {
                    AddContactPerson(company, request);
                }
                else
                {
                    contactPerson.Email = request.ContactEmail;
                    contactPerson.FirstName = request.ContactFirstName;
                    contactPerson.MiddleName = request.ContactMiddleName;
                    contactPerson.LastName = request.ContactLastName;
                    contactPerson.Telephone = request.ContactTelephone;
                    contactPerson.Mobile = request.ContactMobile;
                }
                _context.SaveChanges();
            }
            else //Add
            {
                var org = new Company
                {
                    PaymentGatewayType = request.PaymentGatewayType,
                    Name = request.Name,
                    ServiceFeePercentage = request.ServiceFeePercentage 
                };
                AddAddress(org, request);
                AddContactPerson(org, request);

                _context.Companies.Add(org);
                _context.SaveChanges();
            }
            response.Success = true;
            return response;
        }

        public Company GetCompany(int companyId)
        {
            return _context.Companies.FirstOrDefault(x => x.CompanyId == companyId);
        }

        public BaseResponse DeleteOrgs(List<int> ids)
        {
            var response = new BaseResponse();
            try
            {
                var companies = _context.Companies.Where(x => ids.Contains(x.CompanyId)).ToList();

                foreach (var company in companies)
                {
                    company.IsDeleted = true;
                    company.Name = string.Format("{0}_DELETED_{1}", company.Name, DateTime.UtcNow.ToString("O"));
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

        public OrganizationDetailResponse GetOrganizationDetail(int companyId)
        {
            OrganizationDetailResponse response = null;

            var company = _context.Companies
                .Include(x => x.Addresses)
                .Include(x => x.ContactPersons)
                .FirstOrDefault(x => x.CompanyId == companyId);

            if (company != null)
            {
                var address = company.Addresses.FirstOrDefault();
                var contactPerson = company.ContactPersons.FirstOrDefault();

                response = new OrganizationDetailResponse
                {
                    OrganizationId = company.CompanyId,
                    Name = company.Name,
                    ServiceFeePercentage = company.ServiceFeePercentage,
                    Address = address != null ? address.StreetAddress : string.Empty,
                    City = address != null ? address.City : string.Empty,
                    State = address != null ? address.Territory : string.Empty,
                    PostalCode = address != null ? address.PostalCode : string.Empty,
                    Country = address != null ? address.Country : string.Empty,
                    IsDeleted = company.IsDeleted,
                    ContactFirstName = contactPerson != null ? contactPerson.FirstName : string.Empty,
                    ContactMiddleName = contactPerson != null ? contactPerson.MiddleName : string.Empty,
                    ContactLastName = contactPerson != null ? contactPerson.LastName : string.Empty,
                    ContactEmail = contactPerson != null ? contactPerson.Email : string.Empty,
                    ContactTelephone = contactPerson != null ? contactPerson.Telephone : string.Empty,
                    ContactMobile = contactPerson != null ? contactPerson.Mobile : string.Empty,
                    PaymentGatewayType = company.PaymentGatewayType,
                    Success = true
                };
            }
            else
            {
                response = new OrganizationDetailResponse
                {
                    Message = "Organization does not exist in the database."
                };
            }
            return response;
        }

        public CompanySaveResponse SavePreferences(CompanyPreferencesRequest request)
        {
            var response = new CompanySaveResponse();
            try
            {
                var company = _context.Companies
                   .FirstOrDefault(x => x.CompanyId == request.OrganizationId);

                if (company == null)
                {
                    response.Message = "This organization does not exist";
                    return response;
                }
                company.IsGpsOn = request.IsGpsOn;
                company.Threshold = request.Threshold;
                _context.SaveChanges();
                response.Success = true;
            }
            catch (Exception exception)
            {
                response.Message = exception.Message;
            }
            return response;
        }

        public IList<Company> GetOranizationList()
        {
            return _context.Companies.Where(x => !x.IsDeleted).ToList();
        }

        public IList<CompanyViewResponse> GetAllCompanies()
        {
            var query = _context.Companies
                .Include(x => x.ServiceRecords)
                .Include(x => x.Customers)
                .Include(x => x.CompanyWorkers).Where(x => !x.IsDeleted);

            var list = query.Select(x => new CompanyViewResponse
            {
                Id = x.CompanyId,
                Name = x.Name,
                Customers = x.Customers.Count(),
                Services = x.ServiceRecords.Count(),
                Users = x.CompanyWorkers.Count(),
                GatewayCustomerId = x.GatewayCustomerId,
                MerchantAccountId = x.MerchantAccountId,
                PaymentGatewayType =  x.PaymentGatewayType
            });
            return list.ToList();
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
            else
            {
                company.Name = string.Format("{0}_DELETED_{1}", company.Name, DateTime.UtcNow.ToString("O"));
                company.IsDeleted = true;
            }

            _context.SaveChanges();
            response.Success = true;
            return response;
        }

        public BaseResponse IsOrgNameExist(string name)
        {
            var response = new BaseResponse();
            var company = _context.Companies.FirstOrDefault(x => x.Name == name);
            response.Success = company != null;
            return response;

        }

        #endregion

        #region Private Method(s)

        public void AddAddress(Company company, CompanySaveRequest request)
        {
            company.Addresses = new Collection<Address>
            {
                new Address
                {
                    StreetAddress = request.Address,
                    City = request.City,
                    Territory = request.State,
                    Country = request.Country,
                    PostalCode = request.PostalCode
                }
            };
        }

        public void AddContactPerson(Company company, CompanySaveRequest request)
        {
            company.ContactPersons = new Collection<ContactPerson>
            {
                new ContactPerson
                {
                    Email = request.ContactEmail,
                    FirstName = request.ContactFirstName,
                    MiddleName = request.ContactMiddleName,
                    LastName = request.ContactLastName,
                    Telephone = request.ContactTelephone,
                    Mobile = request.ContactMobile,
                    ContactType = ContactTypes.Office.ToString() //TODO : 
                }
            };
        }

        #endregion
    }
}
