// Copyright (c) 2023 Owen Sigurdson
// MIT License

namespace OpenAILib.Tests
{
    internal static class TestCredentials
    {
        // Makes it possible to use non-nullable strings in test classes without warnings
        private const string OrganizationIdEnvironmentVariableName = "OpenAI_OrganizationId";
        private const string ApiKeyEnvironmentVariableName = "OpenAI_ApiKey";
        private const string AzureApiKeyEnvironmentVariableName = "Azure_OpenAI_ApiKey";
        private const string AzureResourceNameEnvironmentVariableName = "Azure_ResourceName";
        private const string AzureDeploymentIdEnvironmentVariableName = "Azure_DeploymentId";
        private const string AzureApiVersionEnvironmentVariableName = "Azure_ApiVersion";

        private static readonly Lazy<string> OrganizationIdEnvironmentVariableLazy
            = new(() => GetEnvironmentVariable(OrganizationIdEnvironmentVariableName));

        private static readonly Lazy<string> ApiKeyEnvironmentVariableLazy
            = new(() => GetEnvironmentVariable(ApiKeyEnvironmentVariableName));

        private static readonly Lazy<string> AzureApiKeyEnvironmentVariableLazy
        = new(() => GetEnvironmentVariable(AzureApiKeyEnvironmentVariableName));

        private static readonly Lazy<string> AzureResourceNameEnvironmentVariableLazy
        = new(() => GetEnvironmentVariable(AzureResourceNameEnvironmentVariableName));

        private static readonly Lazy<string> AzureDeploymentIdEnvironmentVariableLazy
        = new(() => GetEnvironmentVariable(AzureDeploymentIdEnvironmentVariableName));

        private static readonly Lazy<string> AzureApiVersionEnvironmentVariableLazy
        = new(() => GetEnvironmentVariable(AzureApiVersionEnvironmentVariableName));

        public static string OrganizationId => OrganizationIdEnvironmentVariableLazy.Value;
        public static string ApiKey => ApiKeyEnvironmentVariableLazy.Value;
        public static string AzureApiKey => AzureApiKeyEnvironmentVariableLazy.Value;
        public static string AzureResourceName => AzureResourceNameEnvironmentVariableLazy.Value;
        public static string AzureDeploymentId => AzureDeploymentIdEnvironmentVariableLazy.Value;
        public static string AzureApiVersion => AzureApiVersionEnvironmentVariableLazy.Value;

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
