using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using VT.Common;
using VT.Common.Utils;
using VT.Data;
using VT.Data.Context;
using VT.Data.Entities;
using VT.Services.DTOs;
using VT.Services.Interfaces;

namespace VT.Services.Services
{
    public class ServiceRecordService : IServiceRecordService
    {
        #region Field(s)

        private readonly IVerifyTechContext _context;
        private readonly IPaymentGatewayService _paymentGatewayService;
        private readonly ICommissionService _commissionService;

        #endregion

        #region Constructor

        public ServiceRecordService(IVerifyTechContext context, 
            IPaymentGatewayService paymentGatewayService, ICommissionService commissionService)
        {
            _context = context;
            _paymentGatewayService = paymentGatewayService;
            _commissionService = commissionService;
        }

        #endregion

        #region Interface implementation

        public IList<ServiceRecord> GetServiceRecordsByCompanyWorker(int companyWorkerId)
        {
            return _context.ServiceRecords
                .Include(x => x.ServiceRecordItems)
                .Where(x => x.CompanyWorkerId == companyWorkerId).ToList();
        }

        public IList<ServiceRecord> GetServiceRecordsByCustomer(int customerId)
        {
            return _context.ServiceRecords
                .Include(x => x.ServiceRecordItems)
                .Where(x => x.CustomerId == customerId).ToList();
        }

        public IList<ServiceRecord> GetOutstandingReceivableByCustomerId(int customerId)
        {
            return _context.ServiceRecords
                .Include(x => x.ServiceRecordItems)
                .Where(x => x.CustomerId == customerId
                && !x.IsVoid
                && x.Status != ServiceRecordStatus.PaidByCcOnFile 
                && x.Status != ServiceRecordStatus.PaidExternal).ToList();
        }

        public IList<ServiceRecord> GetAllServiceRecords(int? companyId = null)
        {
            return companyId != null && companyId > 0 ?
                _context.ServiceRecords
                .Include(x => x.CompanyWorker)
                .Include(x => x.Customer)
                .Include(x => x.Company)
                .Include(x => x.ServiceRecordItems)
                .Where(x => x.CompanyId == companyId && !x.IsVoid).ToList()
                : _context.ServiceRecords
                 .Include(x => x.CompanyWorker)
                .Include(x => x.Customer)
                .Include(x => x.Company)
                .Include(x => x.ServiceRecordItems)
                .Where(x => !x.IsVoid).ToList();
        }

        public IList<ServiceRecord> GetVoidServiceRecords(int? companyId = null)
        {
            return companyId != null && companyId > 0 ?
                _context.ServiceRecords.Where(x => x.CompanyId == companyId && x.IsVoid).ToList()
                : _context.ServiceRecords.Where(x => x.IsVoid).ToList();
        }

        public IList<ServiceRecord> GetAllOutstandingReceivables(int? companyId = null)
        {
            var query = _context.ServiceRecords.Include(x => x.Commissions).AsQueryable();

            return companyId != null && companyId > 0 ?
                query.Where(x => x.CompanyId == companyId && x.Commissions.Count == 0).ToList()
                : _context.ServiceRecords.Where(x => x.Commissions.Count == 0).ToList();


            //return companyId != null && companyId > 0 ? 
            //    _context.ServiceRecords.Where(x => x.CompanyId == companyId && !x.BilledToCompany).ToList()
            //    : _context.ServiceRecords.Where(x => !x.BilledToCompany).ToList();
        }

