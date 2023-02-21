// Copyright (c) 2023 Owen Sigurdson
// MIT License

namespace OpenAILib.ResponseCaching
{
    /// <summary>
    /// Stores responses to OpenAI API requests in $tempdir/OpenAILibCache/ 
    /// </summary>
    public class TempFileResponseCache : IResponseCache
    {
        private const string OpenAiCacheDirectoryName = "OpenAILibCache";
        private static readonly Lazy<string> _cacheDirectory = new(() => Path.Combine(Path.GetTempPath(), OpenAiCacheDirectoryName));
        
        public static string CacheDirectory => _cacheDirectory.Value;

        public bool TryGetResponseAsync(Guid contentKey, out string? cachedResponse)
        {
            var fullPath = GetFilePath(contentKey);

            cachedResponse = default;

            if (!File.Exists(fullPath))
            {
                return false;
            }

            try
            {
                cachedResponse = File.ReadAllText(fullPath);
                if (string.IsNullOrEmpty(cachedResponse))
                {
                    return false;
                }
                return true;
            }
            catch (IOException)
            {
                return false;
            }
        }

        public void PutResponse(Guid contentKey, string responseText)
        {
            Directory.CreateDirectory(CacheDirectory);

            var filePath = GetFilePath(contentKey);
            File.WriteAllText(filePath, responseText);
        }

        private static string GetFilePath(Guid contentKey)
        {
            var fileName = $"{contentKey}.json";
            return Path.Combine(CacheDirectory, fileName);

        }
    }
}
