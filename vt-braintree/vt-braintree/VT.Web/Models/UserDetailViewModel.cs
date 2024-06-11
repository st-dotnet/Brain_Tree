using System;
using System.Collections.Generic;
using EO.Internal;
using VT.Data;

namespace VT.Web.Models
{
    public class UserDetailViewModel
    {
        public int CompanyWorkerId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int? CompanyId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsDeleted { get; set; }

        public string CompanyName { get; set; }
    }

    public class ServiceRecordDetail
    {
        public int ServiceRecordId { get; set; }
        public int CustomerId { get; set; }
        public int CompanyId { get; set; }
        public decimal ServiceFeePercentage { get; set; }
        public string CompanyWorkerEmail { get; set; }
        public string Description { get; set; }
        public double TotalAmount { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string CustomerName { get; set; }
        public string CompanyName { get; set; }
        public bool BilledToCompany { get; set; }
        public bool HasNonService { get; set; }
        public ServiceRecordStatus RecordStatus { get; set; }
        public string Status { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string TransactionId { get; set; }
        public DateTime? VoidTime { get; set; }
       
        
        public string StartDate
        {
            get { return this.StartTime != null ? this.StartTime.Value.ToString("g") : string.Empty; }
        }

        public string EndDate
        {
            get { return this.EndTime.ToString("g"); }
        }

        public string TotalAmountFormat
        {
            get { return string.Format("{0:C}", this.TotalAmount); }
        }
        
        public bool ShowCheckbox
        {
            get { return !this.BilledToCompany && !this.HasNonService; }
        }

        public string BilledToCompanyCss
        {
            get { return this.BilledToCompany ? "primary" : "warning"; }
        }

        public string BilledToCompanyText
        {
            get { return this.BilledToCompany ? "Yes" : "No"; }
        }

        public string HasNonServiceCss
        {
            get { return this.HasNonService ? "primary" : "warning"; }
        }

        public string HasNonServiceText
        {
            get { return this.HasNonService ? "Yes" : "No"; }
        }
    }

    public class CustomerServiceRecordDetail
    {
        public int ServiceRecordId { get; set; }
        public int CustomerId { get; set; }
        public int CompanyId { get; set; }
        public decimal ServiceFeePercentage { get; set; }
        public string CompanyWorkerEmail { get; set; }
        public string Description { get; set; }
        public double TotalAmount { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string CustomerName { get; set; }
        public string CompanyName { get; set; }
        public bool HasCustomerCc { get; set; }
        public bool HasNonService { get; set; }
        public ServiceRecordStatus RecordStatus { get; set; }
        public string Status { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string TransactionId { get; set; }

        public string InvoiceDateText
        {
            get { return this.InvoiceDate != null ? this.InvoiceDate.Value.ToString("g") : string.Empty; }
        }

        public string StartDate
        {
            get { return this.StartTime != null ? this.StartTime.Value.ToString("g") : string.Empty; }
        }

        public string EndDate
        {
            get { return this.EndTime.ToString("g"); }
        }

        public string TotalAmountFormat
        {
            get { return string.Format("{0:C}", this.TotalAmount); }
        }

        public bool ShowCheckbox
        {
            get { return !this.HasNonService && 
                this.RecordStatus != ServiceRecordStatus.PaidByCcOnFile && 
                this.RecordStatus != ServiceRecordStatus.PaidExternal; }
        }
        
        public string HasNonServiceCss
        {
            get { return this.HasNonService ? "primary" : "warning"; }
        }

        public string HasNonServiceText
        {
            get { return this.HasNonService ? "Yes" : "No"; }
        }
    }

    public class ServiceRecordItemDetail
    {
        public int ServiceRecordItemId { get; set; }
        public int ServiceRecordId { get; set; }
        public int CompanyServiceId { get; set; }
        public int CustomerId { get; set; }
        public string ServiceName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public Double? CostOfService { get; set; }


        public string StartDate
        {
            get { return this.StartTime.ToString("MM/dd/yyyy HH:mm"); }
        }

        public string EndDate
        {
            get { return this.EndTime.ToString("MM/dd/yyyy HH:mm"); }
        }

        public string TotalAmountFormat
        {
            get { return string.Format("{0:C}", this.CostOfService); }
        }
    }
}