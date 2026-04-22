using System;
using System.Globalization;

namespace Bham.BizTalk.Rest
{
    /// <summary>
    /// Converts UK-local dates and times into Gallagher's required UTC ISO-8601 representation.
    /// </summary>
    public static class GallagherDateTimeHelper
    {
        private static readonly string[] SupportedUkDateFormats =
        {
            "dd/MM/yy",
            "dd/MM/yyyy",
            "d/M/yy",
            "d/M/yyyy"
        };

        private static readonly string[] SupportedUkTimeFormats =
        {
            "HH:mm",
            "H:mm",
            "HH:mm:ss",
            "H:mm:ss"
        };

        /// <summary>
        /// Converts a UK date and UK local time into UTC format like 2026-04-17T09:00:00Z.
        /// </summary>
        public static string ConvertUkDateAndTimeToUtcIso8601(string ukDate, string ukTime)
        {
            if (string.IsNullOrWhiteSpace(ukDate)) throw new ArgumentNullException(nameof(ukDate));
            if (string.IsNullOrWhiteSpace(ukTime)) throw new ArgumentNullException(nameof(ukTime));

            var culture = CultureInfo.GetCultureInfo("en-GB");

            DateTime parsedDate;
            if (!DateTime.TryParseExact(
                ukDate.Trim(),
                SupportedUkDateFormats,
                culture,
                DateTimeStyles.None,
                out parsedDate))
            {
                throw new FormatException("ukDate must be a UK date such as dd/MM/yy or dd/MM/yyyy.");
            }

            DateTime parsedTime;
            if (!DateTime.TryParseExact(
                ukTime.Trim(),
                SupportedUkTimeFormats,
                culture,
                DateTimeStyles.None,
                out parsedTime))
            {
                throw new FormatException("ukTime must be a 24-hour UK time such as HH:mm or HH:mm:ss.");
            }

            var ukLocalDateTime = new DateTime(
                parsedDate.Year,
                parsedDate.Month,
                parsedDate.Day,
                parsedTime.Hour,
                parsedTime.Minute,
                parsedTime.Second,
                DateTimeKind.Unspecified);

            var ukTimeZone = ResolveUkTimeZone();
            var utcDateTime = TimeZoneInfo.ConvertTimeToUtc(ukLocalDateTime, ukTimeZone);

            return utcDateTime.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture);
        }

        private static TimeZoneInfo ResolveUkTimeZone()
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
            }
            catch (TimeZoneNotFoundException)
            {
                return TimeZoneInfo.FindSystemTimeZoneById("Europe/London");
            }
        }
    }
}