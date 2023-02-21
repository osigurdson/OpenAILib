// Copyright (c) 2023 Owen Sigurdson
// MIT License

using System.Security.Cryptography;
using System.Text;

namespace OpenAILib.ResponseCaching
{
    internal static class RequestHashCalculator
    {
        public static Guid CalculateHash(string endPointName, string jsonContent)
        {
            using var ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonContent));
            ms.Write(Encoding.UTF8.GetBytes(endPointName));
            var textBytes = ms.ToArray();
            var hashedBytes = SHA1.HashData(textBytes).AsSpan();
            // Use the first 16 bytes of the SHA1 hash. Completely reasonable
            // in this case as there will be far fewer than 2^64 unique requests
            var result = new Guid(hashedBytes[..16]);
            return result;
        }
    }
}
