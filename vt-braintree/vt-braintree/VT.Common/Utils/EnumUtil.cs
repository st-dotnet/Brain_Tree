using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace VT.Common.Utils
{
	public static class EnumUtil
	{
		public static TEnum Parse<TEnum>(string s, TEnum defValue) where TEnum : struct, IComparable, IFormattable, IConvertible
		{
			try
			{
				return (TEnum) Enum.Parse(typeof (TEnum), s, true);
			}
			catch
			{
				return defValue;
			}
		}

		public static string GetDescription<TEnum>(TEnum value) where TEnum : struct, IComparable, IFormattable, IConvertible
		{
			var fi = value.GetType().GetField(value.ToString(CultureInfo.InvariantCulture));

			var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

			if (attributes.Length > 0)
				return attributes[0].Description;
			
			return value.ToString(CultureInfo.InvariantCulture);
		}

        public static IEnumerable<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        public static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static IEnumerable<KeyValuePair<int, string>> GetKeyValuePairList<T>() where T : struct, IComparable, IFormattable, IConvertible
        {
            return GetValues<T>().Select(x => new
            {
                Id = Convert.ToInt32(x),
                Text = GetDescription(x)
            })
                .Select(x => new KeyValuePair<int, string>(x.Id, x.Text));
        }
	}
}