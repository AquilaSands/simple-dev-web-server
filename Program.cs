using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DevWebServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Manually set the current directory because when run from a context menu shortcut
            // the content directory can be c:\windows\system32 which seems a tad unsafe
            var pathToDll = "";
            foreach (ProcessModule module in Process.GetCurrentProcess().Modules)
            {
                if (string.Equals(module.ModuleName, "devwebserver.dll", StringComparison.OrdinalIgnoreCase))
                {
                    pathToDll = module.FileName;
                }
            }

            if (string.IsNullOrWhiteSpace(pathToDll))
            {
                throw new InvalidOperationException("Can't get the devwebserver.dll module path.");
            }

            var pathToContentRoot = Path.GetDirectoryName(pathToDll);
            Directory.SetCurrentDirectory(pathToContentRoot);

            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .ConfigureKestrel(options => options.AddServerHeader = false);
    }
}
