using IF.Infrastructure.IntegrationTests.Fixtures;

namespace IF.Infrastructure.IntegrationTests.Helper
{
    [CollectionDefinition("SequentialTests", DisableParallelization = true)]
    public class SequentialTestsCollection : ICollectionFixture<DatabaseFixture>
    {

    }
}
