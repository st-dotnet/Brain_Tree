using System.Web;

namespace VT.Web.Interfaces
{
    public interface IAmazonFileUpload
    {
        string UploadFile(HttpPostedFileBase file);
    }
}