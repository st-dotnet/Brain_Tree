using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VT.Common.Utils
{
    public static class DateExtension
    {
        public static DateTime ToPstTime(this DateTime date)
        {
            return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(date, "Pacific Standard Time");
        }
    }
}
