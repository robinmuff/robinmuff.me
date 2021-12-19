using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);
var executionPath = Environment.CommandLine.Replace('\\', '/').Remove(Environment.CommandLine.Replace('\\', '/').LastIndexOf('/'));
var staticFilePath = "static";

var app = builder.Build();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(executionPath + "/" + staticFilePath),
    RequestPath = ""
});

app.MapGet("/", async (HttpContext httpContext) => { await httpContext.Response.SendFileAsync(executionPath + "/" + staticFilePath + "/index.html"); });

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

app.MapGet("/myname", async (HttpContext httpContext) =>
{
    await httpContext.Response.WriteAsync("Robin Muff");
});

app.MapGet("/mydescription", async (HttpContext httpContext) =>
{
    await httpContext.Response.WriteAsJsonAsync(new List<string>() { "Apprentice Computer Scientist", "Programming", "Never stop trying your best" });
});

app.MapGet("/mylinks", async (HttpContext httpContext) =>
{
    await httpContext.Response.SendFileAsync(staticFilePath + "/links.json");
});

app.Run();