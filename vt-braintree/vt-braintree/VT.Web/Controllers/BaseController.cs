using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using DataAccess;
using VT.Common;
using VT.Web.Components.CustomFilter;

namespace VT.Web.Controllers
{
    [CheckSessionOut]
    public abstract class BaseController : Controller
    {
        protected CustomIdentity CurrentIdentity
        {
            get
            {
                if (User == null)
                    return null;

                if (!User.Identity.IsAuthenticated)
                    return null;

                return (CustomIdentity)User.Identity;
            }
        }
      
        protected string GetModelStateValidationErrors()
        {
            // Get all of the validation errors
            var errors = ModelState.Values.SelectMany(v => v.Errors);

            // Init string builder to hold the errors
            var sb = new StringBuilder();

            // Get all of the error messages
            foreach (var error in errors)
                sb.Append(error.ErrorMessage + Environment.NewLine);

            return "Validation Errors: " + Environment.NewLine + sb;
        }

        protected FileResult CreateCsvFile<T>(IEnumerable<T> data, string prefix, Action<MutableDataTable> beforeStreaming = null)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8))
                {
                    var dt = DataTable.New.FromEnumerable(data);

                    if (beforeStreaming != null)
                        beforeStreaming(dt);

                    dt.SaveToStream(streamWriter);

                    // Filename for download
                    var filename = string.Format("{0}_{1}.CSV", prefix, DateTime.UtcNow.ToString("yyyyMMdd_HHmmss"));

                    if (!string.IsNullOrEmpty(prefix) && prefix.Contains(".csv"))
                    {
                        filename = prefix;
                    }

                    return File(memoryStream.ToArray(), "text/csv", filename);
                }
            }
        }
    }
}