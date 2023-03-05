// Copyright (c) 2023 Owen Sigurdson
// MIT License

namespace OpenAILib.ChatCompletions
{
    internal class ChatCompletionSpecV01 : IChatCompletionSpecV01
    {
        private string _model = ChatCompletionModels.Gpt35Turbo;
        private double? _temperature;
        private int? _maxTokens;
        private double? _topProbability;
        private double? _frequencyPenalty;
        private double? _presencePenalty;
        private string[]? _stop;
        private string? _user;

        public IChatCompletionSpecV01 Model(string model)
        {
            _model = model;
            return this;
        }

        public IChatCompletionSpecV01 Temperature(double temperature)
        {
            _temperature = temperature;
            return this;
        }

        public IChatCompletionSpecV01 MaxTokens(int maxTokens)
        {
            _maxTokens = maxTokens;
            return this;
        }

        public IChatCompletionSpecV01 TopProbability(double topProbability)
        {
            _topProbability = topProbability;
            return this;
        }

        public IChatCompletionSpecV01 FrequencyPenalty(double frequencyPenalty)
        {
            _frequencyPenalty = frequencyPenalty;
            return this;
        }

        public IChatCompletionSpecV01 PresencePenalty(double presencePenalty)
        {
            _presencePenalty = presencePenalty;
            return this;
        }

        public IChatCompletionSpecV01 Stop(params string[] stop)
        {
            _stop = stop;
            return this;
        }

        public IChatCompletionSpecV01 User(string user)
        {
            _user = user;
            return this;
        }

        public ChatCompletionRequest ToRequest(IEnumerable<ChatMessage> sequence)
        {
            var chatMessageRequest = sequence
                .Select(chatMessage => new ChatMessageRequest(role: chatMessage.Role, content: chatMessage.Message))
                .ToList();

            var chatCompletionRequest = new ChatCompletionRequest(
                model: _model,
                messages: chatMessageRequest)
            {
                MaxTokens = _maxTokens,
                Temperature = _temperature,
                TopP = _topProbability,
                FrequencyPenalty = _frequencyPenalty,
                PresencePenalty = _presencePenalty,
                Stop = _stop,
                User = _user
            };

            return chatCompletionRequest;
        }
    }
}
