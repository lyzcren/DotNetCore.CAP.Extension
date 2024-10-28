// Copyright (c) .NET Core Community. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using DotNetCore.CAP.Dameng;
using DotNetCore.CAP.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

// ReSharper disable once CheckNamespace
namespace DotNetCore.CAP
{
    internal class DamengCapOptionsExtension : ICapOptionsExtension
    {
        private readonly Action<DamengOptions> _configure;

        public DamengCapOptionsExtension(Action<DamengOptions> configure)
        {
            _configure = configure;
        }

        public void AddServices(IServiceCollection services)
        {
            services.AddSingleton<CapStorageMarkerService>();
            services.Configure(_configure);
            services.AddSingleton<IConfigureOptions<DamengOptions>, ConfigureDamengOptions>();

            services.AddSingleton<IDataStorage, DamengDataStorage>();
            services.AddSingleton<IStorageInitializer, DamengStorageInitializer>();
        }
    }
}