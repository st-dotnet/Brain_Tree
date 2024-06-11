using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Amazon.EC2.Model;

namespace VT.Web.Models
{
    public class CommissionsViewModel
    {
        public DateTime Date { get; set; }
        public int ServiceRecordId { get; set; }
        public decimal Amount { get; set; }
        public string BtTransactionId { get; set; }
        public string Type { get; set; }
        public string CustomerName { get; set; }
        public string CompanyName { get; set; }
    }


    public class CommissionInputModel
    {
        public CommissionInputModel()
        {
            Customers = new List<int>();
        }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? CompanyId { get; set; }
        public List<int> Customers { get; set; }
        public string CompanyName { get; set; }
    }

    public class CommissionsInputModel
    {
        public CommissionsInputModel()
        {
            CompanyId = new List<int>();
        }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; } 
        public List<int> CompanyId { get; set; }
    }

    public class InvoicesViewModel
    {
        public DateTime InvoiceDate { get; set; }
        public int InvoiceNumber { get; set; }
        public string Customer { get; set; }
        public string CustomerEmail { get; set; }
        public decimal Amount { get; set; }
        public string PaymentType { get; set; }
        public DateTime ServiceDate { get; set; }
        public string TransactionId { get; set; }
    }

    public class CommissionExpenseViewModel
    {
        public DateTime Date { get; set; }
        public int CommissionId { get; set; }
        public string InvoiceNumber { get; set; }
        public decimal Amount { get; set; }
        public string TransactionId { get; set; }
        public string WhenCollected { get; set; }
    }

    public class VoidInvoicesViewModel
    {
        public DateTime? InvoiceDate { get; set; }
        public int InvoiceNumber { get; set; }
        public string Customer { get; set; }
        public string CustomerEmail { get; set; }
        public decimal Amount { get; set; }
        public string PaymentType { get; set; }
        public DateTime? VoidedOn { get; set; }
        public string Comments { get; set; } 
    }

    public class VoidInvoiceViewModel
    { 
        public int Id { get; set; }
        public string Organization { get; set; }
        public string Customer { get; set; }
        public string Employee { get; set; } 
        public DateTime? ServiceDate { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string Status { get; set; }
        public int TotalAmount { get; set; }
        public int TransId { get; set; }
        public DateTime? VoidDate { get; set; }
    }
}