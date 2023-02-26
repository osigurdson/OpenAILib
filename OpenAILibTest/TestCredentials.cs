// Copyright (c) 2023 Owen Sigurdson
// MIT License

namespace OpenAILib.Tests
{
    internal static class TestCredentials
    {
        // Makes it possible to use non-nullable strings in test classes without warnings
        private const string OrganizationIdEnvironmentVariableName = "OpenAI_OrganizationId";
        private const string ApiKeyEnvironmentVariableName = "OpenAI_ApiKey";

        private static readonly Lazy<string> OrganizationIdEnvironmentVariableLazy
            = new Lazy<string>(() => GetEnvironmentVariable(OrganizationIdEnvironmentVariableName));

        private static readonly Lazy<string> ApiKeyEnvironmentVariableLazy
            = new Lazy<string>(() => GetEnvironmentVariable(ApiKeyEnvironmentVariableName));

        public static string OrganizationId => OrganizationIdEnvironmentVariableLazy.Value;
        public static string ApiKey => ApiKeyEnvironmentVariableLazy.Value;

        private static string GetEnvironmentVariable(string environmentVariableName)
        {
            var environmentVariable = Environment.GetEnvironmentVariable(environmentVariableName);
            if (environmentVariable == null)
            {
                throw new InvalidOperationException($"Environment variable '{environmentVariableName}' not defined.");
            }
            return environmentVariable;
        }
    }
}
