// Copyright (c) 2023 Owen Sigurdson
// MIT License

namespace OpenAILib
{
    /// <summary>
    /// Represents customizations that can be applied to a given completion
    /// </summary>
    public interface ICompletionSpecV01
    {
        /// <summary>
        /// Completion model to use. This can be a stock Open AI completions model <see cref="CompletionModels"/> or a custom fine tuned model
        /// </summary>
        ICompletionSpecV01 Model(string model);
        ICompletionSpecV01 Temperature(double temperature);
        ICompletionSpecV01 MaxTokens(int maxTokens);
        ICompletionSpecV01 TopProbability(double topProbability);
        ICompletionSpecV01 FrequencyPenalty(double frequencyPenalty);
        ICompletionSpecV01 PresencePenalty(double presencePenalty);
        ICompletionSpecV01 Stop(params string[] stop);
    }
}
