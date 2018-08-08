using System.Data;
using Dapper;

namespace Zoxive.HttpLoadTesting.Client.Domain.Database
{
    public static class DbInitializer
    {
        // TODO would be nice to have a dotnetcore compatible migrator.
        public static void Initialize(IDbWriter db, string @namespace = "main")
        {
            var dbConnection = db.Connection;

            dbConnection.Open();

            dbConnection.Execute("PRAGMA read_uncommitted = true;");

            if (IterationTableExists(dbConnection, @namespace))
                return;

            dbConnection.Execute($"DROP TABLE IF EXISTS {@namespace}.Iteration;");
            dbConnection.Execute($"DROP TABLE IF EXISTS {@namespace}.HttpStatusResult;");

            dbConnection.Execute($@"
CREATE TABLE {@namespace}.Iteration (
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
");

            dbConnection.Execute($@"
CREATE TABLE {@namespace}.HttpStatusResult (
    Id                  INTEGER PRIMARY KEY,
    IterationId         INTEGER,
    Method              VARCHAR,
    ElapsedMilliseconds REAL, 
    RequestUrl          VARCHAR,
    StatusCode          INTEGER,
    RequestStartedMs    REAL
);
");
        }

        private static bool IterationTableExists(IDbConnection dbConnection, string ns)
        {
            return dbConnection.QueryFirstOrDefault<bool>($"SELECT 1 FROM {ns}.sqlite_master WHERE type='table' AND name='Iteration'");
        }
    }
}
