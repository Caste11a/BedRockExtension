using Amazon;
using Amazon.BedrockRuntime;
using Amazon.BedrockRuntime.Model;
using Amazon.Runtime;
using System.Text;
using System.Text.Json;

namespace BedRockExtension.Services
{
    internal class BedrockService : IDisposable
    {
        private readonly IAmazonBedrockRuntime _client;

        public BedrockService(string region, string profile)
        {
            AWSCredentials creds = string.IsNullOrWhiteSpace(profile)
                ? FallbackCredentialsFactory.GetCredentials()
                : new StoredProfileAWSCredentials(profile);

            _client = new AmazonBedrockRuntimeClient(creds, RegionEndpoint.GetBySystemName(region));
        }

        /// <summary>
        /// Titan Text 系モデル（inputText）想定の最小呼び出し
        /// </summary>
        public async Task<string> InvokeTextAsync(string modelId, string prompt, CancellationToken ct)
        {
            var payload = new
            {
                inputText = prompt,
                textGenerationConfig = new
                {
                    maxTokenCount = 512,
                    temperature = 0.3,
                    topP = 0.9
                }
            };

            var json = JsonSerializer.Serialize(payload);
            var req = new InvokeModelRequest
            {
                ModelId = modelId,
                ContentType = "application/json",
                Accept = "application/json",
                Body = new MemoryStream(Encoding.UTF8.GetBytes(json))
            };

            var res = await _client.InvokeModelAsync(req, ct);

            using var reader = new StreamReader(res.Body);
            var body = await reader.ReadToEndAsync();

            // Titan の返却例: { "results": [ { "outputText": "..." } ], ... }
            try
            {
                using var doc = JsonDocument.Parse(body);
                if (doc.RootElement.TryGetProperty("results", out var results)
                    && results.GetArrayLength() > 0
                    && results[0].TryGetProperty("outputText", out var text))
                {
                    return text.GetString() ?? string.Empty;
                }
            }
            catch
            {
                // JSON でない/形式違い → そのまま返す
            }

            return body;
        }

        public void Dispose() => _client.Dispose();
    }
}
