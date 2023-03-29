// Copyright (c) 2023 Owen Sigurdson
// MIT License

namespace OpenAILib.HttpHandling
{
    // <summary>
    // The LazyHttpClient class inherits from HttpClient and provides additional functionality for handling query parameters.
    // </summary>
    public class LazyHttpClient : HttpClient
    {
        // <summary>
        // A dictionary to store query parameters as key-value pairs.
        // </summary>
        private Dictionary<string, string> queryParameters = new Dictionary<string, string>();

        public LazyHttpClient() : base()
        {
        }

        public LazyHttpClient(HttpMessageHandler handler) : base(handler)
        {
        }

        public LazyHttpClient(HttpMessageHandler handler, bool disposeHandler) : base(handler, disposeHandler)
        {
        }

        public Dictionary<string, string> QueryParameters { get { return queryParameters; } set { queryParameters = value; } }
        public void AddParam(string key, string value)
        {
            QueryParameters[key] = value;
        }

        public Uri ReformUri(Uri? requestUri)
        {
            var uri = BaseAddress == null ? requestUri : new Uri(BaseAddress, requestUri);

            var query = uri.Query;
            if (query.Length > 1)
            {
                query += "&";
            }

            query += string.Join("&", queryParameters.Select(param => $"{Uri.EscapeDataString(param.Key)}={Uri.EscapeDataString(param.Value)}"));
            uri = new UriBuilder(uri) { Query = query }.Uri;
            return uri;
        }
    }
}