// Copyright (c) 2023 Owen Sigurdson
// MIT License

namespace OpenAILib.Completions
{
    internal class CompletionSpecV01 : ICompletionSpecV01
    {
        private string _model = CompletionModels.TextDavinci0003;
        private double? _temperature;
        private int? _maxTokens;
        private double? _topProbability;
        private double? _frequencyPenalty;
        private double? _presencePenalty;
        private string[]? _stop;

        public ICompletionSpecV01 Model(string model)
        {
            _model = model;
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

        public CompletionRequest ToCompletionRequest(string prompt)
        {
            var completionRequest = new CompletionRequest(model: _model, prompt: prompt)
            {
                MaxTokens = _maxTokens,
                Temperature = _temperature,
                TopP = _topProbability,
                FrequencyPenalty = _frequencyPenalty,
                PresencePenalty = _presencePenalty,
                Stop = _stop
            };
            return completionRequest;
        }
    }
}
