using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;

namespace AzureCognitiveSearch;

public class CognitiveSearchFunctions
{
    private readonly ILogger _logger;
    private readonly string? _searchApiKey;
    private readonly SearchIndexClient _searchIndexClient;
    private readonly string? _searchServiceName;
    private readonly Uri _serviceEndPoint;
    private readonly AzureKeyCredential _credential;

    public CognitiveSearchFunctions(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<CognitiveSearchFunctions>();
        _searchApiKey = Environment.GetEnvironmentVariable("SearchApiKey");
        _searchServiceName = Environment.GetEnvironmentVariable("SearchServiceName");
        _serviceEndPoint = new($"https://{_searchServiceName}.search.windows.net/");
        _credential = new(_searchApiKey);
        _searchIndexClient = new SearchIndexClient(_serviceEndPoint, _credential);
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

    [Function("AddValues")]
    public async Task<HttpResponseData> AddValues([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = $"{nameof(AddValues)}/{{indexName}}")] HttpRequestData req, string indexName)
    {
        _logger.LogInformation($"C# HTTP trigger function {nameof(CreateIndex)} processed a request.");
        SearchClient searchClient = new(_serviceEndPoint, indexName, _credential);

        string bodyString = await new StreamReader(req.Body).ReadToEndAsync();
        DocTest document = JsonConvert.DeserializeObject<DocTest>(bodyString);
        IndexDocumentsBatch<DocTest> batch = IndexDocumentsBatch.Create(IndexDocumentsAction.Upload(document));

        try
        {
            Response<IndexDocumentsResult> result = await searchClient.IndexDocumentsAsync(batch);
        }
        catch (Exception ex)
        {
            throw;
        }

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
        response.WriteString($"Documents added.");
        return response;
    }

    [Function("SearchValues")]
    public async Task<HttpResponseData> SearchValues([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = $"{nameof(SearchValues)}/{{indexName}}/{{searchText}}")] HttpRequestData req, string indexName, string searchText)
    {
        _logger.LogInformation($"C# HTTP trigger function {nameof(SearchValues)} processed a request.");
        SearchClient searchClient = new(_serviceEndPoint, indexName, _credential);

        SearchOptions searchOptions = new()
        {
            IncludeTotalCount = true,
            QueryType = SearchQueryType.Simple
        };
        SearchResults<DocTest> results = await searchClient.SearchAsync<DocTest>(searchText, searchOptions);

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "application/json; charset=utf-8");
        response.WriteString(JsonConvert.SerializeObject(results));
        return response;
    }
}
