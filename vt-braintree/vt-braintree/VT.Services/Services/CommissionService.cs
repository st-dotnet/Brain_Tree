using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq; 
using VT.Common.Utils;
using VT.Data;
using VT.Data.Context;
using VT.Data.Entities;
using VT.Services.DTOs;
using VT.Services.Interfaces;

namespace VT.Services.Services
{
    public class CommissionService : ICommissionService
    {
        #region Field(s)

        private readonly IVerifyTechContext _context;

        #endregion

        #region Constructor

        public CommissionService(IVerifyTechContext context)
        {
            _context = context;
        }

        #endregion

        #region Interface implementation

        public IList<Commission> GetCommissions(int? companyId)
        {
            if (companyId.HasValue)
            {
                var serviceRecordsIds =
                    _context.ServiceRecords.Where(x => x.CompanyId == companyId).Select(x => x.ServiceRecordId);

                return _context.Commissions.Where(x => serviceRecordsIds.Contains(x.ServiceRecordId)).ToList();
            }

            return _context.Commissions.ToList();
        }
        
        public BaseResponse SaveCommission(Commission commission)
        {
            var response = new BaseResponse();

            try
            {
                _context.Commissions.Add(commission);
                _context.SaveChanges();
                response.Success = true;
            }
            catch (Exception exception)
            {
                response.Message = exception.ToString();
            }
            return response;
        }

        public GetInvoiceDetailResponse GetInvoiceDetails(GetInvoiceDetailRequest request)
        {
            var query = _context.ServiceRecords.Where(x => 
                    x.Status == ServiceRecordStatus.PaidByCcOnFile || 
                    x.Status == ServiceRecordStatus.PaidExternal).AsQueryable();

            var startDate = new DateTime(request.StartDate.GetValueOrDefault().Year, 
                request.StartDate.GetValueOrDefault().Month, 
                request.StartDate.GetValueOrDefault().Day,0,0,0);

            var endDate = new DateTime(request.EndDate.GetValueOrDefault().Year,
                request.EndDate.GetValueOrDefault().Month,
                request.EndDate.GetValueOrDefault().Day, 0, 0, 0);

            endDate = endDate.AddDays(1);

            if (request.StartDate != null) query = query.Where(x => x.InvoiceDate >= startDate);
            if (request.EndDate != null) query = query.Where(x => x.InvoiceDate < endDate);
            if (request.CompanyId != null) query = query.Where(x => x.CompanyId == request.CompanyId);
            if (request.Customers != null && request.Customers.Any()) query = query.Where(x => request.Customers.Contains(x.CustomerId)); 

            var response = new GetInvoiceDetailResponse
            {
                Items = query.Include(x => x.Customer).ToList()
                    .Select(x =>
                    {
                        var contactPerson = x.Customer.ContactPersons.FirstOrDefault();
                        return contactPerson != null ? new GetInvoiceDetailItem
                        {
                            Customer = x.Customer.Name,
                            CustomerEmail =
                                             x.Customer.ContactPersons.Any()
                                                 ? contactPerson.Email
                                                 : String.Empty,
                            Amount = x.TotalAmount,
                            InvoiceDate = x.InvoiceDate,
                            TransactionId = x.BtTransactionId,
                            InvoiceNumber = x.ServiceRecordId,
                            PaymentType = x.Status == ServiceRecordStatus.PaidByCcOnFile ? "Credit Card" : "Other",
                            ServiceDate = x.EndTime
                        } : null;
                    }).ToList() 
            };

            return response;
        }

