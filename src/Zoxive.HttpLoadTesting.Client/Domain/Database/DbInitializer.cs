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

            if (IterationTableExists(dbConnection))
            {
                dbConnection.Execute("DELETE FROM Iteration");
                dbConnection.Execute("delete from sqlite_sequence where name='Iteration'");

                dbConnection.Execute("DELETE FROM HttpStatusResult");
                dbConnection.Execute("delete from sqlite_sequence where name='HttpStatusResult'");

                dbConnection.Execute("Vacuum");

                return;
            }

            dbConnection.Execute(@"
CREATE TABLE Iteration (
    Id         INTEGER PRIMARY KEY AUTOINCREMENT,
    UserNumber INTEGER,
    Iteration  INTEGER,
    StartTick  INTEGER,
    EndTick    INTEGER,
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
    StatusCode          INTEGER
);
");
        }

        private static bool IterationTableExists(IDbConnection dbConnection)
        {
            return dbConnection.QueryFirstOrDefault<bool>("SELECT 1 FROM sqlite_master WHERE type='table' AND name='Iteration'");
        }
    }
}
