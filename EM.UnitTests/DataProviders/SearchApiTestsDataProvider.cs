using System.Collections.Generic;
using EM.MicroService.SearchApi.Models;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace EM.UnitTests.DataProviders;

public static class SearchApiTestsDataProvider
{
    public static IEnumerable<ITestCaseData> GetMultipleSearchData()
    {
        var testData = new List<ITestCaseData>();
        var searchDocs = new List<SearchDocument>
        {
            new()
            {
                Category = "Autotest",
                Document = "{\"Title\":\"MultipleSearchResult test title\", \"Description\":\"Multiple test description\"}",
                Location = "Moscow"
            },
            new()
            {
                Category = "Autotest",
                Document = "{\"Title\":\"MultipleSearchResult test title\", \"Description\":\"Multiple test description\"}",
                Location = "Moscow"
            },
            new()
            {
                Category = "Autotest",
                Document = "{\"Title\":\"Multiple test title\", \"Description\":\"Multiple test description\"}",
                Location = "Roma"
            },
            new()
            {
                Category = "MultipleAutotest",
                Document = "{\"Title\":\"MultipleSearchResult test title\", \"Description\":\"Multiple test description\"}",
                Location = "Roma"
            },
            new()
            {
                Category = "MultipleAutotest",
                Document = "{\"Title\":\"MultipleSearchResult test title\", \"Description\":\"Multiple test description\"}",
                Location = "Roma"
            }
        };

        testData.Add(new TestCaseData(
            searchDocs,
            "MultipleSearchResult",
            "Moscow",
            "Autotest",
            2
        ));
        
        testData.Add(new TestCaseData(
            searchDocs,
            "MultipleSearchResult",
            "Roma",
            "MultipleAutotest",
            2
        ));
        
        testData.Add(new TestCaseData(
            searchDocs,
            "MultipleSearchResult",
            null,
            null,
            4
        ));
        
        testData.Add(new TestCaseData(
            searchDocs,
            "Multiple",
            null,
            null,
            5
        ));
        
        return testData;
    }
    
    public static IEnumerable<ITestCaseData> GetSingleSearchData()
    {
        var testData = new List<ITestCaseData>();
        var searchDoc = new SearchDocument
        {
            Category = "Autotest",
            Document = "{\"Title\":\"SingleSearchResult test title\", \"Description\":\"SingleSearchResult test description\"}",
            Location = "Moscow"
        };
        testData.Add(new TestCaseData(
            searchDoc,
            "SingleSearchResult",
            searchDoc.Location,
            searchDoc.Category
            ));
        
        testData.Add(new TestCaseData(
            searchDoc,
            "SingleSearchResult",
            null,
            null
        ));
        
        return testData;
    }

    public static IEnumerable<ITestCaseData> GetZeroSearchData()
    {
        var testData = new List<ITestCaseData>();
        var searchDoc = new SearchDocument
        {
            Category = "Autotest",
            Document = "{\"Title\":\"SearchResult test title\", \"Description\":\"SearchResult test description\"}",
            Location = "Moscow"
        };
        testData.Add(new TestCaseData(
            searchDoc,
            "ZeroSearchResult",
            searchDoc.Location,
            searchDoc.Category
        ));
        
        testData.Add(new TestCaseData(
            searchDoc,
            "ZeroSearchResult",
            null,
            null
        ));
        
        return testData;
    }
}