using System.Data;
using Dapper;

namespace Zoxive.HttpLoadTesting.Client.Domain.Database
{
    public static class DbInitializer
    {
        // TODO would be nice to have a dotnetcore compatible migrator.
        public static void Initialize(IDbConnection dbConnection)
        {
            dbConnection.Open();

            dbConnection.Execute("DROP TABLE IF EXISTS Iteration;");
            dbConnection.Execute("DROP TABLE IF EXISTS HttpStatusResult;");

            dbConnection.Execute(@"
CREATE TABLE Iteration (
    Id         INTEGER PRIMARY KEY AUTOINCREMENT,
    UserNumber INTEGER,
    Iteration  INTEGER,
    StartTick  BIGINT,
    EndTick    BIGINT,
    UserDelay  BIGINT,
    Exception  VARCHAR,
    DidError   BOOLEAN,
    BaseUrl    VARCHAR,
    Elapsed    BIGINT,
    TestName   VARCHAR
);
");

            dbConnection.Execute(@"
CREATE TABLE HttpStatusResult (
    Id                  INTEGER PRIMARY KEY,
    IterationId         INTEGER REFERENCES Iteration (Id) ON DELETE CASCADE,
    Method              VARCHAR,
    ElapsedMilliseconds REAL, 
    RequestUrl          VARCHAR,
    StatusCode          INTEGER,
    RequestStartTick    BIGINT
);
");
        }

        private static bool IterationTableExists(IDbConnection dbConnection)
        {
            return dbConnection.QueryFirstOrDefault<bool>("SELECT 1 FROM sqlite_master WHERE type='table' AND name='Iteration'");
        }
    }
}
