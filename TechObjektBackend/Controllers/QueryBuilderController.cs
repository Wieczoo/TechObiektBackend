using System;
using Microsoft.AspNetCore.Mvc;
using TechObjektBackend.Services;
using MongoDB.Bson;
using Newtonsoft.Json;
using TechObjektBackend.Models;
using System.Collections;
using Microsoft.AspNetCore.WebUtilities;

[ApiController]
[Route("api/[controller]")]
public class QueryBuilderController : ControllerBase
{
    public readonly QueryBuilderServices _queryBuilderServices;


    public QueryBuilderController(QueryBuilderServices queryBuilderServices) =>
        _queryBuilderServices = queryBuilderServices;

    [HttpGet("collectionsnames")]
    public async Task<List<string>> Get() =>
        await _queryBuilderServices.GetCollectionsNames();


    [HttpGet("{collectionName}/columns")]
    public async Task<List<ColumnsNamesWithType>> GetColumns(string collectionName) =>
        await _queryBuilderServices.GetColumnsNames(collectionName);



    [HttpPost("{collectionName}/data/{filterConditions}")]
    public async Task<string> GetDataDocument(DocumentQuery documentQuery)
    {
        //System.Console.WriteLine(documentQuery.collectionName);
        //System.Console.WriteLine(filterConditions);
        //[FromQuery]string collectionName, [FromBody]List<FilterCondition> filterConditions

        //string jsonData = JsonConvert.SerializeObject(data);
        var result =    await _queryBuilderServices.GetData(documentQuery.collectionName, documentQuery);
        return JsonConvert.SerializeObject(result);
    }

}
