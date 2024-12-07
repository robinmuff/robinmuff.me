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
app.MapGet("/", async (HttpContext httpContext) => await ReturnStartPage(httpContext));

app.MapGet("/api/structure", async (HttpContext httpContext) => await WriteResponse(httpContext, GetInfoValueByKey("Structure")));

app.MapGet("/api/favicon", async (HttpContext httpContext) => await httpContext.Response.SendFileAsync(executionPath + "/" + staticGlobalFolder + "/assets/image/project-robinmuff.me.png"));

app.MapGet("/api/document-title", async (HttpContext httpContext) => await WriteResponse(httpContext, GetInfoValueByKey("Name")));

app.MapGet("/api/name", async (HttpContext httpContext) => await WriteResponse(httpContext, GetInfoValueByKey("Name")));
app.MapGet("/api/description", async (HttpContext httpContext) => await WriteResponse(httpContext, GetInfoValueByKey("Description")));
app.MapGet("/api/socials", async (HttpContext httpContext) => await WriteResponse(httpContext, GetSocials()));
app.MapGet("/api/downloadcv", async (HttpContext httpContext) => await WriteResponse(httpContext, GetInfoValueByKey("DownloadCv")));

app.MapGet("/api/home-title", async (HttpContext httpContext) => await WriteResponse(httpContext, GetInfoValueByKey("Home-Title")));
app.MapGet("/api/home-text", async (HttpContext httpContext) => await WriteResponse(httpContext, GetInfoValueByKey("Home-Text")));

app.MapGet("/api/about-title", async (HttpContext httpContext) => await WriteResponse(httpContext, GetInfoValueByKey("About-Title")));
app.MapGet("/api/about-texts", async (HttpContext httpContext) => await WriteResponse(httpContext, GetInfoValueByKey("About-Texts")));
app.MapGet("/api/about-skills-title", async (HttpContext httpContext) => await WriteResponse(httpContext, GetInfoValueByKey("About-Skills-Title")));
app.MapGet("/api/about-skills", async (HttpContext httpContext) => await WriteResponse(httpContext, GetInfoValueByKey("About-Skills")));
app.MapGet("/api/about-languages-title", async (HttpContext httpContext) => await WriteResponse(httpContext, GetInfoValueByKey("About-Languages-Title")));
app.MapGet("/api/about-languages", async (HttpContext httpContext) => await WriteResponse(httpContext, GetInfoValueByKey("About-Languages")));

app.MapGet("/api/projects-title", async (HttpContext httpContext) => await WriteResponse(httpContext, GetInfoValueByKey("Projects-Title")));
app.MapGet("/api/projects", async (HttpContext httpContext) => await WriteResponse(httpContext, GetInfoValueByKey("Projects")));
app.MapGet("/api/projects-more", async (HttpContext httpContext) => await WriteResponse(httpContext, GetInfoValueByKey("Projects-More")));

app.MapGet("/api/experience-title", async (HttpContext httpContext) => await WriteResponse(httpContext, GetInfoValueByKey("Experience-Title")));
app.MapGet("/api/experience", async (HttpContext httpContext) => await WriteResponse(httpContext, GetInfoValueByKey("Experience")));

app.MapGet("/api/contact-title", async (HttpContext httpContext) => await WriteResponse(httpContext, GetInfoValueByKey("Contact-Title")));

app.MapGet("/api/template-socials", async (HttpContext httpContext) => await WriteResponse(httpContext, GetInfoValueByKey("Template-Socials")));
app.MapGet("/api/template-about-texts", async (HttpContext httpContext) => await WriteResponse(httpContext, GetInfoValueByKey("Template-About-Texts")));
app.MapGet("/api/template-about-skills", async (HttpContext httpContext) => await WriteResponse(httpContext, GetInfoValueByKey("Template-About-Skills")));
app.MapGet("/api/template-about-languages", async (HttpContext httpContext) => await WriteResponse(httpContext, GetInfoValueByKey("Template-About-Languages")));
app.MapGet("/api/template-projects", async (HttpContext httpContext) => await WriteResponse(httpContext, GetInfoValueByKey("Template-Projects")));
app.MapGet("/api/template-experience", async (HttpContext httpContext) => await WriteResponse(httpContext, GetInfoValueByKey("Template-Experience")));

app.Run();

// Functions for the routes
async Task ReturnStartPage(HttpContext httpContext)
{
    await httpContext.Response.SendFileAsync(executionPath + "/" + staticGlobalFolder + "/index.html");
}
async Task WriteResponse(HttpContext httpContext, string response)
{
    await httpContext.Response.WriteAsync(response);
}

// Functions for the infos.json
string GetInfoValueByKey(string key)
{
    if (json == null) return "";

    string value = json[key].ToString();

    // Get text from value between { and }
    if (value.Contains("[[") && value.Contains("]]"))
    {
        int startIndex = value.IndexOf("[[", StringComparison.Ordinal) + 2;
        int endIndex = value.IndexOf("]]", StringComparison.Ordinal);
        string variable = value[startIndex..endIndex];

        value = value.Replace("[[" + variable + "]]", GetVariableData(variable));
    }

    return value;
}
string GetSocials()
{
    return json == null ? "" : (string)JsonConvert.SerializeObject(json.Socials);
}

// Functions for changing data
string GetVariableData(string key)
{
    return key == "age" ? GetAge() : "";
}
string GetAge() 
{
    var today = DateTime.Today;
    var birthdate = new DateTime(2003, 09, 26);
    var age = today.Year - birthdate.Year;

    if (birthdate.Date > today.AddYears(-age)) age--;
    return age.ToString();
}