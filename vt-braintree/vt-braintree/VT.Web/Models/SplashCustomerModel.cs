using VT.Data;

namespace VT.Web.Models
{
    public class SplashCustomerModel
    { 
        public int CompanyId { get; set; }
        public int CustomerId { get; set; }
        public bool IsEditMode { get; set; }
        public bool IsCustomerMode { get; set; }
        public string RedirectUrl { get; set; }

        public string CustomerLoginId { get; set; } 
        public string CustomerFirstName { get; set; } 
        public string CustomerLastName { get; set; } 
        public string CustomerEmail { get; set; } 
        public string CustomerInactive { get; set; } 
        public string CustomerAddress1 { get; set; } 
        public string CustomerCity { get; set; } 
        public string CustomerState { get; set; } 
        public string CustomerCountry { get; set; } 
        public string CustomerZip { get; set; } 
        public string CustomerPhone { get; set; } 
        public string CustomerFax { get; set; }  

        public string PaymentNumber { get; set; } 
        public string PaymentCvv { get; set; } 
        public string PaymentExpiration { get; set; } 
        public string PaymentInactive { get; set; }    

        public string PaymentMethod { get; set; }
    }


    public class SplashPaymentDetailModel
    {
        public int CompanyId { get; set; }

        public string PaymentNumber { get; set; }
        public string PaymentCvv { get; set; }
        public string PaymentExpiration { get; set; }
        public string PaymentInactive { get; set; }
    }
}