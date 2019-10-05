using System;
using System.Diagnostics;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace DevWebServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            bool isDev = string.Equals(
                Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
                Environments.Development,
                StringComparison.OrdinalIgnoreCase);

            if (!isDev)
            {
                // Manually set the current directory because when run from a context menu shortcut
                // the content directory can be c:\windows\system32 which seems a tad unsafe
                var pathToDll = "";
                foreach (ProcessModule module in Process.GetCurrentProcess().Modules)
                {
                    if (string.Equals(module.ModuleName, "devwebserver.dll", StringComparison.OrdinalIgnoreCase))
                    {
                        pathToDll = module.FileName;
                        break;
                    }
                }

                if (string.IsNullOrWhiteSpace(pathToDll))
                {
                    throw new InvalidOperationException("Can't get the devwebserver.dll module path.");
                }

                var pathToContentRoot = Path.GetDirectoryName(pathToDll);
                Directory.SetCurrentDirectory(pathToContentRoot);
            }

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(serverOptions =>
                    {
                        // Set properties and call methods on options
                        serverOptions.AddServerHeader = false;
                    })
                    .UseStartup<Startup>();
                });
    }
}
