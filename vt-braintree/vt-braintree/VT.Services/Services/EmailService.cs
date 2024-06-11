using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
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
    public class EmailService : IEmailService
    {
         #region Field(s)

        private readonly IVerifyTechContext _context;

        #endregion

        #region Constructor

        public EmailService(IVerifyTechContext context)
        {
            _context = context;
        }

        #endregion

        public SendEmailResponse SendEmail(SendEmailRequest request)
        {
            var serviceRecords = new List<ServiceRecord> {request.ServiceRecord};

            return Send(request.PdfTemplate, request.Template, request.ServiceRecord.CustomerId, 
                request.FromEmail, serviceRecords, null, false, request.SetPaidExternal);
        }

        public SendEmailResponse SendEmail(SendCustomerEmailRequest request)
        {
            var serviceRecords = new List<ServiceRecord> { request.ServiceRecord };
            return Send(request.PdfTemplate, request.Template, request.CustomerId, 
                request.FromEmail, serviceRecords, request.SetCreditCardUrl);
        }

        public SendEmailResponse SendEmail(SendMultipleServicesEmailRequest request)
        {
            int customerId = 0;
            
            var serviceRecords =
                _context.ServiceRecords.Include(x => x.ServiceRecordItems)
                    .Where(x => request.ServiceRecords.Contains(x.ServiceRecordId))
                    .ToList();

            foreach (var serviceRecord in serviceRecords)
                customerId = serviceRecord.CustomerId; 
             

            return Send(request.PdfTemplate, request.Template, 
                customerId, request.FromEmail, serviceRecords, null, request.IsBillingFromAdmin, request.SetPaidExternal);
        }


        private SendEmailResponse Send(string pdfTemplate, string emailTemplate,
            int customerId, string fromEmail, List<ServiceRecord> serviceRecords, 
            string url = null, bool isBillingFromAdmin = false, bool isPaidExternally = false)
        {
            var response = new SendEmailResponse();
            
            try
            {
                var customer =
                    _context.Customers.Include(x => x.ContactPersons)
                        .FirstOrDefault(x => x.CustomerId == customerId);

                if (customer == null)
                {
                    response.Message = "Customer does not exist.";
                    return response;
                }

                var company = _context.Companies.FirstOrDefault(x => x.CompanyId == customer.CompanyId);

                if (company == null)
                {
                    response.Message = "Company does not exist.";
                    return response;
                }

                var contact = customer.ContactPersons.FirstOrDefault();

                if (contact == null)
                {
                    response.Message = "Customer contact does not exist.";
                    return response;
                }

                var multipleServices = false;
                string serviceDate = string.Empty; 

                if (!string.IsNullOrEmpty(pdfTemplate))
                {
                    //var subject = GetEmailSubject(status);
                    
                    var hasNonStandard = false;
                    var attachments = new List<Attachment>();
                    var toBeInvoiced = new List<ServiceRecord>();

                    if (serviceRecords.Any())
                    {
                        //if billing is from admin then escape non-standard items from attachment
                        toBeInvoiced.AddRange(isBillingFromAdmin
                            ? serviceRecords.Where(serviceRecord =>serviceRecord.IsInvoiceSent == false)
                            : serviceRecords);


                        if (toBeInvoiced.Count() > 1)
                        {
                            multipleServices = true;
                            var firstOrDefault = toBeInvoiced.FirstOrDefault();
                            var lastOrDefault = toBeInvoiced.LastOrDefault();

                            if (firstOrDefault != null && lastOrDefault != null)
                            {
                                serviceDate = string.Format("between {0} and {1}",
                                    firstOrDefault.EndTime.ToPstTime().ToString("MMMM dd, yyyy"),
                                    lastOrDefault.EndTime.ToPstTime().ToString("MMMM dd, yyyy"));
                            } 
                        }
                        else
                        {
                            var firstOrDefault = toBeInvoiced.FirstOrDefault();
                            if (firstOrDefault != null)
                                serviceDate = string.Format("on {0}", firstOrDefault.EndTime.ToPstTime().ToString("MMMM dd, yyyy"));
                        }

                        //for each service record create email attachment
                        foreach (var serviceRecord in toBeInvoiced.OrderBy(x => x.EndTime))
                        {
                            pdfTemplate = EmailConstants.GeneralPdfTemplate;
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
                                                     "\" target=\"_blank\" style=\"text-decoration: underline\">Photo " + (a + 1) + " (" + photoTime + ") [click here]</a>");

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
                            hasNonStandard = serviceRecord.ServiceRecordItems.Any(x => x.CostOfService == null || x.CostOfService == 0);

                            if (!hasNonStandard && !serviceRecord.InvoiceDate.HasValue)
                            {
                                serviceRecord.InvoiceDate = DateTime.UtcNow;
                                serviceRecord.IsInvoiceSent = hasNonStandard == false;
                            }
                             
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
                                .Replace("{SERVICE-RECORD-ID}", serviceRecord.ServiceRecordId.ToString("D4"))
                                .Replace("{INVOICE-DATE}", serviceRecord.InvoiceDate.HasValue ? serviceRecord.InvoiceDate.Value.ToPstTime().ToString("MMMM dd, yyyy") : string.Empty)
                                .Replace("{SERVICE-RECORD-DATE}", serviceRecord.EndTime.ToPstTime().ToString("MMMM dd, yyyy"))
                                //invoice items
                                .Replace("{INVOICE-ITEMS}", items.ToString())
                                //total, amount-paid & balance
                                .Replace("{TOTAL}", (hasNonStandard ? "TBD" : total.Value.ToString("c")))
                                .Replace("{AMOUNT-PAID}", amountPaid)
                                .Replace("{BALANCE-DUE}", (hasNonStandard ? "TBD" : balance.Value.ToString("c")))
                                //footer message
                                .Replace("{MESSAGE}", message);

                            var attachment = PdfUtil.GetPdfAttachment(pdfTemplate,
                                string.Format("{0}-{1}-{2}.pdf", hasNonStandard ? "WORK-SUMMARY" : "INVOICE", DateTime.UtcNow.ToPstTime().ToString("MMddyyyy"), serviceRecord.ServiceRecordId.ToString(CultureInfo.InvariantCulture)));

                            if (attachment != null)
                                attachments.Add(attachment);

                           
                        }

                        emailTemplate = emailTemplate.Replace("{CUSTOMER-NAME}", customer.Name)
                                .Replace("{TITLE}", hasNonStandard ? "Work Summary" : "Invoice")
                                .Replace("{DATE}", serviceDate)
                                .Replace("{ORGANISATION}", company.Name);

                    }

                    var subject = hasNonStandard
                        ? string.Format("Work Summary for Service {0}",serviceDate)
                        : (multipleServices
                            ? string.Format("Invoices for Service {0}", serviceDate)
                            : string.Format("Invoice for Service {0}", serviceDate));

                    
                    //response.IsInvoiceSent = hasNonStandard == false;

                    var error = EmailUtil.Send(fromEmail, contact.Email, subject, emailTemplate, attachments);
                    response.Success = string.IsNullOrEmpty(error);
                }

                if (!string.IsNullOrEmpty(url))
                {
                    emailTemplate = emailTemplate.Replace("{NAME}", customer.Name).Replace("{URL}", url).Replace("{ORGANISATION}", company.Name);
                    var error = EmailUtil.Send(fromEmail, contact.Email, "Add / Update Credit Card information", emailTemplate, new List<Attachment>());
                    response.Success = string.IsNullOrEmpty(error);
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

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

        private string GetEmailSubject(ServiceRecordStatus serviceRecordStatus)
        {
            var status = string.Empty;

            switch (serviceRecordStatus)
            {
                case ServiceRecordStatus.PaidByCcOnFile:
                    status = "Payment Successful";
                    break;
                case ServiceRecordStatus.UnPaidBillingIdMissingOnly:
                    status = "Customer Credit card information missing";
                    break;
                case ServiceRecordStatus.UnPaidNonStandardServiceOnly:
                    status = "Non-Standard Service";
                    break;
                case ServiceRecordStatus.UnPaidBoth:
                    status = "Customer Credit card information missing & Non-Standard Service";
                    break;
            }
            return status;
        }

        private string GetEmailBody(ServiceRecordStatus status, ContactPerson contactPerson, string template, string url)
        {
            var body = string.Empty;

            switch (status)
            {
                case ServiceRecordStatus.PaidByCcOnFile:
                    var invoiceData = "<tr></tr>";
                    body = string.Format(template, contactPerson.FirstName, contactPerson.LastName, invoiceData);
                    break;
                case ServiceRecordStatus.UnPaidBillingIdMissingOnly:
                     body = string.Format(template, contactPerson.FirstName, contactPerson.LastName, url, DateTime.UtcNow.Year);
                    break;
                case ServiceRecordStatus.UnPaidNonStandardServiceOnly:
                case ServiceRecordStatus.UnPaidBoth:
                    body = string.Format(template, contactPerson.FirstName, contactPerson.LastName, url, DateTime.UtcNow.Year);
                    break;
            }
            return body;
        }
    }
}
