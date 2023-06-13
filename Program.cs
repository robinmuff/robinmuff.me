using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;

// Webapp builder
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
WebApplication? app = builder.Build();

// All paths required for the webapp
string executionPath = Directory.GetCurrentDirectory();
const string staticFolder = "static";
const string staticGlobalFolder = staticFolder + "/www";

// Read infos.json
var json = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(staticFolder + "/infos.json"))!;

// Configure Static Files
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(executionPath + "/" + staticGlobalFolder),
    RequestPath = ""
});

// Configure the Middleware
app.Use(async (httpContext, next) =>
{
    await next();

    if (httpContext.Response.StatusCode != 404) return;

    string path = staticGlobalFolder + httpContext.Request.Path.Value + ".html";
    if (File.Exists(path))
    {
        await httpContext.Response.SendFileAsync(path);
        return;
    }

    // Redirect to start page if 404
    httpContext.Response.Redirect("/");
});

// Map the routes
app.MapGet("/", async (HttpContext httpContext) => await returnStartPage(httpContext));
app.MapGet("/myname", async (HttpContext httpContext) => await writeResponse(httpContext, getName()));
app.MapGet("/mydescription", async (HttpContext httpContext) => await writeResponseAsJson(httpContext, getDescription()));
app.MapGet("/mylinks", async (HttpContext httpContext) => await writeResponse(httpContext, getLinks()));

app.Run();

// Functions for the routes
async Task returnStartPage(HttpContext httpContext)
{
    await httpContext.Response.SendFileAsync(executionPath + "/" + staticGlobalFolder + "/index.html");
}
async Task writeResponse(HttpContext httpContext, string response) 
{
    await httpContext.Response.WriteAsync(response);
}
async Task writeResponseAsJson(HttpContext httpContext, List<string> response) 
{
    await httpContext.Response.WriteAsJsonAsync(response);
}

// Functions for the infos.json
string getName() 
{
    if (json == null) return "";

    return json.Name;
}
List<string> getDescription() 
{
    if (json == null) return new List<string>();

    List<string> description = new List<string>();

    for (int i = 0; i < json.Description.Count; i++) 
    {
        description.Add(json.Description[i].Value);
    }
    return description;
}
string getLinks() 
{
    if (json == null) return "";

    return JsonConvert.SerializeObject(json.Links);
}