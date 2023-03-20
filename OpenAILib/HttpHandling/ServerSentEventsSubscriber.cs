// Copyright (c) 2023 Owen Sigurdson
// MIT License

namespace OpenAILib.HttpHandling
{
    // This class is a lower-level subscriber for Server-Sent Events (SSE) that can be used with various OpenAI
    // endpoints. It is designed to work in conjunction with Channels to provide asynchronous enumerations of SSE events.
    internal class ServerSentEventsSubscriber : IAsyncDisposable
    {
        public const string OpenAISseTerminationMessage = "[DONE]";
        private readonly HttpClient _httpClient;
        private readonly string _url;
        private readonly CancellationToken _cancellationToken;
        private readonly CancellationTokenSource _internalCancellationSource;
        private readonly ValueTask _processingTask;

        // OpenAI SSE events are in the form: data: payload where 'payload' is a json formatted object. Event is raised only when non-empty, non-terminating payload messages are returned
        public EventHandler<string>? NewPayloadMessage;

        public ServerSentEventsSubscriber(HttpClient httpClient, string url, CancellationToken cancellationToken)
        {
            _httpClient = httpClient;
            _url = url;
            _internalCancellationSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _cancellationToken = _internalCancellationSource.Token;
            _processingTask = StartAsync();
        }

        private async ValueTask StartAsync()
        {
            bool isTerminationCriteriaSatisfied = false;
            while (!_cancellationToken.IsCancellationRequested && !isTerminationCriteriaSatisfied)
            {
                using var httpRequest = new HttpRequestMessage(HttpMethod.Get, _url);
                httpRequest.Headers.Add("Accept", "text/event-stream");
                using var httpHeaderResponse = await _httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, _cancellationToken);
                httpHeaderResponse.EnsureSuccessStatusCode();

                var stream = await httpHeaderResponse.Content.ReadAsStreamAsync(_cancellationToken);
                using var streamReader = new StreamReader(stream);

                try
                {
                    while (!_cancellationToken.IsCancellationRequested)
                    {
                        var sseText = await streamReader.ReadLineAsync().WaitAsync(_cancellationToken);
                        if (string.IsNullOrEmpty(sseText))
                        {
                            continue;
                        }

                        var tokens = sseText.Split("data: ");
                        if (tokens.Length != 2)
                        {
                            continue;
                        }

                        var payload = tokens[1].Trim();
                        if (string.IsNullOrEmpty(payload))
                        {
                            continue;
                        }

                        if (payload.StartsWith(OpenAISseTerminationMessage))
                        {
                            isTerminationCriteriaSatisfied = true;
                            // Raise event with exact termination message '[DONE]' so consumers do not
                            // need to (again) use StartsWith to detect termination
                            NewPayloadMessage?.Invoke(this, OpenAISseTerminationMessage);
                            break;
                        }

                        NewPayloadMessage?.Invoke(this, payload);
                    }

                }
                catch (IOException)
                {
                    // if we get an IOException, the server terminated the connection - in this case, re-open
                    // the connection after 5 seconds. This is a regular occurrence with fine tune events as OpenAI
                    // will sensibly terminate the connection after ~1 minute
                    await Task.Delay(5000, _cancellationToken);
                    continue;
                }
            }
        }

        public ValueTask DisposeAsync()
        {
            _internalCancellationSource.Cancel();
            return _processingTask;
        }
    }
}
