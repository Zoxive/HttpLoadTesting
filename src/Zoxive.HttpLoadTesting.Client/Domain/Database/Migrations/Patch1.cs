using System;
using System.Data;
using DbUp.Engine;

namespace Zoxive.HttpLoadTesting.Client.Domain.Database.Migrations
{
    public class Patch1 : IScript
    {
        public static long? Frequency;

        public string ProvideScript(Func<IDbCommand> dbCommandFactory)
        {
            var (isOld, tableExists) = Get(dbCommandFactory);

            if (isOld)
            {
                if (!Frequency.HasValue)
                {
                    throw new Exception("Please add --frequency to migrate this older database");
                }

                MigrateToNewSchema(Frequency, dbCommandFactory);
            }
            else if (tableExists)
            {
                return string.Empty;
            }

            CreateNewSchema(dbCommandFactory);

            return string.Empty;
        }

        private void MigrateToNewSchema(long? frequency, Func<IDbCommand> dbCommandFactory)
        {
            throw new NotImplementedException();
        }

        private void CreateNewSchema(Func<IDbCommand> dbCommandFactory)
        {
            const string createIteration = @"
CREATE TABLE main.Iteration (
    Id         INTEGER PRIMARY KEY AUTOINCREMENT,
    UserNumber INTEGER,
    Iteration  INTEGER,
    UserDelay  BIGINT,
    Exception  VARCHAR,
    DidError   BOOLEAN,
    BaseUrl    VARCHAR,
    ElapsedMs  REAL,
    StartedMs  REAL,
    TestName   VARCHAR
);
";
            ExecuteSql(createIteration, dbCommandFactory);

            const string createStatusResult = @"
CREATE TABLE main.HttpStatusResult (
    Id                  INTEGER PRIMARY KEY,
    IterationId         INTEGER,
    Method              VARCHAR,
    ElapsedMilliseconds REAL, 
    RequestUrl          VARCHAR,
    StatusCode          INTEGER,
    RequestStartedMs    REAL
);
";
            ExecuteSql(createStatusResult, dbCommandFactory);
        }

        private static void ExecuteSql(string sql, Func<IDbCommand> dbCommandFactory)
        {
            using (var cmd = dbCommandFactory())
            {
                cmd.CommandText = sql;

                cmd.ExecuteNonQuery();
            }
        }

        private static (bool isOld, bool tableExists) Get(Func<IDbCommand> cmdFactory)
        {
            var tableExists = false;

            using (var cmd = cmdFactory())
            {
                cmd.CommandText = "PRAGMA table_info('Iteration')";

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tableExists = true;
                        for (var i = 0; i < reader.FieldCount; i++)
                        {
                            var field = reader.GetName(i);
                            if (field == "name")
                            {
                                var value = reader.GetString(i);
                                if (value == "Elapsed")
                                {
                                    return (true, true);
                                }
                            }
                        }
                    }
                }
            }

            return (false, tableExists);
        }
    }
}
