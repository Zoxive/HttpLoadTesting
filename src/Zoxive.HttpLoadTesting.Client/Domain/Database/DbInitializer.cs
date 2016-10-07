namespace Zoxive.HttpLoadTesting.Client.Domain.Database
{
    public static class DbInitializer
    {
        public static void Initialize(IterationsContext context)
        {
            context.Database.EnsureCreated();

            // TODO Migrations?
            //context.Database.Migrate();
        }
    }
}
