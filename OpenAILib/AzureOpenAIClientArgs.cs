// Copyright (c) 2023 Owen Sigurdson
// MIT License

using OpenAILib.ResponseCaching;

namespace OpenAILib
{
    /// <summary>
    /// Represents arguments associated with initializing <see cref="AzureOpenAIClientArgs"/>
    /// </summary>
    public class AzureOpenAIClientArgs
    {
        internal const string AzureOpenAIUrl = "https://{resourceName}.openai.azure.com/openai/deployments/{deploymentId}/?api-version={apiVersion}";

        /// <summary>
        /// Azure OpenAI api key.
        /// </summary>
        public string ApiKey { get; init; }

        /// <summary>
        /// Azure OpenAI resource name.
        /// </summary>
        public string ResourceName { get; set; }

        /// <summary>
        /// Azure OpenAI deployment name, created whem deploying the model.
        /// </summary>
        public string DeploymentId { get; set; }

        /// <summary>
        /// Azure OpenAI api version, which is required for api access.
        /// </summary>
        public string ApiVersion { get; set; } = "2022-12-01";

        /// <summary>
        /// Base url of the Azure OpenAI API service
        /// (https://{resourceName}.openai.azure.com/openai/deployments/{deploymentId}/{endpoint}?api-
        ///  version={apiVersion})
        /// </summary>
        public string Url { get; init; } = AzureOpenAIUrl; 

        /// <summary>
        /// Optional <see cref="IResponseCache"/> implementation. Use the built-in <see
        /// cref="TempFileResponseCache"/> to cache responses in the temp directory.
        /// </summary>
        public IResponseCache ResponseCache { get; init; } = new NullResponseCache();

        public AzureOpenAIClientArgs(string apiKey, string resourceName, string deploymentId, string apiVersion = "2022-12-01")
        {
            ApiKey = apiKey;
            ResourceName = resourceName;
            DeploymentId = deploymentId;
            ApiVersion = apiVersion;
        }
    }
}