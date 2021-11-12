using System;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Zoxive.HttpLoadTesting.Client.Domain.Database.Migrations;

namespace Zoxive.HttpLoadTesting.Framework.Model
{
    public class ClientOptions
    {
        public string DatabaseFile { get; }

        public bool StopApplicationWhenComplete { get; }

        public bool Viewing { get; }

        public ClientOptions(string? databaseFile = null, bool? stopAppWhenComplete = false)
        {

            if (string.IsNullOrWhiteSpace(databaseFile))
            {
                var now = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
                DatabaseFile = $"Test_{now}.db";
            }
            else
            {
                DatabaseFile = databaseFile;
                Viewing = true;
            }

            StopApplicationWhenComplete = stopAppWhenComplete ?? false;
        }

        public static ClientOptions FromArgs(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddCommandLine(args)
                .Build();

            Patch1.Frequency = config.GetValue<long?>("frequency");
            return new ClientOptions(config.GetValue<string?>("databaseFile"));
        }
    }
}