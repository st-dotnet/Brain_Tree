using System.Collections.Generic;

namespace VT.Services.DTOs.SplashPayments
{
    public class GetEntityResponse : BaseResponse
    {
        public EntityResponse response { get; set; }
    }

    public class EntityDatum
    {
        public string id { get; set; }
        public string created { get; set; }
        public string modified { get; set; }
        public string creator { get; set; }
        public string modifier { get; set; }
        public string ipCreated { get; set; }
        public string ipModified { get; set; }
        public object clientIp { get; set; }
        public string login { get; set; }
        public object parameter { get; set; }
        public int type { get; set; }
        public string name { get; set; }
        public string address1 { get; set; }
        public object address2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
        public string country { get; set; }
        public string phone { get; set; }
        public object fax { get; set; }
        public string email { get; set; }
        public string website { get; set; }
        public string ein { get; set; }
        public string currency { get; set; }
        public string custom { get; set; }
        public int inactive { get; set; }
        public int frozen { get; set; }
    }

    public class EntityTotals
    {
        public int count { get; set; }
    }

    public class EntityPage
    {
        public int current { get; set; }
        public int last { get; set; }
    }

    public class EntityDetails
    {
        public int requestId { get; set; }
        public Totals totals { get; set; }
        public Page page { get; set; }
    }

    public class EntityResponse
    {
        public List<EntityDatum> data { get; set; }
        public EntityDetails details { get; set; }
        public List<object> errors { get; set; }
    }
}
