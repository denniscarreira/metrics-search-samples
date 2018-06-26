using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System;

namespace AzureSearch
{
    class Program
    {
        private const string SalesIndexName = "salesindex";
        private static ISearchServiceClient _searchClient;
        private static ISearchIndexClient _indexClient;
        static void Main(string[] args)
        {
            string searchServiceName = "callisto-poc";
            string apiKey = "F1B07470817299CF607C8A4A83D055EF";

            // Create an HTTP reference to the catalog index
            _searchClient = new SearchServiceClient(searchServiceName, new SearchCredentials(apiKey));
            _indexClient = _searchClient.Indexes.GetClient(SalesIndexName);
            ExecuteQueries();
        }

        static void ExecuteQueries()
        {
            SearchParameters parameters;
            DocumentSearchResult results;

            Console.WriteLine("Id eq 1 -> return fileds { 'id', 'SerialNumber', 'ProductSku', 'Date' }");

            parameters =
                new SearchParameters()
                {
                    Filter = "Id eq '1'" ,
                    Select = new[] { "Id", "SerialNumber", "ProductSku", "Date"}
                };

            results = _indexClient.Documents.Search("*", parameters);

            WriteDocuments(results);

            Console.Write("ProductFamilyId eq 1 -> { 'id', 'SerialNumber', 'ProductSku', 'ProductFamilyId', 'Date' }");
            Console.WriteLine("and return the hotelId and description:\n");

            parameters =
                new SearchParameters()
                {
                    Filter = "ProductFamilyId eq 1",
                    Select = new[] { "Id", "SerialNumber", "ProductSku", "ProductFamilyId", "Date" }
                };

            results = _indexClient.Documents.Search("*", parameters);           

            WriteDocuments(results);
          
        }

        private static void WriteDocuments(DocumentSearchResult searchResults)
        {
            foreach (SearchResult result in searchResults.Results)
            {
                Console.ForegroundColor = Console.ForegroundColor == ConsoleColor.Green ? ConsoleColor.White : ConsoleColor.Green;
                Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(result.Document));
            }
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
        }


    }
}
