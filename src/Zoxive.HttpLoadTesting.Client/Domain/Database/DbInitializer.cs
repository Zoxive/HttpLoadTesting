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
        }

        private static bool IterationTableExists(IDbConnection dbConnection)
        {
            return dbConnection.QueryFirstOrDefault<bool>("SELECT 1 FROM sqlite_master WHERE type='table' AND name='Iteration'");
        }
    }
}
