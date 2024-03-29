﻿using System;
using Dapper;
using DbUp;
using Zoxive.HttpLoadTesting.Client.Domain.Database.Migrations;

namespace Zoxive.HttpLoadTesting.Client.Domain.Database
{
    public static class DbInitializer
    {
        public static void Initialize(IDbWriter db)
        {
            var dbConnection = db.Connection;

            dbConnection.Open();

            dbConnection.Execute("PRAGMA read_uncommitted = true;");

            var migrator = DeployChanges.To
                .SQLiteDatabase(db.Connection.ConnectionString)
                .WithScriptsAndCodeEmbeddedInAssembly(typeof(Patch1).Assembly, s => s.StartsWith("Zoxive.HttpLoadTesting.Client.Domain.Database.Migrations", StringComparison.Ordinal))
                .WithTransaction()
                .LogToConsole()
                .Build();

            var wasPatched = migrator.IsUpgradeRequired();

            var result = migrator.PerformUpgrade();

            if (result.Successful && wasPatched)
            {
                db.Connection.Execute("VACUUM");
            }

            if (!result.Successful)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(result.Error);
                Console.ResetColor();

                throw new Exception("Migration Failed");
            }
        }
    }
}
