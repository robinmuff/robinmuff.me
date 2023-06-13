using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;

// Webapp builder
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
WebApplication? app = builder.Build();

// All paths required for the webapp
string executionPath = Directory.GetCurrentDirectory();
const string staticFolder = "static";
const string staticSecureFolder = staticFolder + "/secure";
const string staticGlobalFolder = staticFolder + "/www";

// All API Keys
const string API_KEY_NAME = "X-API-Key";
string API_KEY_VALUE = File.ReadAllText(staticSecureFolder + "/api.key");

// Read infos.json
var json = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(staticFolder + "/infos.json"))!;

// Configure Static Files with Key
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(executionPath + "/" + staticSecureFolder),
    RequestPath = "/secure",
    OnPrepareResponse = ctx => checkForApiKey(ctx.Context)
});
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

// API Key Validator
void checkForApiKey(HttpContext ctx)
{
    if (isApiKeyValid(ctx))
    {
        returnUnauthorized(ctx);
        //returnNotfound(ctx);
    }
}
bool isApiKeyValid(HttpContext ctx)
{
    return !(isHeaderApiKeyValid(ctx) || isParamApiKeyValid(ctx));
}
bool isHeaderApiKeyValid(HttpContext ctx)
{
    var apiKey = ctx.Request.Headers.ToList().Find(item => item.Key == API_KEY_NAME);

    if (apiKey.Key == null) return false;
    return apiKey.Value == API_KEY_VALUE;
}
bool isParamApiKeyValid(HttpContext ctx)
{
    string? key = ctx.Request.Query[API_KEY_NAME];

    if (key == null) return false;
    return key == API_KEY_VALUE;
}
void returnUnauthorized(HttpContext ctx)
{
    ctx.Response.StatusCode = 401;
    ctx.Response.ContentLength = 0;
    ctx.Response.Body = Stream.Null;
}
/*
void returnNotfound(HttpContext ctx)
{
    ctx.Response.StatusCode = 404;
    ctx.Response.ContentLength = 0;
    ctx.Response.Body = Stream.Null;
}
*/