// Copyright (c) .NET Core Community. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;

// ReSharper disable once CheckNamespace
namespace DotNetCore.CAP
{
    public class EFOptions
    {
        public const string DefaultSchema = "CAP";

        /// <summary>
        /// Gets or sets the schema to use when creating database objects.
        /// Default is <see cref="DefaultSchema" />.
        /// </summary>
        public string Schema { get; set; } = DefaultSchema;

        /// <summary>
        /// CAP 表的前缀
        /// </summary>
        public string TableNamePrefix { get; set; }

        internal Type? DbContextType { get; set; }

        /// <summary>
        /// Data version
        /// </summary>
        internal string Version { get; set; } = default!;
    }
}