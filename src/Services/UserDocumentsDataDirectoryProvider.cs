using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AustrianTvScrapper.Services
{
    public class UserDocumentsDataDirectoryProvider : IDataDirectoryProvider
    {
        public string GetDataDirectory()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "AustrianTvScrapper_old");
        }
    }
}
