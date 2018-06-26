using Metrics.Domain;
using Nest;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace Elasticsearch.Init
{
    public class Program
    {
        static ElasticClient _elasticClient;
        static List<ProductFamiliy> _productFamilies;
        static List<Product> _products;
        static void Main(string[] args)
        {
            var node = new Uri("https://6d1d9f4026f8414bb9898ef72930ae03.eu-west-1.aws.found.io:9243");
            var settings = new ConnectionSettings(node);
            _elasticClient = new ElasticClient(settings.BasicAuthentication("elastic", "eSMBdXy6kvE3avUOrCUARqtW"));
            LoadProductFamilies();
            LoadProducts();
            ImportSales();
        }

        static void ImportSales()
        {
            var rnd = new Random();
            var sales = new List<Sale>();
            int count = 0;
            for (int i = 0; i < 2000000; i++)
            {
                var product = _products.FirstOrDefault(p => p.Id == rnd.Next(1, _products.Count())) ?? _products.First();

                sales.Add(new Sale
                {
                    Id = i,
                    Date = DateTime.Now.AddYears(rnd.Next(-3, 1)).AddMonths(rnd.Next(-12, 1)).AddDays(rnd.Next(-30, 1)).AddHours(rnd.Next(-24, 1)),
                    Distribution = i % 2 == 0? "website":"mobile",
                    IsReturned = i % 100 == 0,
                    ProductFamilies = product.ProductFamilies,
                    ProductName = product.Name,
                    ProductSku = product.Sku,
                    SerialNumber = rnd.Next(99999999, 999999999).ToString(),
                    RetailerId = rnd.Next(0, 200),
                    SellerId = new Guid("00000000-0000-0000-0000-0000000" + rnd.Next(10000, 99999).ToString())
                });
                count++;
                if (count == 10000)
                {
                    var resp = _elasticClient.Bulk(b => b
                    .IndexMany(sales, (d, sale) => d.Document(sale).Index("mysaleindex")));
                    count = 0;
                    sales = new List<Sale>();
                }
            }
            
        }

        static void LoadProducts()
        {
            var productFilePath = @"C:\Projects\WAS\Postman\Seed\Data\ORIS\products.json";
            _products = new List<Product>();
            using (StreamReader file = File.OpenText(productFilePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                _products = (List<Product>)serializer.Deserialize(file, typeof(List<Product>));
            }

            foreach(var product in _products)
            {
                product.ProductFamilies = GetProductFamilies(product.ProductFamilyId).Split(':');
            }
        }

        static string GetProductFamilies(int familyId)
        {
            var family = _productFamilies.First(f=>f.Id == familyId);
            if(family.ParentId != null)
            {
                return family.Name + ':'+ GetProductFamilies(family.ParentId.Value);
            }
            return family.Name;
        }
        static void LoadProductFamilies()
        {
            var productFamiliesFilePath = @"C:\Projects\WAS\Postman\Seed\Data\ORIS\productFamiliesCustom.json";
            _productFamilies = new List<ProductFamiliy>();
            using (StreamReader file = File.OpenText(productFamiliesFilePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                _productFamilies = (List<ProductFamiliy>)serializer.Deserialize(file, typeof(List<ProductFamiliy>));
            }
        }
    }
}
