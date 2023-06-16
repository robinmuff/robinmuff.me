using System.Net.Mail;
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

app.MapGet("/aboutme/name", async (HttpContext httpContext) => await writeResponse(httpContext, getInfoValueByKey("Name")));
app.MapGet("/aboutme/description", async (HttpContext httpContext) => await writeResponse(httpContext, getInfoValueByKey("Description")));
app.MapGet("/aboutme/socials", async (HttpContext httpContext) => await writeResponse(httpContext, getSocials()));
app.MapGet("/aboutme/home-title", async (HttpContext httpContext) => await writeResponse(httpContext, getInfoValueByKey("Home-Title")));
app.MapGet("/aboutme/about-title", async (HttpContext httpContext) => await writeResponse(httpContext, getInfoValueByKey("About-Title")));
app.MapGet("/aboutme/about-texts", async (HttpContext httpContext) => await writeResponse(httpContext, getInfoValueByKey("About-Texts")));
app.MapGet("/aboutme/about-skills-title", async (HttpContext httpContext) => await writeResponse(httpContext, getInfoValueByKey("About-Skills-Title")));
app.MapGet("/aboutme/about-skills", async (HttpContext httpContext) => await writeResponse(httpContext, getInfoValueByKey("About-Skills")));
app.MapGet("/aboutme/about-languages-title", async (HttpContext httpContext) => await writeResponse(httpContext, getInfoValueByKey("About-Languages-Title")));
app.MapGet("/aboutme/about-languages", async (HttpContext httpContext) => await writeResponse(httpContext, getInfoValueByKey("About-Languages")));

app.MapGet("/aboutme/experience-title", async (HttpContext httpContext) => await writeResponse(httpContext, getInfoValueByKey("Experience-Title")));
app.MapGet("/aboutme/experience", async (HttpContext httpContext) => await writeResponse(httpContext, getInfoValueByKey("Experience")));

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

// Functions for the infos.json
string getInfoValueByKey(string key)
{
    if (json == null) return "";

    return json[key].ToString();
}
string getSocials()
{
    if (json == null) return "";

    return JsonConvert.SerializeObject(json.Socials);
}