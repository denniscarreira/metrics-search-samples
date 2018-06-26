using Metrics.Domain;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AzureSearch.Init
{
    class Program
    {
        private const string SalesIndexName = "salesindex";
        private static ISearchServiceClient _searchClient;
        private static ISearchIndexClient _indexClient;
        static void Main(string[] args)
        {
            string searchServiceName ="callisto-poc";
            string apiKey = "F1B07470817299CF607C8A4A83D055EF";

            // Create an HTTP reference to the catalog index
            _searchClient = new SearchServiceClient(searchServiceName, new SearchCredentials(apiKey));
            _indexClient = _searchClient.Indexes.GetClient(SalesIndexName);

            CreateOrUpdateIndex();
            ImportSales();

        }


        private static void CreateOrUpdateIndex()
        {
            // Create the Azure Search index based on the included schema
            try
            {
                var definition = new Index()
                {
                    Name = SalesIndexName,
                    Fields = new[]
                    {
                        new Field("Id",   DataType.String)         { IsKey = true, IsSearchable = false,  IsFilterable = true,  IsSortable = true,  IsFacetable = false, IsRetrievable = true},
                        new Field("Date",   DataType.DateTimeOffset)         { IsKey = false, IsSearchable = false,  IsFilterable = true,  IsSortable = true,  IsFacetable = false, IsRetrievable = true},
                        new Field("WarrantyId",  DataType.Int32)         { IsKey = false, IsSearchable = false,  IsFilterable = true,  IsSortable = true,  IsFacetable = false, IsRetrievable = true},
                        new Field("RetailerId",    DataType.Int32)         { IsKey = false, IsSearchable = false,  IsFilterable = true,  IsSortable = true,  IsFacetable = false, IsRetrievable = true},
                        new Field("SellerId",  DataType.String)          { IsKey = false, IsSearchable = false, IsFilterable = true,  IsSortable = true,  IsFacetable = true,  IsRetrievable = true},
                        new Field("ProductSku",    DataType.String)         { IsKey = false, IsSearchable = true,  IsFilterable = true,  IsSortable = true,  IsFacetable = false, IsRetrievable = true},
                        new Field("ProductName", DataType.String)          { IsKey = false, IsSearchable = true, IsFilterable = true,  IsSortable = true,  IsFacetable = true,  IsRetrievable = true},
                        new Field("ProductFamilyId",      DataType.Int32)          { IsKey = false, IsSearchable = false, IsFilterable = true,  IsSortable = true,  IsFacetable = true,  IsRetrievable = true},
                        new Field("WarrantyCardCode",     DataType.String)          { IsKey = false, IsSearchable = true, IsFilterable = true,  IsSortable = true,  IsFacetable = true,  IsRetrievable = true},
                        new Field("SerialNumber",       DataType.String)         { IsKey = false, IsSearchable = true,  IsFilterable = true,  IsSortable = true,  IsFacetable = false, IsRetrievable = true},
                    }
                };

                _searchClient.Indexes.CreateOrUpdate(definition);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error creating index: {0}\r\n", ex.Message);
            }

        }

        private static void ImportSales()
        {
            var productFilePath = @"C:\Projects\WAS\Postman\Seed\Data\ORIS\products.json";
            var productList = new List<Product>();
            using (StreamReader file = File.OpenText(productFilePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                productList = (List<Product>)serializer.Deserialize(file, typeof(List<Product>));
            }
            var rnd = new Random();
            var salesList = new List<object>();
            int counter = 0;
            for (int i = 0; i < 10000; i++)
            {
                var product = productList.FirstOrDefault(p => p.Id == rnd.Next(1, productList.Count())) ?? productList.First();

                salesList.Add(new
                {
                    Id = i.ToString(),
                    Date = DateTime.Now.AddYears(rnd.Next(-3, 0)).AddMonths(rnd.Next(-12, 0)).AddDays(rnd.Next(-30, 0)).AddHours(rnd.Next(-24, 0)),
                    ProductFamilyId =  product.Id,
                    ProductName = product.Name,
                    ProductSku = product.Sku,
                    SerialNumber = rnd.Next(99999999, 999999999).ToString(),
                    RetailerId = rnd.Next(0, 200),
                    SellerId = Guid.NewGuid().ToString(),
                    WarrantyId = rnd.Next(0, 999999999),
                    WarrantyCardCode = rnd.Next(1000, 999999999).ToString()
                });
                counter++;
                if(counter == 1000)
                {
                    var batch = IndexBatch.Upload(salesList);
                    _indexClient.Documents.Index(batch);
                    counter = 0;
                    salesList = new List<object>();
                }
            }

            if (salesList.Any())
            {
                var batch = IndexBatch.Upload(salesList);
                _indexClient.Documents.Index(batch);
            }
        }
    }
}
