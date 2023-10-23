using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using EM.MicroService.SearchApi.Models;
using EM.MicroService.SearchApi.Options;
using EM.MicroService.SearchApi.Repository;
using EM.UnitTests.DataProviders;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace EM.UnitTests.Tests;

public class SearchApiTests
{
    private ElasticRepository _elasticRepository;

    public SearchApiTests()
    {
        _elasticRepository = new ElasticRepository(new OptionsWrapper<ElasticSearchOptions>(new ElasticSearchOptions
        {
            SearchIndexPattern = "em-search-test-index-{0}",
            Uri = "http://localhost:9200"
        }));
    }
    
    [Test]
    [TestCaseSource(typeof(SearchApiTestsDataProvider), nameof(SearchApiTestsDataProvider.GetMultipleSearchData))]
    public void MultipleSearchResult(List<SearchDocument> searchDocs, string query, string location, string category, int count)
    {
        var searchDocIds = new List<string>();
        try
        {
            foreach (var docId in searchDocs.Select(searchDoc => _elasticRepository.AddDocument(searchDoc).Result))
            {
                Assert.NotNull(docId, "Doc id after add document method is null");
                Assert.IsNotEmpty(docId, "Doc id after add document method is empty");
                searchDocIds.Add(docId);
            }

            Thread.Sleep(2000);
            var searchResult = _elasticRepository.SearchDocuments(query, location, new[] { category }).Result;
            Assert.AreEqual(count, searchResult.Count(), $"Search result count is not {count}");
            foreach (var actualSearchDoc in searchResult)
            {
                if(category != null) Assert.AreEqual(category, actualSearchDoc.Category);
                Assert.IsTrue(actualSearchDoc.Document.Contains(query),
                    $"Search result document:\r\n {actualSearchDoc.Document} \r\nnot contains query: {query}");
                if(location != null) Assert.AreEqual(location, actualSearchDoc.Location);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            foreach (var docId in searchDocIds)
            {
                _elasticRepository.DeleteDocuments(docId).Wait();
            }
        
            Thread.Sleep(2000);
            var searchResultAfterDelete = _elasticRepository.SearchDocuments(query, location, new []{category}).Result;
            Assert.AreEqual(0, searchResultAfterDelete.Count(), "Search result count is not empty after delete");
        }
    }

    [Test]
    [TestCaseSource(typeof(SearchApiTestsDataProvider), nameof(SearchApiTestsDataProvider.GetSingleSearchData))]
    public void SingleSearchResult(SearchDocument searchDoc, string query, string location, string category)
    {
        var docId = _elasticRepository.AddDocument(searchDoc).Result;
        Assert.NotNull(docId, "Doc id after add document method is null");
        Assert.IsNotEmpty(docId, "Doc id after add document method is empty");
        try
        {
            Thread.Sleep(2000);
            var searchResult = _elasticRepository.SearchDocuments(query, location, new[] { category }).Result;
            Assert.AreEqual(1, searchResult.Count(), "Search result count is not 1");
            var actualSearchDoc = searchResult.Single();
            Assert.AreEqual(searchDoc.Category, actualSearchDoc.Category);
            Assert.AreEqual(searchDoc.Document, actualSearchDoc.Document);
            Assert.AreEqual(searchDoc.Location, actualSearchDoc.Location);
        }
        catch(Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            _elasticRepository.DeleteDocuments(docId).Wait();
            Thread.Sleep(2000);
            var searchResultAfterDelete = _elasticRepository.SearchDocuments(query, location, new []{category}).Result;
            Assert.AreEqual(0, searchResultAfterDelete.Count(), "Search result count is not empty after delete");
        }
    }
    
    [Test]
    [TestCaseSource(typeof(SearchApiTestsDataProvider), nameof(SearchApiTestsDataProvider.GetZeroSearchData))]
    public void ZeroSearchResult(SearchDocument searchDoc, string query, string location, string category)
    {
        var docId = _elasticRepository.AddDocument(searchDoc).Result;
        Assert.NotNull(docId, "Doc id after add document method is null");
        Assert.IsNotEmpty(docId, "Doc id after add document method is empty");
        try
        {
            Thread.Sleep(2000);
            var searchResult = _elasticRepository.SearchDocuments(query, location, new[] { category }).Result;
            Assert.AreEqual(0, searchResult.Count(), "Search result count is not 0");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            Thread.Sleep(2000);
            _elasticRepository.DeleteDocuments(docId).Wait();
            var searchResultAfterDelete = _elasticRepository.SearchDocuments(query, location, new []{category}).Result;
            Assert.AreEqual(0, searchResultAfterDelete.Count(), "Search result count is not empty after delete");
        }
    }
}