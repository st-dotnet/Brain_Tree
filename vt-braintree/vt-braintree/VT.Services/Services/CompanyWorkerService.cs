using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using VT.Common.Utils;
using VT.Data.Context;
using VT.Data.Entities;
using VT.Services.DTOs;
using VT.Services.Interfaces;

namespace VT.Services.Services
{
    public class CompanyWorkerService : ICompanyWorkerService
    {
        #region Field(s)

        private readonly IVerifyTechContext _context;

        #endregion

        #region Constructor

        public CompanyWorkerService(IVerifyTechContext context)
        {
            _context = context;
        }

        #endregion

        #region Interface implementation

        //Login
        public LoginResponse AuthenticateUser(LoginRequest request)
        {
            var response = new LoginResponse();

            var companyWorker = _context.CompanyWorkers
                .Include(x => x.Company)
                .FirstOrDefault(x => x.Email == request.Email);

            if (companyWorker == null)
            {
                response.Message = "User does not exist in the system.";
            }
            else
            {
                response.Success = true;
                response.CompanyWorker = companyWorker;
            }

            return response;
        }

        public IList<CompanyWorker> GetAllUsers(int? companyId)
        {
            var query = _context.CompanyWorkers.Where(x => !x.IsDeleted);
            return (companyId == null) ? query.ToList() : query.Where(x => x.CompanyId == companyId).ToList();
        }

        public BaseResponse DeleteUsers(List<int> ids)
        {
            var response = new BaseResponse();
            try
            {
                var companyWorkers = _context.CompanyWorkers.Where(x => ids.Contains(x.CompanyWorkerId)).ToList();

                foreach (var companyWorker in companyWorkers)
                {
                    companyWorker.IsDeleted = true;
                    companyWorker.Email = EmailDeleted(companyWorker.Email);
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

        public CompanyWorkerResponse GetCompanyWorker(int companyWorkerId)
        {
            var user = _context.CompanyWorkers.Where(x => !x.IsDeleted)
                    .FirstOrDefault(x => x.CompanyWorkerId == companyWorkerId);

            var customers =
                _context.CompanyWorkerCustomers.Where(x => x.CompanyWorkerId == companyWorkerId)
                    .OrderBy(x => x.CustomerOrder)
                    .Select(x => x.CustomerId)
                    .ToList();

            if (user == null)
            {
                throw new Exception("User does not exist in the database");
            }

            return new CompanyWorkerResponse
            {
                CompanyId = user.CompanyId,
                CompanyWorkerId = user.CompanyWorkerId,
                CompanyName = user.Company != null ? user.Company.Name : "Super Admin User",
                Username = user.Email,
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                IsAdmin = user.IsAdmin,
                LastName = user.LastName,
                IsDeleted = user.IsDeleted,
                AuthKey = user.HashedPassword,
                To = customers
            };
        }

        public BaseResponse IsEmailAlreadyExist(string email)
        {
            var response = new BaseResponse();
            var user = _context.CompanyWorkers.FirstOrDefault(x => x.Email == email);
            response.Success = user != null;
            return response;
        }

        public BaseResponse ResetUserPassword(ResetPasswordRequest request)
        {
            var response = new BaseResponse();
            var companyWorker = _context.CompanyWorkers.FirstOrDefault(x => x.CompanyWorkerId == request.UserId);

            if (companyWorker == null)
            {
                response.Message = "User record does not exist in the database.";
                return response;
            }

            companyWorker.PasswordSalt = PasswordUtil.GenerateSalt();
            companyWorker.HashedPassword = PasswordUtil.CreatedHashedPassword(request.NewPassword, companyWorker.PasswordSalt);
            _context.SaveChanges();

            response.Success = true;
            return response;
        }

        public List<string> GetAllActiveUsersEmail()
        {
            return _context.CompanyWorkers.Where(x => !x.IsDeleted).Select(x => x.Email).ToList(); 
        }

        public CompanyWorkerSaveResponse AddUser(CompanyWorkerSaveRequest request)
        {
            var resposne = new CompanyWorkerSaveResponse();
            try
            {
                var companyWorker = request.CompanyWorkerId > 0
                ? _context.CompanyWorkers.Include(x => x.AccessibleCustomers).FirstOrDefault(x => x.CompanyWorkerId == request.CompanyWorkerId)
                : new CompanyWorker();

                if (companyWorker == null)
                {
                    resposne.Message = "User record does not exist in the database.";
                    return resposne;
                }

                companyWorker.Email = request.Email;
                companyWorker.CompanyId = request.CompanyId;
                companyWorker.FirstName = request.FirstName;
                companyWorker.MiddleName = request.MiddleName;
                companyWorker.LastName = request.LastName;
                companyWorker.IsAdmin = request.IsAdmin;

                if (companyWorker.CompanyWorkerId == 0)
                {
                    companyWorker.PasswordSalt = PasswordUtil.GenerateSalt();
                    companyWorker.HashedPassword = PasswordUtil.CreatedHashedPassword(request.Password,
                        companyWorker.PasswordSalt);

                    _context.CompanyWorkers.Add(companyWorker);
                }
                else
                {
                    if (!string.IsNullOrEmpty(request.Password))
                    {
                        var hashedPassword = PasswordUtil.CreatedHashedPassword(request.Password,
                          companyWorker.PasswordSalt);

                        // ReSharper disable once RedundantCheckBeforeAssignment
                        if (companyWorker.HashedPassword != hashedPassword) //update the password
                        {
                            companyWorker.HashedPassword = hashedPassword;
                        }
                    }

                }
                if (companyWorker.AccessibleCustomers != null)
                {
                    // remove existing access if any
                    foreach (var access in companyWorker.AccessibleCustomers.ToList())
                    {
                        _context.CompanyWorkerCustomers.Remove(access);
                    }
                }
                var count = 0;

                // add new customer access
                if (request.AccessibleCustomers != null)
                {
                    foreach (var customerId in request.AccessibleCustomers)
                    {
                        _context.CompanyWorkerCustomers.Add(new CompanyWorkerCustomer
                        {
                            CompanyWorkerId = companyWorker.CompanyWorkerId,
                            CustomerId = customerId,
                            CustomerOrder = count++
                        });
                    }
                }
                

                _context.SaveChanges();
                resposne.Success = true;
                resposne.CompanyWorker = companyWorker;
            }
            catch (Exception exception)
            {
                resposne.Message = exception.ToString();
            }

            return resposne;
        }

        #endregion

        #region Private Method(s)

        private string EmailDeleted(string email)
        {
            var arr = email.Split('@');
            return arr.Length == 2 ? string.Format("{0}_deleted_{1}@{2}", arr[0], DateTime.UtcNow.ToString("MMddyyyyHHmmss"), arr[1]) : string.Empty;
        }

        #endregion



    }
}