        public BaseResponse SaveServiceRecord(SaveServiceRecordRequest request)
        {
            var response = new BaseResponse();
            try
            {
                var total = request.SaveServiceRecordItems.Sum(x => x.CostOfService);
                var customer = _context.Customers.FirstOrDefault(x => x.CustomerId == request.CustomerId);
                var company = _context.Companies.FirstOrDefault(x => x.CompanyId == request.CompanyId);

                if (customer == null)
                {
                    response.Message = "Customer does not exist.";
                    return response;
                }

                if (company == null)
                {
                    response.Message = "Company does not exist.";
                    return response;
                }

                var serviceRecord = new ServiceRecord
                {
                    BilledToCompany = false,
                    CompanyId = request.CompanyId,
                    Company = company,
                    CustomerId = request.CustomerId,
                    Customer = customer,
                    Description = request.Description,
                    CompanyWorkerId = request.CompanyWorkerId,
                    EndTime = request.EndTime,
                    StartTime = request.StartTime,
                    TotalAmount = total != null ? total.Value : 0,
                    Status = ServiceRecordStatus.NotProcessed,
                    Commissions = new List<Commission>()
                };

                var serviceRecordItems = request.SaveServiceRecordItems.Select(serviceRecordItemRequest => new ServiceRecordItem
                {
                    CompanyServiceId = serviceRecordItemRequest.CompanyServiceId,
                    CustomerServiceId = serviceRecordItemRequest.CustomerServiceId,
                    CostOfService = serviceRecordItemRequest.CostOfService,
                    CustomerId = serviceRecordItemRequest.CustomerId,
                    Description = serviceRecordItemRequest.Description,
                    EndTime = serviceRecordItemRequest.EndTime,
                    StartTime = serviceRecordItemRequest.StartTime,
                    ServiceName = serviceRecordItemRequest.ServiceName,
                    Type = serviceRecordItemRequest.Type,
                    ServiceRecord = serviceRecord,
                    ServiceRecordAttachments = serviceRecordItemRequest.Attachments.Select(x => new ServiceRecordAttachment
                    {
                        Description = x.Description,
                        Date = x.Date,
                        Type = x.Type,
                        Url = x.Url
                    }).ToList()
                }).ToList();

                foreach (var serviceRecordItem in serviceRecordItems)
                {
                    foreach (var attachment in serviceRecordItem.ServiceRecordAttachments)
                    {
                        attachment.ServiceRecordItem = serviceRecordItem;
                    }
                }

                serviceRecord.ServiceRecordItems = serviceRecordItems;
                _context.ServiceRecords.Add(serviceRecord);
                _context.SaveChanges();

                var paymentResponse = _paymentGatewayService.ProcessPayment(new ProcessPaymentRequest
                {
                    ServiceRecord = serviceRecord
                });

                serviceRecord = paymentResponse.ServiceRecord;

                var commission = paymentResponse.Commission;

                if (commission != null)
                {
                    commission.ServiceRecord = serviceRecord;
                    serviceRecord.Commissions.Add(commission);
                }
                _context.SaveChanges();

                response.Success = paymentResponse.Success;
                response.Message = paymentResponse.Message;
            }
            catch (DbEntityValidationException exception)
            {
                response.Message = exception.Message;
            }
            catch (Exception exception)
            {
                response.Message = exception.Message;
            }
            return response;
        }

        public IList<ServiceRecord> GetServiceRecordsByCompanyService(int id)
        {
            return (from s in _context.ServiceRecords
                    join i in _context.ServiceRecordItems on s.ServiceRecordId equals i.ServiceRecordId
                    where i.CompanyServiceId == id
                    group s by s.ServiceRecordId into g
                    select g.FirstOrDefault()).ToList();
        }

        public byte[] GetPdf(int id)
        {
            var pdfTemplate = GetPdfHtml(id);
            var memoryStream = PdfUtil.GetPdfMemoryStream(pdfTemplate);
            return memoryStream;
        }
 
