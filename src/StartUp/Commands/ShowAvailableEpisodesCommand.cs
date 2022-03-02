﻿using AustrianTvScrapper.Services;
using System;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Linq;

namespace AustrianTvScrapper.StartUp.Commands
{
    internal class ShowAvailableEpisodesCommand : Command
    {
        private readonly IOrfTvSeriesScrapper orfTvSeriesScrapper;

        public ShowAvailableEpisodesCommand(IOrfTvSeriesScrapper orfTvSeriesScrapper)
            : base("getSubscribedEpisodes", "gets all epsiodes for the subscriptions")
        {
            this.orfTvSeriesScrapper = orfTvSeriesScrapper;

            AddArgument(new Argument<string>("channel", getDefaultValue: () => "Orf"));

            Handler = CommandHandler.Create<string>(_HandleCommand);
        }

        private void _HandleCommand(string channel)
        {
            var tvSeries = orfTvSeriesScrapper.GetListOfTvSeries();

            var subscriptionService = new OrfTvSeriesSubscriptionService(new UserDocumentsDataDirectoryProvider());
            var subscriptions = subscriptionService.GetSubscriptions();

            var episodeProvider = new OrfTvSeriesEpisodesProvider();

            foreach (var subscription in subscriptions)
            {
                var tvSeriesItem = tvSeries.FirstOrDefault(x => x.Id == subscription.OrfTvSeriesId);
                if (tvSeriesItem == null)
                {
                    continue;
                }

                var episodes = episodeProvider.GetAvailableEpisodes(tvSeriesItem);

                foreach (var episode in episodes)
                {
                    Console.WriteLine($"{tvSeriesItem.Title}:{episode.Date} {episode.Title}");
                }
            }
        }
    }
}