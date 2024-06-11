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
    public class CustomerService : ICustomerService
    {
        #region Field(s)

        private readonly IVerifyTechContext _context;

        #endregion

        #region Constructor

        public CustomerService(IVerifyTechContext context)
        {
            _context = context;
        }

        #endregion

        #region Interface implementation

        public IList<Customer> GetAllCustomers(int? companyId)
        {
            var query = _context.Customers.Where(x => !x.IsDeleted);
            return companyId == null
                ? query.ToList()
                : query.Where(x => x.CompanyId == companyId).ToList();
        }

        public IList<Customer> GetAllCustomersForUser(int userId)
        {
            var customers = new List<Customer>();
            var user = _context.CompanyWorkers.FirstOrDefault(x => x.CompanyWorkerId == userId);

            if (user != null && user.CompanyId != null)
            {
                customers = _context.Customers.Where(x => x.CompanyId == user.CompanyId && !x.IsDeleted).ToList();
            }

            return customers;
        }

        public IList<Customer> GetUserCustomers(int companyWorkerId)
        {
            var customers =
                _context.CompanyWorkerCustomers.Include(x => x.Customer).Where(x => x.CompanyWorkerId == companyWorkerId)
                    .OrderBy(x => x.CustomerOrder)
                    .Select(x => x.Customer)
                    .ToList(); 

            return customers;
        }

        public Customer GetCustomer(int customerId)
        {
            return _context.Customers.FirstOrDefault(x => x.CustomerId == customerId);
        }

        public Customer GetCustomer(string token)
        {
            return _context.Customers.FirstOrDefault(x => x.Token == token);
        }


        public CustomerSaveResponse SaveCustomer(CustomerSaveRequest request)
        {
            var response = new CustomerSaveResponse();
            Customer customer = null;
            if (request.CustomerId > 0) //Edit
            {
                customer = _context.Customers
                    .Include(x => x.Addresses)
                    .Include(x => x.ContactPersons)
                    .FirstOrDefault(x => x.CustomerId == request.CustomerId);

                if (customer == null)
                {
                    response.Message = "This customer does not exist";
                    return response;
                }

                customer.CompanyId = request.CompanyId;
                customer.Name = request.Name; 

                var address = customer.Addresses.FirstOrDefault();
                var contactPerson = customer.ContactPersons.FirstOrDefault();

                //Address
                if (address == null)
                {
                    AddAddress(customer, request);
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
                    AddContactPerson(customer, request);
                }
                else
                {
                    contactPerson.Email = request.ContactEmail;
                    contactPerson.FirstName = request.ContactFirstName;
                    contactPerson.LastName = request.ContactLastName;
                    contactPerson.Telephone = request.ContactTelephone;
                    contactPerson.Mobile = request.ContactMobile;
                    contactPerson.MiddleName = request.ContactMiddleName;
                } 
            }
            else //Add
            {
                customer = new Customer
                {
                    Name = request.Name,
                    CompanyId = request.CompanyId,
                };
                AddAddress(customer, request);
                AddContactPerson(customer, request);
                _context.Customers.Add(customer);
            }

            customer.IsCcActive = request.IsActive;
            _context.SaveChanges();
            response.Customer = customer;
            response.Success = true;
            return response;
        }

        public BaseResponse DeleteCustomers(List<int> ids)
        {
            var response = new BaseResponse();
            try
            {
                var customers = _context.Customers.Where(x => ids.Contains(x.CustomerId)).ToList();

                foreach (var customer in customers)
                {
                    customer.IsDeleted = true;
                    customer.Name = string.Format("{0}_DELETED_{1}", customer.Name, DateTime.UtcNow.ToString("O"));
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

        public BaseResponse SetExpireTokenForCustomer(CustomerSetExpireTokenRequest request)
        {
            var response = new BaseResponse();

            var customer = _context.Customers
                    .FirstOrDefault(x => x.CustomerId == request.CustomerId);

            if (customer == null)
            {
                response.Message = "This customer does not exist";
                return response;
            }

            customer.Token = request.Token;
            customer.ExpireAt = DateTime.UtcNow.AddDays(1);

            _context.SaveChanges();
            response.Success = true;

            return response;
        }

        public IList<string> GetCustomerContactEmails()
        {
            return _context.ContactPersons.Where(x => x.Customers.All(y => !y.IsDeleted)).Select(x => x.Email).ToList();
        }

        public IList<string> GetCustomerNames()
        {
            return _context.Customers.Where(x => !x.IsDeleted).Select(x => x.Name).ToList();
        }

        public BaseResponse BatchSave(CustomerBatchRequest request)
        {
            var response = new BaseResponse();

            var customersToBeImported = request.ImportCustomers;

            using (var dbContextTransaction = _context.Db.BeginTransaction())
            {
                try
                {
                    foreach (var dataItem in customersToBeImported.Where(x => x.Status == ExportStatus.Valid.ToString()))
                    {
                        var customerSaveRequest = new CustomerSaveRequest
                        {
                            CompanyId = request.CompanyId,
                            Name = dataItem.CustomerName,

                            //Contact
                            ContactFirstName = dataItem.FirstName,
                            ContactMiddleName = dataItem.MiddleName,
                            ContactLastName = dataItem.LastName,
                            ContactEmail = dataItem.Email,
                            ContactMobile = dataItem.Mobile,
                            ContactTelephone = dataItem.Telephone,

                            //Address
                            Address = dataItem.Address,
                            City = dataItem.City,
                            State = dataItem.State,
                            PostalCode = dataItem.PostalCode,
                            Country = dataItem.Country
                        };

                        var customer = new Customer
                        {
                            Name = customerSaveRequest.Name,
                            CompanyId = request.CompanyId,
                        };
                        AddAddress(customer, customerSaveRequest);
                        AddContactPerson(customer, customerSaveRequest);
                        _context.Customers.Add(customer);
                    }

                    _context.SaveChanges();
                    dbContextTransaction.Commit();
                    response.Success = true;
                }
                catch (Exception exception)
                {
                    dbContextTransaction.Rollback();
                    response.Message = exception.ToString();
                }
            }
            return response;
        }

        public BaseResponse UserCustomerAccess(UserCustomerAccessRequest request)
        {
            var response = new BaseResponse();

            

            try
            {
                // existing customer access
                var existingCustomerAccess = _context.CompanyWorkerCustomers.Where(x => x.CompanyWorkerId == request.UserId);

                // remove existing access if any
                foreach (var access in existingCustomerAccess)
                {
                    _context.CompanyWorkerCustomers.Remove(access);
                }

                var count = 0;
                
                if (request.Customers != null)
                {
                    // add new customer access
                    foreach (var customerId in request.Customers)
                    {
                        _context.CompanyWorkerCustomers.Add(new CompanyWorkerCustomer
                        {
                            CompanyWorkerId = request.UserId,
                            CustomerId = customerId,
                            CustomerOrder = count++
                        });
                    }
                }

                // save all changes
                _context.SaveChanges();

                response.Success = true;
            }
            catch (Exception exception)
            {
                response.Message = exception.Message;
            }
            return response;
        }

        public List<CompanyWorkerCustomer> GetUserCustomerAccess(int companyWorkerId)
        {
           var accessList =  _context.CompanyWorkerCustomers
                .Include(x=> x.Customer)
                .Where(x => x.CompanyWorkerId == companyWorkerId)
                .OrderBy(x => x.CustomerOrder).ToList(); 

            return accessList;
        }

        public CustomerDetailResponse GetCustomerDetail(int customerId)
        {
            CustomerDetailResponse response = null;
            var customer = _context.Customers
               .Include(x => x.Addresses)
               .Include(x => x.ContactPersons)
               .FirstOrDefault(x => x.CustomerId == customerId);

            if (customer != null)
            {
                var address = customer.Addresses.FirstOrDefault();
                var contactPerson = customer.ContactPersons.FirstOrDefault();

                response = new CustomerDetailResponse
                {
                    CustomerId = customer.CustomerId,
                    CompanyId = customer.CompanyId,
                    Name = customer.Name,
                    GatewayCustomerId = customer.GatewayCustomerId,
                    Address = address != null ? address.StreetAddress : string.Empty,
                    City = address != null ? address.City : string.Empty,
                    State = address != null ? address.Territory : string.Empty,
                    PostalCode = address != null ? address.PostalCode : string.Empty,
                    Country = address != null ? address.Country : string.Empty,
                    IsDeleted = customer.IsDeleted,
                    IsActive = customer.IsCcActive,
                    ContactFirstName = contactPerson != null ? contactPerson.FirstName : string.Empty,
                    ContactLastName = contactPerson != null ? contactPerson.LastName : string.Empty,
                    ContactEmail = contactPerson != null ? contactPerson.Email : string.Empty,
                    ContactTelephone = contactPerson != null ? contactPerson.Telephone : string.Empty,
                    ContactMobile = contactPerson != null ? contactPerson.Mobile : string.Empty,
                    ContactMiddleName = contactPerson != null ? contactPerson.MiddleName : string.Empty,
                    Success = true
                };
            }
            else
            {
                response = new CustomerDetailResponse
                {
                    Message = "Customer does not exist in the database."
                };
            }
            return response;
        }

        #endregion

        #region Private Method(s)

        public void AddAddress(Customer customer, CustomerSaveRequest request)
        {
            customer.Addresses = new Collection<Address>
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

        public void AddContactPerson(Customer customer, CustomerSaveRequest request)
        {
            customer.ContactPersons = new Collection<ContactPerson>
            {
                new ContactPerson
                {
                    Email = request.ContactEmail,
                    FirstName = request.ContactFirstName,
                    LastName = request.ContactLastName,
                    Telephone = request.ContactTelephone,
                    Mobile = request.ContactMobile,
                    MiddleName = request.ContactMiddleName,
                    ContactType = ContactTypes.Office.ToString() //TODO : 
                }
            };
        }

        #endregion
    }
}
