# AzureCognitiveSearch

## local.settings.json
- "SearchApiKey": "{search-service-key}",
- "SearchServiceName": "{search-service-name}"

## CreateIndex
```
[GET] http://localhost:7232/api/CreateIndex/{indexName}
```

## AddValue
```
[POST] http://localhost:7232/api/AddValue/{indexName}

{
	"id": "practitioner-1",
	"type": "practitioner",
	"family": "family practitioner-1",
	"given": "given practitioner-1",
	"practitioner": null,
	"organization": null,
	"identifier": "identifier practitioner-1",
	"address": "address practitioner-1",
	"name": null,
	"description": "description practitioner-1",
	"phone": "phone practitioner-1"
}
```

## AddValues
```
[POST] http://localhost:7232/api/AddValues/{indexName}

[
    {
        "id": "practitioner-1",
        "type": "practitioner",
        "family": "family practitioner-1",
        "given": "given practitioner-1",
        "practitioner": null,
        "organization": null,
        "identifier": "identifier practitioner-1",
        "address": "address practitioner-1",
        "name": null,
        "description": "description practitioner-1",
        "phone": "phone practitioner-1"
    },
    {
        "id": "practitioner-2",
        "type": "practitioner",
        "family": "family practitioner-2",
        "given": "given practitioner-2",
        "practitioner": null,
        "organization": null,
        "identifier": "identifier practitioner-2",
        "address": "address practitioner-2",
        "name": null,
        "description": "description practitioner-2",
        "phone": "phone practitioner-2"
    },
    {
        "id": "practitioner-role-1",
        "type": "practitioner-role",
        "family": null,
        "given": null,
        "practitioner": "practitioner/practitioner-1",
        "organization": "organization/organization-1",
        "identifier": null,
        "address": null,
        "name": null,
        "description": "description practitioner-role-1",
        "phone": null
    },
    {
        "id": "practitioner-role-2",
        "type": "practitioner-role",
        "family": null,
        "given": null,
        "practitioner": "practitioner/practitioner-1",
        "organization": "organization/organization-1",
        "identifier": null,
        "address": null,
        "name": null,
        "description": "description practitioner-role-1",
        "phone": null
    },
    {
        "id": "organization-1",
        "type": "organization",
        "family": null,
        "given": null,
        "practitioner": null,
        "organization": null,
        "identifier": null,
        "address": null,
        "name": "name organization-1",
        "description": "description organization-1",
        "phone": null
    },
    {
        "id": "organization-2",
        "type": "organization",
        "family": null,
        "given": null,
        "practitioner": null,
        "organization": null,
        "identifier": null,
        "address": null,
        "name": "name organization-2",
        "description": "description organization-2",
        "phone": null
    },
    {
        "id": "location-1",
        "type": "location",
        "family": null,
        "given": null,
        "practitioner": null,
        "organization": "organization/organization-1",
        "identifier": null,
        "address": "123 rue A",
        "name": "name location-1",
        "description": "description location-1",
        "phone": "phone location-1"
    },
    {
        "id": "location-2",
        "type": "location",
        "family": null,
        "given": null,
        "practitioner": null,
        "organization": "organization/organization-2",
        "identifier": null,
        "address": "234 rue B",
        "name": "name location-2",
        "description": "description location-2",
        "phone": "phone location-2"
    }
]
```

## SearchValues
```
[GET] http://localhost:7232/api/SearchValues/{indexName}/{searchText}
```

```
[GET] https://{search-service-name}.search.windows.net/indexes/{indexName}/docs?api-version=2020-06-30&search=car*
```

```
[GET] https://flavio-search.search.windows.net/indexes/fhir-index/docs?api-version=2020-06-30&queryType=full&search=/.*ner.*/&searchFields=Family, Given
```

To make a request contains. [Lucene](https://learn.microsoft.com/en-ca/azure/search/query-lucene-syntax) documentation.
```
search=/.*hi.*/&queryType=full
```

[search documentation](https://learn.microsoft.com/en-us/rest/api/searchservice/search-documents)