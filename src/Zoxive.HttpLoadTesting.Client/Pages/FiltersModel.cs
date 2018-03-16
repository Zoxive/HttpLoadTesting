using Zoxive.HttpLoadTesting.Client.Framework.Model;

namespace Zoxive.HttpLoadTesting.Client.Pages
{
    public class FiltersModel
    {
        public FiltersModel(Filters filters, HttpStatusResultDistincts distincts)
        {
            Filters = filters;
            Distincts = distincts;
        }

        public HttpStatusResultDistincts Distincts { get; }
        public Filters Filters { get; }
    }
}
