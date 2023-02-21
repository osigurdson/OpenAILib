// Copyright (c) 2023 Owen Sigurdson
// MIT License

namespace OpenAILib.ResponseCaching
{
    internal class NullResponseCache : IResponseCache
    {
        public void PutResponse(Guid key, string response)
        {
        }

        public bool TryGetResponseAsync(Guid key, out string? cachedResponse)
        {
            cachedResponse = default;
            return false;
        }
    }
}
