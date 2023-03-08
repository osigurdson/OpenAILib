// Copyright (c) 2023 Owen Sigurdson
// MIT License

namespace OpenAILib
{
    /// <summary>
    /// Represents customizations that can be applied to a given completion. Defaults values used when not specified.
    /// </summary>
    public interface ICompletionSpecV01
    {
        /// <summary>
        /// Completion model to use. This can be a stock Open AI completions model <see cref="CompletionModels"/> or a custom fine tuned model. Defaults to 'text-davinci-003'.
        /// </summary>
        ICompletionSpecV01 Model(string model);

        /// <summary>
        /// Value between 0.0 and 2.0 which controls the randomness of the output. Default value is 1.0. Not recommended in conjuction with <see cref="TopProbability(double)"/>.
        /// </summary>
        ICompletionSpecV01 Temperature(double temperature);

        /// <summary>
        /// The token count of your prompt plus max_tokens cannot exceed the model's context length. Most models have a context length of 2048 tokens (except for the newest models, which support 4096). Defaults to (just) 16.
        /// </summary>
        ICompletionSpecV01 MaxTokens(int maxTokens);

        /// <summary>
        /// An alternative to sampling with temperature, called nucleus sampling, where the model considers the results of the tokens with top_p probability mass. So 0.1 means only the tokens comprising the top 10% probability mass are considered.
        /// </summary>
        ICompletionSpecV01 TopProbability(double topProbability);

        /// <summary>
        /// Number between -2.0 and 2.0. Positive values penalize new tokens based on their existing frequency in the text so far, decreasing the model's likelihood to repeat the same line verbatim.
        /// </summary>
        ICompletionSpecV01 FrequencyPenalty(double frequencyPenalty);

        /// <summary>
        /// Number between -2.0 and 2.0. Positive values penalize new tokens based on whether they appear in the text so far, increasing the model's likelihood to talk about new topics.
        /// </summary>
        ICompletionSpecV01 PresencePenalty(double presencePenalty);

        /// <summary>
        /// Up to 4 sequences where the API will stop generating further tokens. The returned text will not contain the stop sequence.
        /// </summary>
        ICompletionSpecV01 Stop(params string[] stop);
    }
}
