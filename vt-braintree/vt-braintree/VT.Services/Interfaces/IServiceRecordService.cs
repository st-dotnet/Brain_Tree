using System.Collections.Generic; 
using VT.Data.Entities;
using VT.Services.DTOs;

namespace VT.Services.Interfaces
{
    public interface IServiceRecordService
    {
        IList<ServiceRecord> GetServiceRecordsByCompanyWorker(int id);
        IList<ServiceRecord> GetServiceRecordsByCompanyService(int id);
        IList<ServiceRecord> GetServiceRecordsByCustomer(int id);
        BaseResponse VoidServiceActivity(int id);
        IList<ServiceRecord> GetAllServiceRecords(int? companyId=null);
        IList<ServiceRecord> GetVoidServiceRecords(int? companyId = null);
        BaseResponse SaveServiceRecord(SaveServiceRecordRequest request);
        IList<ServiceRecord> GetAllOutstandingReceivables(int? companyId = null);
        IList<ServiceRecord> GetOutstandingReceivableByCustomerId(int customerId);
        byte[] GetPdf(int id);
    }
}
