using Metrics.Domain;
using Nest;
using System;

namespace Elasticsearch
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Init ElasticSearch Client");
            var node = new Uri("https://6d1d9f4026f8414bb9898ef72930ae03.eu-west-1.aws.found.io:9243");
            var settings = new ConnectionSettings(node)
                .DefaultIndex("mysaleindex")
                .BasicAuthentication("elastic", "eSMBdXy6kvE3avUOrCUARqtW");

            var client = new ElasticClient(settings);

            Console.WriteLine("Get  one document wit id == 1 ");
            Console.WriteLine("\nPress any key to continue");
            Console.ReadKey();

            var response2 = client.Search<Sale>(s => s
            .From(0)
            .Size(10)
            .Query(q =>
                    q.Term(t => t.Id, 1)
                )
            );

            ShowResult(response2);


            Console.WriteLine("\nGet documents wit ProductFamilyId == 'Diving'  (TOP 10)");
            Console.WriteLine("\nPress any key to continue");
            Console.ReadKey();

            var response3 = client.Search<Sale>(s => s
            .From(0)
            .Size(100)
            .Query(q=>q.Match(c=>c
                .Query("Diving")
                .Field(f=>f.ProductFamilies))
                )
            );

            ShowResult(response3);

            Console.WriteLine("\nGet document aggregated by day");
            Console.WriteLine("\nPress any key to continue");
            Console.ReadKey();


            var response4 = client.Search<Sale>(s => s
           .From(0)
           //.Size(100)
          .Aggregations(r => r.DateHistogram("my_date_histogram", h => h
            .Field(p => p.Date)
            .Interval("30d")
                 ))
           );

            ShowDataAggregation(response4);

            Console.WriteLine("\nGet  one document wit ProductFamilyId == 'Oris Aquis Date Diamonds' aggregated by day");
            Console.WriteLine("\nPress any key to continue");
            Console.ReadKey();
            var response5 = client.Search<Sale>(s => s
            .From(0)
            .Query(
                q => q.Match(c => c
                .Query("Oris Aquis Date Diamonds")
                .Field(f => f.ProductFamilies))
            )
            .Aggregations(r => r.DateHistogram("my_date_histogram", h => h
            .Field(p => p.Date)
            .Interval("1d")
                 ))
            );

            ShowDataAggregation(response5);

            Console.WriteLine("\nPress any key to exit");
            Console.ReadKey();
        }

        static void ShowResult(ISearchResponse<Sale> resp)
        {
            Console.WriteLine("Documents: ");
            foreach (var doc in resp.Documents)
            {
                Console.WriteLine();
                Console.ForegroundColor = Console.ForegroundColor == ConsoleColor.Green ? ConsoleColor.White : ConsoleColor.Green;
                Console.Write(Newtonsoft.Json.JsonConvert.SerializeObject(doc));
            }
            Console.ForegroundColor = ConsoleColor.White;
        }
        static void ShowDataAggregation(ISearchResponse<Sale> resp)
        {
            Console.WriteLine("Aggregations: ");
            foreach (var agg in resp.Aggregations.DateHistogram("my_date_histogram").Buckets)
            {
                Console.WriteLine();
                Console.ForegroundColor = Console.ForegroundColor == ConsoleColor.Green ? ConsoleColor.White : ConsoleColor.Green;
                Console.Write(agg.KeyAsString);
                Console.Write(" -> ");
                Console.Write(agg.DocCount);
                Console.WriteLine();
            }
        }

        static void ShowDataAggregationDateRange(ISearchResponse<Sale> resp)
        {
            Console.WriteLine("Aggregations: ");
            foreach (var agg in resp.Aggregations.DateRange("my_date_range").Buckets)
            {
                Console.WriteLine();
                Console.ForegroundColor = Console.ForegroundColor == ConsoleColor.Green ? ConsoleColor.White : ConsoleColor.Green;
                //Console.Write(agg.);
                Console.Write(" -> ");
                Console.Write(agg.DocCount);
                Console.WriteLine();
            }
        }
    }
}
