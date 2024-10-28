using DotNetCore.CAP.Persistence;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using DotNetCore.CAP;
using System.Data;
using Dm;

namespace DotNetCore.CAP.Dameng
{
    public class DamengStorageInitializer : IStorageInitializer
    {
        private readonly ILogger _logger;
        private readonly IOptions<DamengOptions> _options;

        public DamengStorageInitializer(
            ILogger<DamengStorageInitializer> logger,
            IOptions<DamengOptions> options)
        {
            _options = options;
            _logger = logger;
        }

        public virtual string GetPublishedTableName()
        {
            return $"\"{_options.Value.Schema}\".\"{_options.Value.TableNamePrefix}Published\"";
        }

        public virtual string GetReceivedTableName()
        {
            return $"\"{_options.Value.Schema}\".\"{_options.Value.TableNamePrefix}Received\"";
        }

        public async Task InitializeAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested) return;

            var sql = CreateDbTablesScript(_options.Value.Schema);
            await using (var connection = new DmConnection(_options.Value.ConnectionString))
                connection.ExecuteNonQuery(sql);

            _logger.LogDebug("Ensuring all create database tables script are applied.");
        }

        protected virtual string CreateDbTablesScript(string schema)
        {
            var batchSql = $@"
BEGIN
  IF NOT EXISTS (SELECT 1 FROM SYSOBJECTS Where TYPE$ = 'SCH' And Name = '{schema}') THEN
    EXECUTE IMMEDIATE 'CREATE SCHEMA {schema}';
  END IF;

  IF NOT EXISTS (SELECT 1 FROM all_tables WHERE table_name = '{_options.Value.TableNamePrefix}Received' AND owner = '{schema}') THEN
    EXECUTE IMMEDIATE '
        CREATE TABLE IF NOT EXISTS {GetReceivedTableName()}(
          ""Id"" BIGINT PRIMARY KEY NOT NULL,
          ""Version"" VARCHAR(20) NOT NULL,
          ""Name"" VARCHAR(200) NOT NULL,
          ""Group"" VARCHAR(200) NULL,
          ""Content"" CLOB NULL,
          ""Retries"" INT NOT NULL,
          ""Added"" TIMESTAMP NOT NULL,
          ""ExpiresAt"" TIMESTAMP NULL,
          ""StatusName"" VARCHAR(50) NOT NULL
        )';
  END IF;

  IF NOT EXISTS (SELECT 1 FROM all_tables WHERE table_name = '{_options.Value.TableNamePrefix}Published' AND owner = '{schema}') THEN
    EXECUTE IMMEDIATE '
        CREATE TABLE IF NOT EXISTS {GetPublishedTableName()}(
          ""Id"" BIGINT PRIMARY KEY NOT NULL,
          ""Version"" VARCHAR(20) NOT NULL,
          ""Name"" VARCHAR(200) NOT NULL,
          ""Content"" CLOB NULL,
          ""Retries"" INT NOT NULL,
          ""Added"" TIMESTAMP NOT NULL,
          ""ExpiresAt"" TIMESTAMP NULL,
          ""StatusName"" VARCHAR(50) NOT NULL
        )';
  END IF;
END;
";
            return batchSql;
        }
    }
}