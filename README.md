# AzureCognitiveSearch

## local.settings.json
- "SearchApiKey": "{{search-service-key}}",
- "SearchServiceName": "{{search-service-name}}"

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