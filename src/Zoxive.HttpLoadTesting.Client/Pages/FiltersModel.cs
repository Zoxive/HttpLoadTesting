using Zoxive.HttpLoadTesting.Client.Framework.Model;

namespace Zoxive.HttpLoadTesting.Client.Pages
{
    public class FiltersModel
    {
        public FiltersModel(HttpStatusResultDistincts distincts, Filters filters, bool showCollationType)
        {
            Distincts = distincts;
            Filters = filters;
            ShowCollationType = showCollationType;
        }

        public HttpStatusResultDistincts Distincts { get; }
        public Filters Filters { get; }
        public bool ShowCollationType { get; }
    }
}