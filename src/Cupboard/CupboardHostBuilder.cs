using System;
using System.Collections.Generic;
using Cupboard.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.IO;

namespace Cupboard
{
    public sealed class CupboardHostBuilder
    {
        private readonly List<Action<IHostBuilder>> _configurations;
        private readonly IAnsiConsole _console;
        private bool _propagateExceptions = false;

        public CupboardHostBuilder(IAnsiConsole? console = null)
        {
            _console = console ?? AnsiConsole.Console;
            _configurations = new List<Action<IHostBuilder>>
            {
                builder => builder.ConfigureServices(services =>
                {
                    services.AddAll<Manifest>();

                    services.AddModule<ResourcesModule>();
                    services.AddModule<MacOsModule>();
                    services.AddModule<WindowsModule>();

                    services.AddSingleton<IAnsiConsole>(_console);
                    services.AddSingleton<IPlatform, Platform>();
                    services.AddSingleton<ICupboardFileSystem, CupboardFileSystem>();
                    services.AddSingleton<ICupboardEnvironment, CupboardEnvironment>();

                    services.AddSingleton<IWindowsRegistry, WindowsRegistry>();

                    services.AddSingleton<IProcessRunner, ProcessRunner>();
                    services.AddSingleton<IEnvironmentRefresher, EnvironmentRefresher>();
                    services.AddSingleton<IFactBuilder, FactBuilder>();
                    services.AddSingleton<ISecurityPrincipal, SecurityPrincipal>();
                    services.AddSingleton<IRebootDetector, RebootDetector>();

                    services.AddSingleton<ICupboardLogger, CupboardLogger>();
                    services.AddSingleton<ExecutionEngine>();
                    services.AddSingleton<ExecutionPlanBuilder>();
                    services.AddSingleton<ReportRenderer>();
                    services.AddSingleton<FactBuilder>();
                    services.AddSingleton<ResourceProviderRepository>();

                    services.AddCommandLine<RunCommand>(config =>
                    {
                        config.ConfigureConsole(_console);
                        config.PropagateExceptions();

                        config.AddCommand<FactCommand>("facts");
                    });
                }),
            };
        }

        public CupboardHostBuilder PropagateExceptions()
        {
            _propagateExceptions = true;
            return this;
        }

        public CupboardHostBuilder AddCatalog<TCatalog>()
            where TCatalog : Catalog
        {
            _configurations.Add(b => b.ConfigureServices(s => s.AddSingleton<Catalog, TCatalog>()));
            return this;
        }

        public CupboardHostBuilder ConfigureServices(Action<IServiceCollection> services)
        {
            _configurations.Add(b => b.ConfigureServices(services));
            return this;
        }

        public CupboardHost Build()
        {
            var builder = Host.CreateDefaultBuilder();
            _configurations.ForEach(action => action(builder));

            var host = builder.Build();
            var console = host.Services.GetRequiredService<IAnsiConsole>();

            return new CupboardHost(console, host, _propagateExceptions);
        }

        public int Run(string[] args)
        {
            var host = Build();
            return host.Run(args);
        }
    }
}
