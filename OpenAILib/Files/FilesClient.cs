// Copyright (c) 2023 Owen Sigurdson
// MIT License

using System.Text.Json;

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
        private readonly HttpClient _httpClient;

        public FilesClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> UploadStreamAsync(Stream stream, string purpose, string fileName)
        {
            // See https://platform.openai.com/docs/api-reference/files/upload for details
            // The file content and file 'purpose' are required. At the time of writing only
            // allowable purpose is 'fine-tune'
            var streamContent = new StreamContent(stream);
            var form = new MultipartFormDataContent
            {
                { new StringContent(purpose), "purpose" },
                { streamContent, "file", fileName }
            };

            using var httpResponse = await _httpClient.PostAsync(FilesEndPointName, form);
            httpResponse.EnsureSuccessStatusCode();
            using var responseText = await httpResponse.Content.ReadAsStreamAsync();
            var deserializedResponse = JsonSerializer.Deserialize<FileResponse>(responseText);

            if (deserializedResponse?.Id == null)
            {
                throw new OpenAIException($"Failed to deserialize file upload response: '{responseText}'");
            }

            return deserializedResponse.Id;
        }

        public async Task<IReadOnlyList<FileResponse>> GetFilesAsync()
        {
            using var httpResponse = await _httpClient.GetAsync(FilesEndPointName);
            httpResponse.EnsureSuccessStatusCode();
            using var responseStream = await httpResponse.Content.ReadAsStreamAsync();
            var deserilizedResponse = JsonSerializer.Deserialize<FileListResponse>(responseStream);
            
            if (deserilizedResponse?.Data == null)
            {
                throw new OpenAIException($"Failed to deserialize file information response.");
            }

            return deserilizedResponse.Data.ToList();
        }

        public async Task<bool> DeleteFileAsync(string fileId)
        {
            using var httpResponse = await _httpClient.DeleteAsync($"{FilesEndPointName}/{fileId}");
            if (httpResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return false;
            }
            httpResponse.EnsureSuccessStatusCode();
            using var responseStream = await httpResponse.Content.ReadAsStreamAsync();
            var response = JsonSerializer.Deserialize<DeletedFileResponse>(responseStream);
            
            if (response == null || !response.Deleted.HasValue)
            {
                throw new OpenAIException($"Failed to deserialize file deletion response.");
            }
            
            return response.Deleted.Value;
        }

        public async Task<FileResponse> GetFileInfoAsync(string fileId)
        {
            using var httpResponse = await _httpClient.GetAsync($"{FilesEndPointName}/{fileId}");
            httpResponse.EnsureSuccessStatusCode();
            using var responseStream = await httpResponse.Content.ReadAsStreamAsync();
            var response = JsonSerializer.Deserialize<FileResponse>(responseStream);
            
            if (response == null)
            {
                throw new OpenAIException($"Failed to deserialize file deletion response.");
            }
            
            return response;
        }

        public async Task<byte[]> GetFileContentAsync(string fileId)
        {
            using var httpResponse = await _httpClient.GetAsync($"{FilesEndPointName}/{fileId}/content");
            httpResponse.EnsureSuccessStatusCode();
            return await httpResponse.Content.ReadAsByteArrayAsync();
        }
    }
}
