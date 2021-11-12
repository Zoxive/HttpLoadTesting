using Zoxive.HttpLoadTesting.Client.Framework.Model;

namespace Zoxive.HttpLoadTesting.Client.Pages
{
    public class FiltersModel
    {
        public FiltersModel()
        {
            Distincts = HttpStatusResultDistincts.Empty;
            Filters = Filters.Empty;
            ShowCollationType = false;
        }

        public FiltersModel(HttpStatusResultDistincts? distincts, Filters? filters, bool showCollationType)
        {
            Distincts = distincts ?? HttpStatusResultDistincts.Empty;
            Filters = filters ?? Filters.Empty;
            ShowCollationType = showCollationType;
        }

        public HttpStatusResultDistincts Distincts { get; }
        public Filters Filters { get; }
        public bool ShowCollationType { get; }
    }
}