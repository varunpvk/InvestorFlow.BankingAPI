using IF.Infrastructure.IntegrationTests.Helper;

namespace IF.Infrastructure.IntegrationTests.Fixtures
{
    public class DatabaseFixture : IDisposable
    {
        public SqliteTestHelper TestHelper { get; private set; }

        public DatabaseFixture()
        {
            TestHelper = new SqliteTestHelper();
        }

        public void Dispose()
        {
           TestHelper.Dispose();
        }
    }
}
