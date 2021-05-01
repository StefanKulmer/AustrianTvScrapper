using AustrianTvScrapper.Services;
using System;

namespace AustrianTvScrapper.StartUp
{
    class Program
    {
        static void Main(string[] args)
        {
            var scrapper = new OrfTvSeriesScrapper();
            var tvSeries = scrapper.GetListOfTvSeries();
            foreach (var item in tvSeries)
            {
                Console.WriteLine($"{item.Title} ({item.Id})");
                Console.WriteLine($"\t{item.Url}");
                Console.WriteLine($"\t{item.Description}");
                Console.WriteLine($"\t{item.Channel}");
                Console.WriteLine($"\t{item.Profile}");
            }
        }
    }
}
