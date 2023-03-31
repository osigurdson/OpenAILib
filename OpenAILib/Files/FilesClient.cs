// Copyright (c) 2023 Owen Sigurdson
// MIT License

using OpenAILib.HttpHandling;

namespace OpenAILib.Files
{
    // This is an internal, relatively low level interface to the OpenAI 'files' 
    // end point. At the time of writing, only one type of content can actually 
    // be uploaded which as jsonl (json lines) prompt, completion pairs. The class
    // utilizes general streams such that it could potentially be used for
    // other types of user uploaded content. 
    internal class FilesClient
    {
        private const string FilesEndPointName = "files";
        private readonly OpenAIHttpClient _httpClient;

        public FilesClient(OpenAIHttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> UploadStreamAsync(Stream stream, FilePurpose purpose, string fileName, CancellationToken cancellationToken = default)
        {
            // See https://platform.openai.com/docs/api-reference/files/upload for details
            // The file content and file 'purpose' are required. At the time of writing only
            // allowable purpose is 'fine-tune'
            var streamContent = new StreamContent(stream);
            var form = new MultipartFormDataContent
            {
                { new StringContent(purpose.ToString()), "purpose" },
                { streamContent, "file", fileName }
            };

            var response = await _httpClient.PostAsync<FileResponse>(
                originalRequestUri: FilesEndPointName,
                content: form,
                cancellationToken: cancellationToken);

            return response.Id;
        }

        public async Task<IReadOnlyList<FileResponse>> GetFilesAsync(CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync<FileListResponse>(FilesEndPointName, cancellationToken);

            if (response.Data == null)
            {
                throw new OpenAIException($"Failed to deserialize file information response.");
            }

            return response.Data.ToList();
        }

        public async Task<bool> DeleteAsync(string fileId, CancellationToken cancellationToken = default)
        {
            return await _httpClient.DeleteAsync($"{FilesEndPointName}/{fileId}", cancellationToken);
        }

        public async Task<FileResponse> GetFileInfoAsync(string fileId, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync<FileResponse>($"{FilesEndPointName}/{fileId}", cancellationToken);
            return response;
        }

        public async Task<byte[]> GetFileContentAsync(string fileId, CancellationToken cancellationToken = default)
        {
            using var httpResponse = await _httpClient.GetHttpResponseAsync($"{FilesEndPointName}/{fileId}/content", cancellationToken);
            httpResponse.EnsureSuccessStatusCode();
            return await httpResponse.Content.ReadAsByteArrayAsync(cancellationToken);
        }
    }

    internal struct FilePurpose
    {
        // This is an approach to allow for an extensible enum like structure without having to use reflection
        // on enum attributes.
        public static FilePurpose FineTune = new FilePurpose("fine-tune");

        private readonly string _purposeText;


        public FilePurpose(string purposeText)
        {
            _purposeText = purposeText;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            FilePurpose other = (FilePurpose)obj;
            return _purposeText == other._purposeText;
        }

        public override int GetHashCode()
        {
            return _purposeText.GetHashCode();
        }

        public override string ToString()
        {
            return _purposeText;
        }
    }
}
