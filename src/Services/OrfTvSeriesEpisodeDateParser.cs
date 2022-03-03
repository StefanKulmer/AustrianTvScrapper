using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
