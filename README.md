# AzureCognitiveSearch

## local.settings.json
- "SearchApiKey": "{search-service-key}",
- "SearchServiceName": "{search-service-name}"

## CreateIndex
```
[GET] http://localhost:7232/api/CreateIndex/{indexName}
```

## AddValues
```
[POST] http://localhost:7232/api/AddValues/{indexName}

{
    "id": "abc-123",
    "name": "my name",
    "description": "my description"
}
```

## SearchValues
```
[GET] http://localhost:7232/api/SearchValues/{indexName}/{searchText}
```

```
https://{search-service-name}.search.windows.net/indexes/{indexName}/docs?api-version=2020-06-30&search=car*
```

To make a request contains. [Lucene](https://learn.microsoft.com/en-ca/azure/search/query-lucene-syntax) documentation.
```
search=/.*hi.*/&queryType=full
```

[search documentation](https://learn.microsoft.com/en-us/rest/api/searchservice/search-documents)