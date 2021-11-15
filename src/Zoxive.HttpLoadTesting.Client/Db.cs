using System.Data.Common;

namespace Zoxive.HttpLoadTesting
{
    public class Db : IDbWriter, IDbReader
    {
        public Db(DbConnection connection)
        {
            Connection = connection;
        }

        public DbConnection Connection { get; }
    }

    public interface IDbWriter
    {
        DbConnection Connection { get; }
    }

    public interface IDbReader
    {
        DbConnection Connection { get; }
    }
}