        public GetVoidInvoiceDetailResponse GetVoidInvoiceDetails(GetVoidInvoiceDetailRequest request)
        {
            var query = _context.ServiceRecords.Where(x => x.IsVoid && x.InvoiceDate != null).AsQueryable();

            var startDate = new DateTime(request.StartDate.GetValueOrDefault().Year,
                request.StartDate.GetValueOrDefault().Month,
                request.StartDate.GetValueOrDefault().Day, 0, 0, 0);

            var endDate = new DateTime(request.EndDate.GetValueOrDefault().Year,
                request.EndDate.GetValueOrDefault().Month,
                request.EndDate.GetValueOrDefault().Day, 0, 0, 0);

            endDate = endDate.AddDays(1);

            if (request.StartDate != null) query = query.Where(x => x.VoidTime >= startDate);
            if (request.EndDate != null) query = query.Where(x => x.VoidTime < endDate);
            if (request.CompanyId != null) query = query.Where(x => x.CompanyId == request.CompanyId);
            if (request.Customers != null && request.Customers.Any(x => x != 0)) query = query.Where(x => request.Customers.Contains(x.CustomerId));

            var response = new GetVoidInvoiceDetailResponse
            {
                Items = query.Include(x => x.Customer).ToList()
                    .Select(x =>
                    {
                        var contactPerson = x.Customer.ContactPersons.FirstOrDefault();
                        return contactPerson != null ? new GetVoidInvoiceDetailItem
                        {
                            Customer = x.Customer.Name,
                            CustomerEmail =
                                             x.Customer.ContactPersons.Any()
                                                 ? contactPerson.Email
                                                 : String.Empty,
                            Amount = x.TotalAmount,
                            InvoiceDate = x.InvoiceDate,
                            Comments = x.Status == ServiceRecordStatus.PaidByCcOnFile ? "The customer's credit card	has	been billed, make sure you issue a refund" : "",
                            InvoiceNumber = x.ServiceRecordId,
                            PaymentType = x.Status == ServiceRecordStatus.PaidByCcOnFile ? "Credit Card" : "Other",
                            VoidedOn = x.VoidTime
                        } : null;
                    }).ToList()
            };

            return response;
        }

        public GetCommissionExpenseResponse GetCommissionExpenseDetails(GetCommissionExpenseRequest request)
        {
            var query = _context.Commissions.AsQueryable();

            var startDate = new DateTime(request.StartDate.GetValueOrDefault().Year,
                request.StartDate.GetValueOrDefault().Month,
                request.StartDate.GetValueOrDefault().Day, 0, 0, 0);

            var endDate = new DateTime(request.EndDate.GetValueOrDefault().Year,
                request.EndDate.GetValueOrDefault().Month,
                request.EndDate.GetValueOrDefault().Day, 0, 0, 0);

            endDate = endDate.AddDays(1);

            if (request.StartDate != null) query = query.Where(x => x.Date >= startDate);
            if (request.EndDate != null) query = query.Where(x => x.Date < endDate);
            if (request.CompanyId != null) query = query.Where(x => x.ServiceRecord.CompanyId == request.CompanyId);
            if (request.Customers != null && request.Customers.Any()) query = query.Where(x => request.Customers.Contains(x.ServiceRecord.CustomerId));

            var response = new GetCommissionExpenseResponse
            {
                Items = query.ToList().Select(x => new GetCommissionExpenseDetailItem
                {
                    Amount = x.Amount,
                    Date = x.Date,
                    CommissionId = x.CommissionId,
                    InvoiceNumber = x.ServiceRecordId.ToString(),
                    TransactionId = x.BtTransactionId,
                    WhenCollected = EnumUtil.GetDescription(x.Type)
                }).ToList()
            };

            return response;
        }

        public GetCommissionExpenseResponse GetCommissionsForSuperAdmin(GetCommissionsRequest request)
        {
            var query = _context.Commissions.AsQueryable();

            var startDate = new DateTime(request.StartDate.GetValueOrDefault().Year,
                request.StartDate.GetValueOrDefault().Month,
                request.StartDate.GetValueOrDefault().Day, 0, 0, 0);

            var endDate = new DateTime(request.EndDate.GetValueOrDefault().Year,
                request.EndDate.GetValueOrDefault().Month,
                request.EndDate.GetValueOrDefault().Day, 0, 0, 0);

            endDate = endDate.AddDays(1);

            if (request.StartDate != null) query = query.Where(x => x.Date >= startDate);
            if (request.EndDate != null) query = query.Where(x => x.Date < endDate); 
            if (request.CompanyId != null && request.CompanyId.Any()) query = query.Where(x => request.CompanyId.Contains(x.ServiceRecord.CompanyId));

            var response = new GetCommissionExpenseResponse
            {
                Items = query.ToList().Select(x => new GetCommissionExpenseDetailItem
                {
                    Amount = x.Amount,
                    Date = x.Date,
                    CommissionId = x.CommissionId,
                    InvoiceNumber = x.ServiceRecordId.ToString(),
                    TransactionId = x.BtTransactionId,
                    WhenCollected = EnumUtil.GetDescription(x.Type)
                }).ToList()
            };

            return response;
        }

        #endregion

    }
}
