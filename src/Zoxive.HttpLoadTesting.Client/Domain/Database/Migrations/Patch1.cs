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

                return string.Empty;
            }

            if (tableExists)
            {
                return string.Empty;
            }

            CreateNewSchema(dbCommandFactory);

            return string.Empty;
        }

        private void MigrateToNewSchema(long? frequency, Func<IDbCommand> dbCommandFactory)
        {
            ExecuteSql("ALTER TABLE HttpStatusResult ADD RequestStartedMs REAL DEFAULT 0 NOT NULL", dbCommandFactory);

            ExecuteSql($@"
            UPDATE HttpStatusResult SET RequestStartedMs = 
            (
              SELECT
                     (r.RequestStartTick - min.min) / ({frequency} / 1000.0) as RequestStartedMs
              FROM HttpStatusResult r
              JOIN (SELECT MIN(RequestStartTick) as min FROM HttpStatusResult) min
              WHERE r.Id = HttpStatusResult.Id
            )", dbCommandFactory);

            ExecuteSql(@"
            CREATE TABLE HttpStatusResult0440
            (
                Id INTEGER PRIMARY KEY,
                IterationId INTEGER,
                Method VARCHAR,
                ElapsedMilliseconds REAL,
                RequestUrl VARCHAR,
                StatusCode INTEGER,
                RequestStartedMs REAL DEFAULT 0 NOT NULL
            );
            INSERT INTO HttpStatusResult0440(Id, IterationId, Method, ElapsedMilliseconds, RequestUrl, StatusCode, RequestStartedMs) SELECT Id, IterationId, Method, ElapsedMilliseconds, RequestUrl, StatusCode, RequestStartedMs FROM HttpStatusResult;
            DROP TABLE HttpStatusResult;
            ALTER TABLE HttpStatusResult0440 RENAME TO HttpStatusResult;
            ", dbCommandFactory);

            ExecuteSql(@"
            ALTER TABLE Iteration ADD ElapsedMs REAL DEFAULT 0 NOT NULL;
            ALTER TABLE Iteration ADD StartedMs REAL DEFAULT 0 NOT NULL;
            ", dbCommandFactory);

            ExecuteSql($@"
            UPDATE Iteration Set StartedMs =
            (
             SELECT
                  (i.StartTick - min.min) / ({frequency} / 1000.0) as StartedMs
            FROM Iteration i
            JOIN (SELECT MIN(StartTick) as min FROM Iteration) min
            WHERE i.Id = Iteration.Id
            ), ElapsedMs =
            (
             SELECT
                (i.EndTick - i.StartTick) / ({frequency} / 1000.0) as ElapsedMs
            FROM Iteration i
            JOIN (SELECT MIN(StartTick) as min FROM Iteration) min
            WHERE i.Id = Iteration.Id
            )
            ", dbCommandFactory);

            ExecuteSql(@";
            CREATE TABLE Iterationac0e
            (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                UserNumber INTEGER,
                Iteration INTEGER,
                UserDelay BIGINT,
                Exception VARCHAR,
                DidError BOOLEAN,
                BaseUrl VARCHAR,
                TestName VARCHAR,
                ElapsedMs REAL DEFAULT 0 NOT NULL,
                StartedMs REAL DEFAULT 0 NOT NULL
            );
            INSERT INTO Iterationac0e(Id, UserNumber, Iteration, UserDelay, Exception, DidError, BaseUrl, TestName, ElapsedMs, StartedMs) SELECT Id, UserNumber, Iteration, UserDelay, Exception, DidError, BaseUrl, TestName, ElapsedMs, StartedMs FROM Iteration;
            DROP TABLE Iteration;
            ALTER TABLE Iterationac0e RENAME TO Iteration;
            ", dbCommandFactory);

            ExecuteSql("VACUUM", dbCommandFactory);
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
