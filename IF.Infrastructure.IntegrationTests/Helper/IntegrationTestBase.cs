using IF.Infrastructure.BankingRepository;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace IF.Infrastructure.IntegrationTests.Helper
{
    public class IntegrationTestBase : IDisposable
    {
        protected readonly IConfiguration Configuration;
        private readonly BankingContext _context;

        public BankingContext Context { get; }

        public IntegrationTestBase()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Integration.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();

            _context = new BankingContext(Configuration);
            Context = _context;
        }

        public void Dispose()
        {
            _context.Connection.Close();
            _context.Connection.Dispose();
        }
    }
}
