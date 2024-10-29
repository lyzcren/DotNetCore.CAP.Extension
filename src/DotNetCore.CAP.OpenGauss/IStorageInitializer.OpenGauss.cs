// Copyright (c) .NET Core Community. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using DotNetCore.CAP.Persistence;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;

namespace DotNetCore.CAP.OpenGauss
{
    public class OpenGaussStorageInitializer : IStorageInitializer
    {
        private readonly ILogger _logger;
        private readonly IOptions<OpenGaussOptions> _options;

        public OpenGaussStorageInitializer(
            ILogger<OpenGaussStorageInitializer> logger,
            IOptions<OpenGaussOptions> options)
        {
            _options = options;
            _logger = logger;
        }

        public virtual string GetPublishedTableName()
        {
            return $"\"{_options.Value.Schema}\".\"published\"";
        }

        public virtual string GetReceivedTableName()
        {
            return $"\"{_options.Value.Schema}\".\"received\"";
        }

        public async Task InitializeAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested) return;

            var sql = CreateDbTablesScript(_options.Value.Schema);
            await using (var connection = new NpgsqlConnection(_options.Value.ConnectionString))
                connection.ExecuteNonQuery(sql);

            _logger.LogDebug("Ensuring all create database tables script are applied.");
        }

        protected virtual string CreateDbTablesScript(string schema)
        {
            var batchSql = $@"
DO
$$
BEGIN
    IF NOT EXISTS (SELECT schema_name FROM information_schema.schemata WHERE schema_name = '{schema}') THEN
        CREATE SCHEMA ""{schema}"";
    END IF;
END
$$;

CREATE TABLE IF NOT EXISTS {GetReceivedTableName()}(
	""Id"" BIGINT PRIMARY KEY NOT NULL,
    ""Version"" VARCHAR(20) NOT NULL,
	""Name"" VARCHAR(200) NOT NULL,
	""Group"" VARCHAR(200) NULL,
	""Content"" TEXT NULL,
	""Retries"" INT NOT NULL,
	""Added"" TIMESTAMP NOT NULL,
    ""ExpiresAt"" TIMESTAMP NULL,
	""StatusName"" VARCHAR(50) NOT NULL
);

CREATE TABLE IF NOT EXISTS {GetPublishedTableName()}(
	""Id"" BIGINT PRIMARY KEY NOT NULL,
    ""Version"" VARCHAR(20) NOT NULL,
	""Name"" VARCHAR(200) NOT NULL,
	""Content"" TEXT NULL,
	""Retries"" INT NOT NULL,
	""Added"" TIMESTAMP NOT NULL,
    ""ExpiresAt"" TIMESTAMP NULL,
	""StatusName"" VARCHAR(50) NOT NULL
);
";
            return batchSql;
        }
    }
}