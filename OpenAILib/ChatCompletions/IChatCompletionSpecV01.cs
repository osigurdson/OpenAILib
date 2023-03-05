// Copyright (c) 2023 Owen Sigurdson
// MIT License

namespace OpenAILib
{
    /// <summary>
    /// Represents chat completion customizations
    /// </summary>
    public interface IChatCompletionSpecV01
    {
        /// <summary>
        /// Chat completion model to use. If not specified <see cref="ChatCompletionModels.Gpt35Turbo"/> used
        /// </summary>
        IChatCompletionSpecV01 Model(string model);
        IChatCompletionSpecV01 Temperature(double temperature);
        IChatCompletionSpecV01 MaxTokens(int maxTokens);
        IChatCompletionSpecV01 TopProbability(double topProbability);
        IChatCompletionSpecV01 FrequencyPenalty(double frequencyPenalty);
        IChatCompletionSpecV01 PresencePenalty(double presencePenalty);
        IChatCompletionSpecV01 Stop(params string[] stop);
        IChatCompletionSpecV01 User(string user);
    }
}
