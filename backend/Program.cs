using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);
var staticFilePath = "static";

var app = builder.Build();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Environment.CommandLine.Replace('\\', '/').Remove(Environment.CommandLine.Replace('\\', '/').LastIndexOf('/')) + "/" + staticFilePath),
    RequestPath = ""
});

app.MapGet("/", async (HttpContext httpContext) => { await httpContext.Response.SendFileAsync(staticFilePath + "/index.html"); });

app.Use(async (httpContext, next) =>
{
    await next();
    if (httpContext.Response.StatusCode == 404)
    {
        if (File.Exists(staticFilePath + httpContext.Request.Path.Value + ".html"))
        {
            await httpContext.Response.SendFileAsync(staticFilePath + httpContext.Request.Path.Value + ".html");
        }
        else
        {
            // Error HTML
            httpContext.Response.Redirect("/");
        }
    }
});

app.Run();