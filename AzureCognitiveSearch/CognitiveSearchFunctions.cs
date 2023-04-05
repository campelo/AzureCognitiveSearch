using Azure;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace AzureCognitiveSearch
{
    public class CognitiveSearchFunctions
    {
        private readonly ILogger _logger;
        private readonly SearchIndexClient _searchIndexClient;

        public CognitiveSearchFunctions(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<CognitiveSearchFunctions>();
            string? searchApiKey = Environment.GetEnvironmentVariable("SearchApiKey");
            string? searchServiceName = Environment.GetEnvironmentVariable("SearchServiceName");

            Uri serviceEndPoint = new($"https://{searchServiceName}.search.windows.net/");
            AzureKeyCredential credential = new(searchApiKey);

            _searchIndexClient = new SearchIndexClient(serviceEndPoint, credential);
        }

        [Function("CreateIndex")]
        public async Task<HttpResponseData> CreateIndex([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = $"{nameof(CreateIndex)}/{{indexName}}")] HttpRequestData req, string indexName)
        {
            _logger.LogInformation($"C# HTTP trigger function {nameof(CreateIndex)} processed a request.");

            bool indexExists = await ExistIndexAsync(indexName);

            if (!indexExists)
            {
                SearchIndex index = new SearchIndex(indexName)
                {
                    Fields = new List<SearchField>()
                    {
                        new ("Id", SearchFieldDataType.String) { IsKey = true },
                        new ("Name", SearchFieldDataType.String),
                        new ("Description", SearchFieldDataType.String),
                    },
                };

                Response<SearchIndex> createIndexResponse = await _searchIndexClient.CreateIndexAsync(index);
                if (!createIndexResponse.GetRawResponse().Status.Equals(201))
                {
                    Enum.TryParse<HttpStatusCode>(createIndexResponse.GetRawResponse().Status.ToString(), out HttpStatusCode status);
                    var errorresponse = req.CreateResponse(status);
                    errorresponse.Headers.Add("Content-Type", "text/plain; charset=utf-8");
                    errorresponse.WriteString($"Failed to create index: {createIndexResponse.GetRawResponse().ReasonPhrase}");
                    return errorresponse;

                }

                var okResponse = req.CreateResponse(HttpStatusCode.OK);
                okResponse.Headers.Add("Content-Type", "text/plain; charset=utf-8");
                okResponse.WriteString($"Index {indexName} created successfully.");
                return okResponse;
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
            response.WriteString($"Index {indexName} already exists.");
            return response;

        }

        private async Task<bool> ExistIndexAsync(string indexName)
        {
            try
            {
                await _searchIndexClient.GetIndexAsync(indexName);
                return true;
            }
            catch { return false; }
        }
    }
}
