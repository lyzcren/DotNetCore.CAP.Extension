// Copyright (c) .NET Core Community. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using DotNetCore.CAP.Persistence;
using DotNetCore.CAP.OpenGauss;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

// ReSharper disable once CheckNamespace
namespace DotNetCore.CAP
{
    internal class OpenGaussCapOptionsExtension : ICapOptionsExtension
    {
        private readonly Action<OpenGaussOptions> _configure;

        public OpenGaussCapOptionsExtension(Action<OpenGaussOptions> configure)
        {
            _configure = configure;
        }

        public void AddServices(IServiceCollection services)
        {
            services.AddSingleton<CapStorageMarkerService>();
            services.Configure(_configure);
            services.AddSingleton<IConfigureOptions<OpenGaussOptions>, ConfigureOpenGaussOptions>();

            services.AddSingleton<IDataStorage, OpenGaussDataStorage>();
            services.AddSingleton<IStorageInitializer, OpenGaussStorageInitializer>();
        }
    }
}