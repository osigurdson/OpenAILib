// Copyright (c) 2023 Owen Sigurdson
// MIT License

namespace OpenAILib.ResponseCaching
{
    public interface IResponseCache
    {
        bool TryGetResponseAsync(Guid key, out string? cachedResponse);
        void PutResponse(Guid key, string response);
    }
}
