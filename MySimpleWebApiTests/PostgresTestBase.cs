using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Marten;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;

namespace MySimpleWebApiTests
{
    [TestClass]
    public class PostgresTestBase
    {
        private static string PostgresOutPort { get; } = "5433";
        private static string PostgresInPort { get; } = "5432";
        private static string TestUrl { get; } = $"http://localhost:{PostgresOutPort}/ping";
        private const string ImageName = "PostgreSQL_test";
        private const string DbName = "values_test";
        private const string Username = "dbuser";
        private const string Password = "dbpwd";

        private static readonly TimeSpan TestTimeout = TimeSpan.FromSeconds(180);
        private static Process _process;

        protected static string ConnectionString { get; } = $"host=localhost;port={PostgresOutPort};database={DbName};username={Username};password={Password}";

        [TestInitialize]
        public void StartPostgres()
        {
            if (_process == null)
            {
                _process = Process.Start("docker",
                    $"run --name {ImageName} -e POSTGRES_USER={Username} -e POSTGRES_PASSWORD={Password} -e POSTGRES_DB={DbName} -p {PostgresOutPort}:{PostgresInPort} postgres:alpine");

                var started = WaitForContainer().Result;
                if (!started)
                {
                    throw new Exception($"Startup failed, could not get '{TestUrl}' after trying for '{TestTimeout}'");
                }
            }

            Environment.SetEnvironmentVariable("DB_CONNECTION_STRING", ConnectionString);
        }

        private async Task<bool> WaitForContainer()
        {
            var startTime = DateTime.Now;
            while (DateTime.Now - startTime < TestTimeout)
            {
                try
                {
                    using (var conn = new NpgsqlConnection(ConnectionString))
                    {
                        conn.Open();
                        if (conn.State == System.Data.ConnectionState.Open)
                        {
                            return true;
                        }
                    }
                }
                catch
                {
                    // Ignore exceptions, just retry
                }

                await Task.Delay(1000).ConfigureAwait(false);
            }

            return false;
        }

        [TestCleanup]
        public void DeleteAllData()
        {
            var store = DocumentStore.For(ConnectionString);

            store.Advanced.Clean.CompletelyRemoveAll();
        }

        [AssemblyCleanup]
        public static void StopPostgres()
        {
            if (_process != null)
            {
                _process.Dispose();
                _process = null;
            }

            var processStop = Process.Start("docker", $"stop {ImageName}");
            processStop.WaitForExit();
            var processRm = Process.Start("docker", $"rm {ImageName}");
            processRm.WaitForExit();
        }
    }
}
