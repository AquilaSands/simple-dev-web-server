using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace DevWebServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            bool isDev = string.Equals(
                Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
                EnvironmentName.Development,
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

            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .ConfigureKestrel(options => options.AddServerHeader = false);
    }
}
