// <copyright file="Program.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace SystemCommandLine.Demo
{
    using System.CommandLine;
    using System.CommandLine.Builder;
    using System.CommandLine.Parsing;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Subscription;
    using DownloadListCreator;
    using System.IO;
    using System;
    using System.Text.Json;
    using Downloader.Services;
    using Microsoft.Extensions.Options;
    internal static class Program
    {
        /// <summary>
        /// The entry point for the program.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>When complete, an integer representing success (0) or failure (non-0).</returns>
        public static async Task<int> Main(string[] args)
        {
            ServiceProvider serviceProvider = BuildServiceProvider();
            Parser parser = BuildParser(serviceProvider);

            return await parser.InvokeAsync(args).ConfigureAwait(false);
        }

        private static Parser BuildParser(ServiceProvider serviceProvider)
        {
            var rootCommand = new RootCommand("Austrian TV Series Scrapper");

            foreach (Command command in serviceProvider.GetServices<Command>())
            {
                rootCommand.AddCommand(command);
            }

            var commandLineBuilder = new CommandLineBuilder(rootCommand);

            return commandLineBuilder.UseDefaults().Build();
        }

        private static ServiceProvider BuildServiceProvider()
        {
            string userDataDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string appDataDir = Path.Combine(userDataDir, "AustrianTvScrapper");
            string optionsFilePath = Path.Combine(appDataDir, "appsettings.json");

            if (!File.Exists(optionsFilePath))
            {
                Console.WriteLine("Options file not found. Creating default options file.");
                Directory.CreateDirectory(appDataDir);
                var defaultOptions = new DirectoryOptions { 
                    SubscriptionsDirectory = appDataDir,
                    DownloadListDirectory = appDataDir,
                    DownloadDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "AustrianTvScrapper")
                };
                File.WriteAllText(optionsFilePath, JsonSerializer.Serialize(defaultOptions));
            }

            // Build configuration
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(optionsFilePath)
                .Build();

            var services = new ServiceCollection();

            services.Configure<DirectoryOptions>(config.GetSection("DirectoryOptions"));
            services.AddCliCommands();
            services.AddOrfTvSeriesCommands();
            services.AddSubscription();
            services.AddDownloadListCreator();

            return services.BuildServiceProvider();
        }
    }
}
