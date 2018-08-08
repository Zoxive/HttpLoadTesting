using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace Zoxive.HttpLoadTesting.Client.Framework
{
    public class SimpleTransaction : ISimpleTransaction
    {
        private readonly DbConnection _dbConnection;

        public SimpleTransaction(IDbWriter dbConnection)
        {
            _dbConnection = (DbConnection)dbConnection.Connection;
        }

        public Task OpenConnection()
        {
            return _dbConnection.State != ConnectionState.Open ? _dbConnection.OpenAsync() : Task.CompletedTask;
        }

        public Task Begin()
        {
            return RawExecuteAsync("BEGIN TRANSACTION");
        }

        public Task Commit()
        {
            return RawExecuteAsync("COMMIT TRANSACTION");
        }

        public Task Rollback()
        {
            return RawExecuteAsync("ROLLBACK TRANSACTION");
        }

        private async Task RawExecuteAsync(string sql)
        {
            using (var cmd = _dbConnection.CreateCommand())
            {
                cmd.CommandText = sql;

                await cmd.ExecuteNonQueryAsync();
            }
        }
    }

    public interface ISimpleTransaction
    {
        Task OpenConnection();

        Task Begin();

        Task Commit();

        Task Rollback();
    }
}
