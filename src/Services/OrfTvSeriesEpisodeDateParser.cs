using System;
using System.Globalization;

namespace AustrianTvScrapper.Services
{
    internal static class OrfTvSeriesEpisodeDateParser
    {
        public static DateTime Parse(string dateTimeString)
        {
            dateTimeString = dateTimeString.Replace("CET", " ").Replace("CEST", " ");
            return DateTime.ParseExact(dateTimeString, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        }
    }
}
