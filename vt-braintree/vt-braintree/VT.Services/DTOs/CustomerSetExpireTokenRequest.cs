namespace VT.Services.DTOs
{
    public class CustomerSetExpireTokenRequest
    {
        public int CustomerId { get; set; }
        public string Token { get; set; }
    }
}
