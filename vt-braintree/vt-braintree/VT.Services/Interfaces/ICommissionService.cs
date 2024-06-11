using System.Collections.Generic;
using VT.Data.Entities;
using VT.Services.DTOs;

namespace VT.Services.Interfaces
{
    public interface ICommissionService
    {
        IList<Commission> GetCommissions(int? companyId);
        BaseResponse SaveCommission(Commission commission);
        GetInvoiceDetailResponse GetInvoiceDetails(GetInvoiceDetailRequest request);
        GetVoidInvoiceDetailResponse GetVoidInvoiceDetails(GetVoidInvoiceDetailRequest request);
        GetCommissionExpenseResponse GetCommissionExpenseDetails(GetCommissionExpenseRequest request);
        GetCommissionExpenseResponse GetCommissionsForSuperAdmin(GetCommissionsRequest request);
    }
}
