// Copyright (c) 2023 Owen Sigurdson
// MIT License

using OpenAILib.FineTuning;

namespace OpenAILib.Completions
{
    internal class CompletionSpecV01 : ICompletionSpecV01
    {
        private string _model = CompletionModels.TextDavinci0003;
        private FineTuneInfo? _fineTunedModel;
        private double? _temperature;
        private int? _maxTokens;
        private double? _topProbability;
        private double? _frequencyPenalty;
        private double? _presencePenalty;
        private string[]? _stop;

        public ICompletionSpecV01 Model(string model)
        {
            _model = model;
            _fineTunedModel = null;
            return this;
        }

        public ICompletionSpecV01 Model(FineTuneInfo model)
        {
            _fineTunedModel = model;
            return this;
        }

        public ICompletionSpecV01 Temperature(double temperature)
        {
            _temperature = temperature;
            return this;
        }

        public ICompletionSpecV01 MaxTokens(int maxTokens)
        {
            _maxTokens = maxTokens;
            return this;
        }

        public ICompletionSpecV01 TopProbability(double topProbability)
        {
            _topProbability = topProbability;
            return this;
        }

        public ICompletionSpecV01 FrequencyPenalty(double frequencyPenalty)
        {
            _frequencyPenalty = frequencyPenalty;
            return this;
        }

        public ICompletionSpecV01 PresencePenalty(double presencePenalty)
        {
            _presencePenalty = presencePenalty;
            return this;
        }

        public ICompletionSpecV01 Stop(params string[] stop)
        {
            _stop = stop;
            return this;
        }

        public async Task<CompletionRequest> ToCompletionRequestAsync(string prompt, IFineTuneModelNameProvider fineTuneModelNameProvider)
        {
            string model;
            string[]? stop;
            if (_fineTunedModel != null)
            {
                var fineTunedModelName = await fineTuneModelNameProvider.GetFineTuneModelNameAsync(_fineTunedModel);
                if (string.IsNullOrEmpty(fineTunedModelName))
                {
                    throw new InvalidOperationException($"Cannot use fine tuned model '{_fineTunedModel.FineTuneId}' to create a completion as it has not successfully completed training");
                }
                model = fineTunedModelName;
                stop = new string[] { _fineTunedModel.CompletionSuffix };
                prompt += _fineTunedModel.PromptSuffix;
            }
            else
            {
                model = _model;
                stop = _stop;
            }
            var completionRequest = new CompletionRequest(model: model, prompt: prompt)
            {
                MaxTokens = _maxTokens,
                Temperature = _temperature,
                TopP = _topProbability,
                FrequencyPenalty = _frequencyPenalty,
                PresencePenalty = _presencePenalty,
                Stop = stop
            };
            return completionRequest;
        }
    }
}
