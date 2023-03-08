// Copyright (c) 2023 Owen Sigurdson
// MIT License

namespace OpenAILib
{
    /// <summary>
    /// Represents chat completion customizations. See https://platform.openai.com/docs/api-reference/chat/create for a detailed explanation
    /// of how to use the tuning parameters. Default values used when not specified.
    /// </summary>
    public interface IChatCompletionSpecV01
    {
        /// <summary>
        /// Chat completion model to use. If not specified <see cref="ChatCompletionModels.Gpt35Turbo"/> used
        /// </summary>
        IChatCompletionSpecV01 Model(string model);

        /// <summary>
        /// Value between 0.0 and 2.0 which controls the randomness of the output. Default value is 1.0. Not recommended in conjuction with <see cref="TopProbability(double)"/>.
        /// </summary>
        IChatCompletionSpecV01 Temperature(double temperature);

        /// <summary>
        /// The maximum number of tokens allowed for the generated answer. By default, the number of tokens the model can return will be (4096 - prompt tokens).
        /// </summary>
        IChatCompletionSpecV01 MaxTokens(int maxTokens);

        /// <summary>
        /// An alternative to sampling with temperature, called nucleus sampling, where the model considers the results of the tokens with top_p probability mass. So 0.1 means only the tokens comprising the top 10% probability mass are considered.
        /// </summary>
        IChatCompletionSpecV01 TopProbability(double topProbability);

        /// <summary>
        /// Number between -2.0 and 2.0. Positive values penalize new tokens based on their existing frequency in the text so far, decreasing the model's likelihood to repeat the same line verbatim. Default is 0.0.
        /// </summary>
        IChatCompletionSpecV01 FrequencyPenalty(double frequencyPenalty);


        /// <summary>
        /// Number between -2.0 and 2.0. Positive values penalize new tokens based on whether they appear in the text so far, increasing the model's likelihood to talk about new topics. Default is 0.0.
        /// </summary>
        IChatCompletionSpecV01 PresencePenalty(double presencePenalty);

        /// <summary>
        /// Up to 4 sequences where the API will stop generating further tokens.
        /// </summary>
        IChatCompletionSpecV01 Stop(params string[] stop);

        /// <summary>
        /// A unique identifier representing your end-user, which can help OpenAI to monitor and detect abuse. 
        /// </summary>
        IChatCompletionSpecV01 User(string user);
    }
}
