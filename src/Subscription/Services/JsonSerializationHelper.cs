using System.IO.Abstractions;
using System.Text.Json;

namespace Subscription.Services
{
    internal static class JsonSerializationHelper
    {
        public static void Serialize<T>(IFileInfo fileInfo, T value)
        {
            using var stream = fileInfo.OpenWrite();
            JsonSerializer.Serialize<T>(stream,  value, GetSerializerOptions());
        }

        public static T? Deserialize<T>(IFileInfo fileInfo)
        {
            using var stream = fileInfo.OpenRead();
            return JsonSerializer.Deserialize<T>(stream, GetSerializerOptions());
        }

        private static JsonSerializerOptions GetSerializerOptions()
        {
            return new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                WriteIndented = true
            };
        }
    }
}
