namespace AustrianTvScrapper.Services
{
    class DirectorySanitizer
    {
        public static string Sanitize(string directoryName)
        {
            var firstStep = directoryName.Replace("Ä", "Ae")
                .Replace("Ö", "Oe")
                .Replace("Ü", "Ue")
                .Replace("ä", "ae")
                .Replace("ö", "oe")
                .Replace("ü", "ue")
                .Replace("ß", "ss")
                .Replace("/", "v")
                .Replace(": ", " - ")
                .Replace("\"", "")
                .Replace("–", "-")
                .Replace("…", "...")
                .Replace(":", "-");

            string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return System.Text.RegularExpressions.Regex.Replace(firstStep, invalidRegStr, "_");
        }
    }
}
