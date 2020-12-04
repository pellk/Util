using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace nuell
{
    public static class DateConvert
    {
        private static readonly PersianCalendar calendar = new PersianCalendar();

        public static (int year, int month, int day) ParsePersianDate(string date)
        {
            var reg = new Regex(@"^(\d+)\/(\d+)\/(\d+)$");
            var match = reg.Match(date);
            if (match.Success)
            {
                int year = int.Parse(match.Groups[1].Value);
                int month = int.Parse(match.Groups[2].Value);
                int day = int.Parse(match.Groups[3].Value);
                if (IsPersianDateValid(year, month, day))
                    return (year, month, day);
            }
            return (0, 0, 0);
        }

        public static bool IsPersianDateValid(int year, int month, int day)
        {
            try
            {
                calendar.ToDateTime(year, month, day, 0, 0, 0, 0);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string GregorianToPersian(int gregorianOADate, bool twoDigitYear, bool monthName, bool persianDigits = true)
            => GregorianToPersian(DateTime.FromOADate(gregorianOADate), twoDigitYear, monthName, persianDigits);

        public static string GregorianToPersian(int gregorianOADate, string format, bool persianDigits = true)
            => GregorianToPersian(DateTime.FromOADate(gregorianOADate), format, persianDigits);

        public static string GregorianToPersian(DateTime gregorianDate, bool twoDigitYear = true, bool monthName = false, bool persianDigits = true)
        {
            int year = calendar.GetYear(gregorianDate);
            int month = calendar.GetMonth(gregorianDate);
            int day = calendar.GetDayOfMonth(gregorianDate);

            string date = monthName
                ? $"{day} {PersianMonth(month)} {(twoDigitYear ? year % 100 : year)}"
                : $"{(twoDigitYear ? year % 100 : year)}/{month}/{day}";

            return persianDigits ? NumeralConvert.PersianDigits(date) : date;
        }

        public static string GregorianToPersian(DateTime gregorianDate, string format, bool persianDigits = true)
        {
            int year = calendar.GetYear(gregorianDate);
            int month = calendar.GetMonth(gregorianDate);
            int day = calendar.GetDayOfMonth(gregorianDate);

            var date = new StringBuilder(format);
            date.Replace("ddd", PersianWeekday(gregorianDate));
            date.Replace("dd", day.ToString("00"));
            date.Replace("d", day.ToString());
            date.Replace("mmm", PersianMonth(month));
            date.Replace("mm", month.ToString("00"));
            date.Replace("m", month.ToString());
            date.Replace("yyyy", year.ToString());
            date.Replace("yy", (year % 100).ToString());
            return persianDigits ? NumeralConvert.PersianDigits(date) : date.ToString();
        }

        public static DateTime PersianToGregorian(string persianDate)
        {
            var (year, month, day) = ParsePersianDate(persianDate);
            return calendar.ToDateTime(year, month, day, 0, 0, 0, 0);
        }

        public static DateTime PersianToGregorian(int year, int month, int day)
            => calendar.ToDateTime(year, month, day, 0, 0, 0, 0);

        public static string FormatPersian(int? persianDate, string format = "yy/m/d", bool persianDigits = true)
        {
            if (!persianDate.HasValue)
                return null;

            int date = persianDate.Value;

            int year = date / 10000;
            int month = date / 100 % 100;
            int day = date % 100;

            var strDate = new StringBuilder(format);
            strDate.Replace("ddd", PersianWeekday(PersianToGregorian(year, month, day)));
            strDate.Replace("dd", day.ToString("00"));
            strDate.Replace("d", day.ToString());
            strDate.Replace("mmm", PersianMonth(month));
            strDate.Replace("mm", month.ToString("00"));
            strDate.Replace("m", month.ToString());
            strDate.Replace("yyyy", year.ToString());
            strDate.Replace("yy", (year % 100).ToString());
            return persianDigits ? NumeralConvert.PersianDigits(strDate) : strDate.ToString();
        }

        public static string PersianMonth(int month)
        {
            if (month < 1 || month > 12)
                throw new ArgumentOutOfRangeException(nameof(month));
            string[] farsiMonth = { "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور",
                "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند" };
            return farsiMonth[month - 1];
        }

        public static string PersianMonth(DateTime gregorianDate)
            => PersianMonth(calendar.GetMonth(gregorianDate));

        public static string KurdishMonth(int month)
        {
            if (month < 1 || month > 12)
                throw new ArgumentOutOfRangeException(nameof(month));
            string[] months = { "کانوونی دووەم", "شوبات", "ئازار", "نیسان", "ئایار", "حوزەیران",
                    "تەمموز", "ئاب", "ئەیلوول", "تشرینی یەکەم", "تشرینی دووەم", "کانوونی یەکەم" };
            return months[month - 1];
        }

        public static string KurdishMonth(DateTime gregorianDate)
            => KurdishMonth(calendar.GetMonth(gregorianDate));

        public static string PersianWeekday(DateTime gregorianDate)
            => PersianWeekday(gregorianDate.DayOfWeek);

        public static string PersianWeekday(DayOfWeek dayOfWeek)
            => dayOfWeek switch
            {
                DayOfWeek.Saturday => "شنبه",
                DayOfWeek.Sunday => "یکشنبه",
                DayOfWeek.Monday => "دوشنبه",
                DayOfWeek.Tuesday => "سه‌شنبه",
                DayOfWeek.Wednesday => "چهارشنبه",
                DayOfWeek.Thursday => "پنجشنبه",
                _ => "جمعه",
            };

        public static string TimeAgo(DateTime t, string lang)
        {
            var (aFew, s, ago, second, minute, hour, day, yesterday, week, month, year) = lang switch
            {
                "ku" => ("چەن", "", "لەمەو پێش", "چرکە", "خولەک", "کات‌ژمێر", "رۆژ", "دوێنێ", "ھەفتە", "مانگ", "ساڵ"),
                "fa" => ("چند", "", "پیش", "ثانیه", "دقیقه", "ساعت", "روز", "دیروز", "هفته", "ماه", "سال"),
                _ => ("a few", "s", "ago", "second", "minute", "hour", "day", "yesterday", "week", "month", "year")
            };
            var span = DateTime.Now.Subtract(t);
            string output;
            if (span.TotalSeconds < 60)
                output = $"{aFew} {second}{s} {ago}";
            else if (span.TotalMinutes < 60)
                output = $"{(int)span.TotalMinutes} {minute}{s} {ago}";
            else if (span.TotalHours < 24)
                output = $"{(int)span.TotalHours} {hour}{s} {ago}";
            else if (span.TotalDays < 2)
                output = yesterday;
            else if (span.TotalDays < 7)
                output = $"{(int)span.TotalDays} {day}{s} {ago}";
            else if (span.TotalDays < 31)
                output = $"{(int)span.TotalDays / 7} {week}{s} {ago}";
            else if (span.TotalDays < 365)
                output = $"{(int)(span.TotalDays / 30)} {month}{s} {ago}";
            else
                output = $"{(int)(span.TotalDays / 365)} {year}{s} {ago}";
            return lang == "fa" || lang == "ku" ? NumeralConvert.PersianDigits(output) : output;
        }
    }
}
