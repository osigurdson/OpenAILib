// Copyright (c) 2023 Owen Sigurdson
// MIT License

namespace OpenAILib.Serialization
{
    internal static class Base64UrlConverter
    {
        // TODO - Ideally this should be replaced with custom, reduced allocation methods
        public static string ConvertToBase64Url(ReadOnlySpan<byte> bytes)
        {
            return Convert.ToBase64String(bytes)
                .Replace("+", "-").Replace("/", "_").TrimEnd('=');
        }

        public static byte[] ConvertFromBase64Url(string base64Url)
        {
            // Replace Base64URL-specific characters with their Base64 equivalents
            string base64 = base64Url.Replace("-", "+").Replace("_", "/");

            // Pad the Base64 string if necessary
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }

            // Decode the Base64 string to bytes
            return Convert.FromBase64String(base64);
        }
    }
}
