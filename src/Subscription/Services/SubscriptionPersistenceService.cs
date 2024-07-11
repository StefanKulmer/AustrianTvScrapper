using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subscription.Services
{
    internal interface ISubscriptionPersistenceService
    {
        List<Model.Subscription> LoadAll(string fileName);
        void SaveAll(string fileName, List<Model.Subscription> subscriptions);
    }

    internal class SubscriptionPersistenceService : ISubscriptionPersistenceService
    {
        private readonly IDataDirectoryProvider dataDirectoryProvider;
        private const string FileName = "Subscribed.json";

        public SubscriptionPersistenceService(IDataDirectoryProvider dataDirectoryProvider)
        {
            this.dataDirectoryProvider = dataDirectoryProvider;
        }

        public List<Model.Subscription> LoadAll(string fileName)
        {
            var fileInfo = GetFileInfo(fileName);
            var result = JsonSerializationHelper.Deserialize<List<Model.Subscription>>(fileInfo);
            return result ?? [];
        }

        public void SaveAll(string fileName, List<Model.Subscription> subscriptions)
        {
            var fileInfo = GetFileInfo(fileName);
            JsonSerializationHelper.Serialize(fileInfo, subscriptions);
        }

        private IFileInfo GetFileInfo(string fileName)
        {
            var dataDirectory = dataDirectoryProvider.GetDataDirectory();
            var path = Path.Combine(dataDirectory.FullName, fileName);
            return dataDirectory.FileSystem.FileInfo.New(path);
        }
    }
}