        public BaseResponse VoidServiceActivity(int id)
        {
            var response = new BaseResponse();

            var serviceRecord = _context.ServiceRecords
                .Include(x => x.Company)
                .FirstOrDefault(x => x.ServiceRecordId == id);
            
            if (serviceRecord != null)
            {
                var customer =
                    _context.Customers.Include(x => x.ContactPersons)
                        .FirstOrDefault(x => x.CustomerId == serviceRecord.CustomerId);

                if (customer == null)
                {
                    response.Message = "Customer does not exist.";
                    return response;
                }

                var customerAddress = customer.Addresses.FirstOrDefault();

                if (customerAddress == null)
                {
                    response.Message = "Customer address does not exist.";
                    return response;
                }

                var contact = customer.ContactPersons.FirstOrDefault();

                if (contact == null)
                {
                    response.Message = "Customer contact does not exist.";
                    return response;
                }
                 
                serviceRecord.IsVoid = true;
                serviceRecord.VoidTime = DateTime.UtcNow;
                _context.SaveChanges();

                // send email to customer

                // get pdf html
                var pdfHtml = GetPdfHtml(id);

                // get pdf byte array for attachement
                var bytesArr = PdfUtil.GetPdfMemoryStream(pdfHtml);

                var hasNonStandard = serviceRecord.ServiceRecordItems.Any(x => x.CostOfService == null || x.CostOfService == 0);

                // structured name for pdf
                var name = string.Format("{0}-{1}-{2}.pdf", hasNonStandard ? "VOIDED WORK SUMMARY" : "VOIDED INVOICE", DateTime.UtcNow.ToPstTime().ToString("MMddyyyy"),
                    serviceRecord.ServiceRecordId.ToString(CultureInfo.InvariantCulture));

                // attachments
                var attachments = new List<Attachment> { new Attachment(new MemoryStream(bytesArr), name) }; 

                var message = string.Empty;


                if (serviceRecord.Status == ServiceRecordStatus.UnPaidPaymentFailed)
                {
                    message = @"Your credit card was not charged for this amount when the work was completed. No action is required on your part. This courtesy email is for your information.";
                }
                else if (serviceRecord.Status == ServiceRecordStatus.PaidByCcOnFile)
                {
                    message = string.Format("Your credit card has been billed for {0} previously on the invoice date above. " +
                                            "This money will be refunded to you via check and mailed to: <br/> <br/>" +
                                             "{1}<br/>{2}, {3} {4}", serviceRecord.TotalAmount.ToString("C"),
                                             customerAddress.StreetAddress, customerAddress.City, customerAddress.Territory, 
                                             customerAddress.PostalCode); 
                }
                else if (serviceRecord.Status == ServiceRecordStatus.PaidExternal)
                {
                    message =
                        string.Format(
                            "You were sent an invoice via email previously to pay this amount. " +
                            "If you <b> have not issued payment </b> yet, please ignore the past " +
                            "email and <b> do not send </b> any payment. If you have already sent payment " +
                            "please contact {0} and let us know so we can make sure you are not charged or refunded expeditiously.",
                            serviceRecord.Company.Name); 
                }
                else if (hasNonStandard)
                {
                    message = "The work summary that was previously sent has not been billed to you in any way. " +
                              "This is a courtesy email to let you know that the previous email may be discarded.";
                }

                var emailTemplate = EmailConstants.VoidEmailTemplate
                    .Replace("{CUSTOMER-NAME}", string.Format("{0} {1}", contact.FirstName, contact.LastName))
                    .Replace("{TITLE}", hasNonStandard ? "a Work Summary" : "an Invoice")
                    .Replace("{INVOICE-DATE}", 
                        serviceRecord.InvoiceDate != null ? 
                        string.Format("<b>Invoice Date:</b> {0} <br/>", serviceRecord.InvoiceDate.Value.ToPstTime().ToString("MMMM dd, yyyy")): 
                        string.Empty)
                    .Replace("{SERVICE-DATE}", string.Format("<b>Service Date:</b> {0} <br/>", serviceRecord.EndTime.ToPstTime().ToString("MMMM dd, yyyy")))
                    .Replace("{VOID-DATE}", serviceRecord.VoidTime != null ?
                        string.Format("<b>Void Date:</b> {0} <br/>", serviceRecord.VoidTime.Value.ToPstTime().ToString("MMMM dd, yyyy"))
                        : string.Empty)
                    .Replace("{MESSAGE}",message)
                    .Replace("{COMPANY-NAME}", serviceRecord.Company.Name);


                var subject = hasNonStandard ? "Work Summary voided" : "Invoice voided";

                var error = EmailUtil.Send(ApplicationSettings.FromEmail, contact.Email, subject, emailTemplate, attachments);

                if (!string.IsNullOrEmpty(error))
                {
                    response.Message = string.Format("Invoice has been voided but there was an error sending in email. Error: {0}", error); 
                }
                else
                {
                    response.Success = true;
                } 
            }
            else
            {
                response.Message = "Service record does not exist.";
            }
            return response;
        }

