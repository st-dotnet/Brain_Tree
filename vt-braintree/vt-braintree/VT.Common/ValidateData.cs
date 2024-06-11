using System.Text.RegularExpressions;

namespace VT.Common
{
    public static class ValidateData
    {
        public static bool IsEmail(string email)
        {
            bool isEmail = Regex.IsMatch(email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
            return isEmail;
        }

        public static bool IsPhoneNumber(string phone)
        {
            bool isPhone = Regex.IsMatch(phone, @"^(?:(?:[\+]?(?<CountryCode>[\d]{1,3}(?:[ ]+|[\-.])))?[(]?(?<AreaCode>[\d]{3})[\-/)]?(?:[ ]+)?)?(?<Number>[a-zA-Z2-9][a-zA-Z0-9 \-.]{6,})(?:(?:[ ]+|[xX]|(i:ext[\.]?)){1,2}(?<Ext>[\d]{1,5}))?$", RegexOptions.IgnoreCase);
            return isPhone;
        }
        
        public static bool IsZipCode(string zipCode)
        {
            string _usZipRegEx = @"^\d{5}(?:[-\s]\d{4})?$";
            string _caZipRegEx = @"^([ABCEGHJKLMNPRSTVXY]\d[ABCEGHJKLMNPRSTVWXYZ])\ {0,1}(\d[ABCEGHJKLMNPRSTVWXYZ]\d)$";
            bool validZipCode = !((!Regex.Match(zipCode, _usZipRegEx).Success) && (!Regex.Match(zipCode, _caZipRegEx).Success));
            return validZipCode;
        }
    }
}
