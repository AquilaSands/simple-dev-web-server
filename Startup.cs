using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DevWebServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<FileServerConfig>(Configuration.GetSection("FileServerConfig"));
            services.AddOptions();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,
            ILogger<Startup> logger, IOptions<FileServerConfig> configAccessor)
        {
            app.UseDeveloperExceptionPage();

            bool isKestrelHost = string.Equals(Process.GetCurrentProcess().ProcessName, "dotnet", StringComparison.OrdinalIgnoreCase);
            if (isKestrelHost)
            {
                app.UseMiddleware<SecureHeaderMiddleware>();
            }

            // Provide default favicon if one is not specified in the html to prevent 404
            // This will probably prevent using a favicon with the default 'favicon.ico' name
            app.Use(async (context, next) =>
            {
                if (context.Request.Path.Equals("/favicon.ico", StringComparison.OrdinalIgnoreCase))
                {
                    var fileInfo = env.ContentRootFileProvider.GetFileInfo("favicon.ico");

                    if (fileInfo.Exists)
                    {
                        HttpResponse response = context.Response;
                        response.StatusCode = StatusCodes.Status200OK;
                        response.ContentType = "image/vnd.microsoft.icon";
                        await response.SendFileAsync(fileInfo);
                        return;
                    }
                }

                await next();
            });

            var config = configAccessor.Value;

            Console.WriteLine($"Serving files from path {config.BaseDirectory}");
            Console.WriteLine($"Directory browsing is {(config.DirectoryBrowsing ? "enabled" : "disabled")}");

            var fsOptions = new FileServerOptions
            {
                EnableDirectoryBrowsing = config.DirectoryBrowsing,
                FileProvider = new PhysicalFileProvider(config.BaseDirectory)
            };
            fsOptions.StaticFileOptions.ServeUnknownFileTypes = true;
            fsOptions.StaticFileOptions.DefaultContentType = "text/plain";

            app.UseFileServer(fsOptions);
        }
    }
}
