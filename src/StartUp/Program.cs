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
            var services = new ServiceCollection();
            IConfigurationRoot config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            services.AddSingleton<IConfiguration>(config);
            services.AddCliCommands();
            services.AddOrfTvSeriesCommands();
            services.AddSubscription();
            services.AddDownloadListCreator();

            return services.BuildServiceProvider();
        }
    }
}