        #endregion

        #region Private Methods

        private string GetAddressString(Address address)
        {
            if (address == null) return string.Empty;

            var adr = new StringBuilder();
            adr.Append(address.StreetAddress + "<br/>");
            adr.Append(address.City + "," + " ");
            adr.Append(address.Territory + " ");
            adr.Append(address.PostalCode);

            return adr.ToString();
        }

        private string GetPdfHtml(int id)
        {
            var serviceRecord = _context.ServiceRecords
                .Include(x => x.ServiceRecordItems)
                .FirstOrDefault(x => x.ServiceRecordId == id);

            if (serviceRecord == null) return null;

            var isPaidExternally = serviceRecord.Status == ServiceRecordStatus.PaidExternal;

            var company = serviceRecord.Company;
            var customer = serviceRecord.Customer;

            var pdfTemplate = serviceRecord.IsVoid ? EmailConstants.VoidPdfTemplate : EmailConstants.GeneralPdfTemplate;
            var items = new StringBuilder();
            double? total = 0;
            double? balance = 0;

            foreach (var serviceRecordItem in serviceRecord.ServiceRecordItems)
            {
                items.Append("<tr>");
                items.Append("<td>" + serviceRecordItem.Description + "</td>");
                items.Append("<td style=\"text-align: center;\">");

                if (serviceRecordItem.ServiceRecordAttachments.Any())
                {
                    var attachs = serviceRecordItem.ServiceRecordAttachments.ToArray();
                    for (var a = 0; a < attachs.Length; a++)
                    {
                        string photoLink = attachs[a].Url;
                        string photoTime;
                        try
                        {
                            photoTime = attachs[a].Date.ToPstTime().ToString("MM/dd/yyyy hh:mm:ss tt");
                        }
                        catch (Exception exception)
                        {
                            photoTime = "View";
                        }

                        if (!string.IsNullOrEmpty(photoLink))
                        {
                            items.Append("<a href=\"" + photoLink +
                                     "\" target=\"_blank\" style=\"text-decoration: underline\">Photo" + (a + 1) + " (" + photoTime + ") [click here]</a>");

                            if (a < attachs.Length - 1 && attachs.Length > 1)
                            {
                                items.Append("<br /><br />");
                            }
                        }
                    }
                }

                items.Append("</td>");
                items.Append("<td><span>" + (serviceRecordItem.CostOfService != null && serviceRecordItem.CostOfService != 0 ? serviceRecordItem.CostOfService.Value.ToString("c") : "TBD") + "</span></td>");
                items.Append("</tr>");

                if (serviceRecordItem.CostOfService > 0)
                    total += serviceRecordItem.CostOfService;
            }
            var hasNonStandard = serviceRecord.ServiceRecordItems.Any(x => x.CostOfService == null || x.CostOfService == 0);

            var companyAddress = company.Addresses.FirstOrDefault() ?? new Address();
            var companyContact = company.ContactPersons.FirstOrDefault() ?? new ContactPerson();

            var companyPhones = companyContact.Telephone +
                                (!string.IsNullOrEmpty(companyContact.Mobile)
                                    ? string.Format("{0}{1}", (!string.IsNullOrEmpty(companyContact.Telephone) ? " / " : string.Empty), companyContact.Mobile)
                                    : string.Empty);

            var customerAddress = customer.Addresses.FirstOrDefault() ?? new Address();
            var customerContact = customer.ContactPersons.FirstOrDefault() ?? new ContactPerson();

            var customerPhones = customerContact.Telephone +
                                (!string.IsNullOrEmpty(customerContact.Mobile)
                                ? string.Format("{0}{1}", (!string.IsNullOrEmpty(customerContact.Telephone) ? " / " : string.Empty),
                                customerContact.Mobile) : string.Empty);

            var message = string.Empty;

            if (serviceRecord.Status == ServiceRecordStatus.PaidByCcOnFile)
            {
                message =
                    "Your credit card on file has already been charged. DO NOT send additional payment";
                balance = 0;
            }
            else if (serviceRecord.Status == ServiceRecordStatus.UnPaidPaymentFailed)
            {
                message =
                    "Your credit card on file was not charged successfully. DO NOT remit any payment. We will try to process your credit card at a later time.";
                balance = total;
            }
            else if (serviceRecord.Status == ServiceRecordStatus.UnPaidBillingIdMissingOnly ||
                     serviceRecord.Status == ServiceRecordStatus.UnPaidBoth ||
                     serviceRecord.Status == ServiceRecordStatus.UnPaidNonStandardServiceOnly ||
                     isPaidExternally)
            {
                message =
                    "Please remit payment for the Balance Due upon receipt.";
                balance = total;
            }

            if (hasNonStandard)
            {
                message =
                    "This is a summary of services performed and NOT an Invoice. An Invoice will be sent to you soon once the actual cost of services is determined";
            }

            string amountPaid = serviceRecord.Status == ServiceRecordStatus.UnPaidPaymentFailed || hasNonStandard || !customer.IsCcActive ? 0.ToString("c") : total.Value.ToString("c");

            if (isPaidExternally)
                amountPaid = 0.ToString("c");

            pdfTemplate = pdfTemplate
                .Replace("{TITLE}", hasNonStandard ? "WORK SUMMARY" : "INVOICE")
                //company detail
                .Replace("{COMPANY-NAME}", company.Name)
                .Replace("{COMANPY-ADDRESS}", GetAddressString(companyAddress))
                .Replace("{COMPANY-TELEPHONE}", companyPhones)
                .Replace("{COMPANY-EMAIL}", companyContact.Email)
                //customer detail
                .Replace("{CUSTOMER-NAME}", customer.Name)
                .Replace("{CUSTOMER-ADDRESS}", GetAddressString(customerAddress))
                .Replace("{CUSTOMER-TELEPHONE}", customerPhones)
                .Replace("{CUSTOMER-EMAIL}", customerContact.Email)
                //service record detail
                .Replace("{VOID-TIME}", serviceRecord.VoidTime.HasValue ? serviceRecord.VoidTime.Value.ToPstTime().ToString("MMMM dd, yyyy") :
                string.Empty)
                .Replace("{SERVICE-RECORD-ID}", serviceRecord.ServiceRecordId.ToString("D4"))
                .Replace("{INVOICE-DATE}", serviceRecord.InvoiceDate.HasValue ? serviceRecord.InvoiceDate.Value.ToPstTime().ToString("MMMM dd, yyyy") :
                string.Empty)
                .Replace("{SERVICE-RECORD-DATE}", serviceRecord.EndTime.ToPstTime().ToString("MMMM dd, yyyy"))
                //invoice items
                .Replace("{INVOICE-ITEMS}", items.ToString())
                //total, amount-paid & balance
                .Replace("{TOTAL}", (hasNonStandard ? "TBD" : total.Value.ToString("c")))
                .Replace("{AMOUNT-PAID}", amountPaid)
                .Replace("{BALANCE-DUE}", (hasNonStandard ? "TBD" : balance.Value.ToString("c")))
                //footer message
                .Replace("{MESSAGE}", message);

            return pdfTemplate;
        }

        #endregion 
    }
}
