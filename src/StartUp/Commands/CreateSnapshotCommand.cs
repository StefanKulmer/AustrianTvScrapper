using AustrianTvScrapper.Services;
using System;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Linq;

namespace AustrianTvScrapper.StartUp.Commands
{
    internal class CreateSnapshotCommand : Command
    {
        private readonly IOrfTvSeriesSnapshotService snapshotService;

        public CreateSnapshotCommand(IOrfTvSeriesSnapshotService snapshotService)
            : base("createSnapshot", "creates a snapshot")
        {
            this.snapshotService = snapshotService;

            AddArgument(new Argument<string>("channel", getDefaultValue: () => "Orf"));

            Handler = CommandHandler.Create<string>(_HandleCommand);
        }

        private void _HandleCommand(string channel)
        {
            snapshotService.CreateSnapshot();
        }
    }
}
