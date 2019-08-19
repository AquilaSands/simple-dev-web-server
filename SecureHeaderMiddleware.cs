using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DevWebServer
{
    public class SecureHeaderMiddleware
    {
        private readonly RequestDelegate _next;

        public SecureHeaderMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public static IDictionary<string, string> SecurityHeaders { get; } = new Dictionary<string, string>
        {
            {"x-frame-options", "deny"},
            {"x-xss-protection", "1; mode=block"},
            {"x-content-type-options", "nosniff"}
        };

        public Task InvokeAsync(HttpContext context)
        {

            context.Response.OnStarting((state) =>
            {

                if (state is HttpResponse response && response != null)
                {
                    var headers = response.Headers;

                    foreach (var headerValuePair in SecurityHeaders)
                    {
                        headers[headerValuePair.Key] = headerValuePair.Value;
                    }

                    // foreach (var header in result.RemoveHeaders)
                    // {
                    //     headers.Remove(header);
                    // }
                }

                return Task.CompletedTask;
            }, context.Response);

            // Call the next delegate/middleware in the pipeline
            return _next(context);
        }
    }
}
