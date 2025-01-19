using Neo4j.Driver;

namespace FitnessCentar.Email.Persistence
{
    public class EmailDbContext
    {
        private readonly IDriver _driver;

        public EmailDbContext(IConfiguration configuration)
        {
            _driver = GraphDatabase.Driver(
                configuration["Neo4j:Uri"],
                AuthTokens.Basic(configuration["Neo4j:Username"], configuration["Neo4j:Password"])
            );
        }

        public async Task ExecuteWriteAsync(string query, object parameters)
        {
            var session = _driver.AsyncSession();
            try
            {
                await session.WriteTransactionAsync(tx => tx.RunAsync(query, parameters));
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public async Task<IReadOnlyList<IRecord>> ExecuteReadAsync(string query, object parameters)
        {
            var session = _driver.AsyncSession();
            try
            {
                var result = await session.ReadTransactionAsync(tx => tx.RunAsync(query, parameters));
                return await result.ToListAsync();
            }
            finally
            {
                await session.CloseAsync();
            }
        }
    }
}
