using System;
using System.Text;
using System.Text.RegularExpressions;

namespace nuell
{
    public static class NumeralConvert
    {
        public static string PersianText(long n, bool ordinal = false)
        {
            n = Math.Abs(n);
            if (n == 0)
                if (ordinal)
                    throw new OverflowException();
                else
                    return "صفر";
            if (n >= 1e15)
                throw new OverflowException();
            if (n == 1 && ordinal)
                return "اول";
            const string and = " و ";
            string[] twenties = { "", "یک", "دو", "سه", "چهار", "پنج", "شش", "هفت", "هشت", "نه", "ده",
                "یازده", "دوازده", "سیزده", "چهارده", "پانزده", "شانزده", "هفده", "هجده", "نوزده" };
            string[] tens = { "", "", "بیست", "سی", "چهل", "پنجاه", "شصت", "هفتاد", "هشتاد", "نود" };
            string[] hundreds = { "", "صد", "دویست", "سیصد", "چهارصد", "پانصد", "ششصد", "هفتصد", "هشتصد", "نهصد" };
            string[] thousands = { "", " هزار", " میلیون", " میلیارد", " تریلیون" };
            var result = new StringBuilder();
            var mil = new StringBuilder();
            for (int pow = 0; n > 0; pow++)
            {
                mil.Clear();
                int cent = (int)(n % 1000);
                int dec = cent % 100;

                mil.Append(hundreds[cent / 100]);
                if (cent > 100 && dec > 0)
                    mil.Append(and);
                if (dec < 20)
                    mil.Append(twenties[dec]);
                else
                {
                    mil.Append(tens[dec / 10]);
                    if (dec % 10 != 0)
                    {
                        mil.Append(and);
                        mil.Append(twenties[dec % 10]);
                    }
                }
                if (cent != 0)
                    mil.Append(thousands[pow]);

                n /= 1000;
                result.Insert(0, mil);
                if (n > 0 && cent > 0)
                    result.Insert(0, and);
            }
            if (ordinal)
                if (result.ToString().EndsWith("سه"))
                {
                    result.Remove(result.Length - 2, 2);
                    result.Append("سوم");
                }
                else
                    result.Append("م");

            return result.ToString();
        }

        public static string KurdishText(long n)
        {
            n = Math.Abs(n);
            if (n == 0)
                return "سفر";
            if (n >= 1e15 || n < 0)
                throw new OverflowException();
            string and = " و ";
            string[] twenties = { "", "یەک", "دوو", "سێ", "چوار", "پێنج", "شەش", "حەوت", "ھەشت", "نۆ", "دە",
                "یازدە", "دوازدە", "سێزدە", "چواردە", "پازدە", "شازدە", "حەڤدە", "ھەژدە", "نۆزدە" };
            string[] tens = { "", "", "بیست", "سی", "چل", "پەنجا", "شەست", "حەفتا", "ھەشتا", "نەوەد" };
            string[] hundreds = { "", "سەد", "دووسەد", "سێسەد", "چوارسەد", "پێنسەد", "شەشسەد", "حەوسەد", "ھەشسەد", "نۆسەد" };
            string[] thousands = { "", " ھەزار", " ملیۆن", " ملیار", " ترلیۆن" };
            var result = new StringBuilder();
            var mil = new StringBuilder();
            for (int pow = 0; n > 0; pow++)
            {
                mil.Clear();
                int cent = (int)(n % 1000);
                int dec = cent % 100;

                mil.Append(hundreds[cent / 100]);
                if (cent > 100 && dec > 0)
                    mil.Append(and);
                if (dec < 20)
                    mil.Append(twenties[dec]);
                else
                {
                    mil.Append(tens[dec / 10]);
                    if (dec % 10 != 0)
                    {
                        mil.Append(and);
                        mil.Append(twenties[dec % 10]);
                    }
                }
                if (cent != 0)
                    mil.Append(thousands[pow]);

                n /= 1000;
                result.Insert(0, mil);
                if (n > 0 && cent > 0)
                    result.Insert(0, and);
            }
            return result.ToString();
        }

        public static byte[] HexToBytes(string hex)
        {
            var b = new byte[hex.Length / 2];
            for (int i = 0; i < b.Length; i++)
                b[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            return b;
        }

#pragma warning disable CS3001 // Argument type is not CLS-compliant
        public static string DecimalToBase(ulong n, string digits)
#pragma warning restore CS3001 // Argument type is not CLS-compliant
        {
            uint targetBase = (uint)digits.Length;
            string result = "";
            do
            {
                result = digits[(int)(n % targetBase)] + result;
                n = n / targetBase;
            }
            while (n > 0);
            return result;
        }

#pragma warning disable CS3002 // Return type is not CLS-compliant
        public static ulong BaseToDecimal(string n, string digits)
#pragma warning restore CS3002 // Return type is not CLS-compliant
        {
            uint sourceBase = (uint)digits.Length;
            ulong result = 0;
            ulong exp = 1;
            for (int i = n.Length - 1; i >= 0; i--)
            {
                result += (ulong)digits.IndexOf(n[i]) * exp;
                exp *= sourceBase;
            }
            return result;
        }

        public static string PersianDigits(object value, char decimalSeparator = '٫')
        {
            if (value == null)
                return null;
            string s = value.ToString();
            string[] convert = {
                @"(\d)\.(\d)", $"$1{decimalSeparator}$2",
                "0", "۰" ,
                "1", "۱" ,
                "2", "۲" ,
                "3", "۳" ,
                "4", "۴" ,
                "5", "۵" ,
                "6", "۶" ,
                "7", "۷" ,
                "8", "۸" ,
                "9", "۹"
            };
            for (int i = 0; i < convert.Length; i += 2)
                s = Regex.Replace(s, convert[i], convert[i + 1]);
            return s.Trim();
        }

        public static string LatinDigits(object value)
        {
            if (value == null)
                return null;
            string s = value.ToString();
            string[] convert = {
                "۰|٠", "0" ,
                "۱|۱", "1" ,
                "۲|٢", "2" ,
                "۳|٣", "3" ,
                "۴|٤", "4" ,
                "۵|٥", "5" ,
                "۶|٦", "6" ,
                "۷|٧", "7" ,
                "۸|٨", "8" ,
                "۹|٩", "9",
                @"(\d)\/(\d)", @"$1.$2"
            };
            for (int i = 0; i < convert.Length; i += 2)
                s = Regex.Replace(s, convert[i], convert[i + 1]);
            return s.Trim();
        }
    }
}
