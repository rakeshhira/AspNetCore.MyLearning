using System;
using System.Text;

namespace DotNetCoreCommon
{
	public static class StringConvertExtensions
	{
		private static readonly string SemicolonSeperatedFalseValues = "0;off;no;false"; // this list should be moved to a resource file
		private static readonly string SemicolonSeperatedTrueValues = "1;on;yes;true"; // this list should be moved to a resource file
		private const char Space = ' ';
		private const char Semicolon = ';';

		public static bool ToBoolean(this string value, bool defaultValue, string semicolonSeperatedFalseValues = null, string semicolonSeperatedTrueValues = null)
		{
			try
			{
				return ToBoolean(value, semicolonSeperatedFalseValues, semicolonSeperatedTrueValues);
			}
			catch (Exception)
			{
				return defaultValue;
			}
		}

		public static bool ToBoolean(this string value, string semicolonSeperatedFalseValues = null, string semicolonSeperatedTrueValues = null)
		{
			if (value == null)
			{
				throw new ArgumentNullException(nameof(value));
			}

			string valueLowerCase = value.ToLower().Trim(new char[] { Space, Semicolon });
			string semicolonSeperatedFalseValuesToUse = semicolonSeperatedFalseValues ?? SemicolonSeperatedFalseValues;
			if (semicolonSeperatedFalseValuesToUse.ToLower().Contains(valueLowerCase))
			{
				return false;
			}

			string semicolonSeperatedTrueValuesToUse = semicolonSeperatedTrueValues ?? SemicolonSeperatedTrueValues;
			if (semicolonSeperatedTrueValuesToUse.ToLower().Contains(valueLowerCase))
			{
				return true;
			}

			return bool.Parse(value);
		}

		public static bool BoolTryParse(this string value, out bool parsedValue, string semicolonSeperatedFalseValues = null, string semicolonSeperatedTrueValues = null)
		{
			parsedValue = false;
			try
			{
				parsedValue = ToBoolean(value, semicolonSeperatedFalseValues, semicolonSeperatedTrueValues);
			}
			catch (Exception)
			{
				return false;
			}

			return true;
		}

		public static byte[] ToUtf8Bytes(this string value)
		{
			return Encoding.UTF8.GetBytes(value);
		}

		public static string FromUtf8ToString(this byte[] value)
		{
			return Encoding.UTF8.GetString(value);
		}
	}
}
