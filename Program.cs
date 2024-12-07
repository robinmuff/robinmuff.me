using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using static System.String;

// Webapp builder
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// All paths required for the webapp
var executionPath = Directory.GetCurrentDirectory();
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

    var path = staticGlobalFolder + httpContext.Request.Path.Value + ".html";
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
app.MapGet("/favicon", async (HttpContext httpContext) => await httpContext.Response.SendFileAsync(executionPath + "/" + staticGlobalFolder + "/assets/image/project-robinmuff.me.png"));
app.MapGet("/api/{item}", async (string item, HttpContext httpContext) =>
{
    await WriteResponse(httpContext, GetInfoValueByKey(item));
});
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

    key = Join('-', 
        key.Split('-').Select(word =>
            IsNullOrEmpty(word) ? word : char.ToUpper(word[0]) + word[1..]
            ).ToArray());

    string value = json[key].ToString();
    
    if (!value.Contains("[[") || !value.Contains("]]")) return value;
    
    // Get text from value between { and }
    var startIndex = value.IndexOf("[[", StringComparison.Ordinal) + 2;
    var endIndex = value.IndexOf("]]", StringComparison.Ordinal);
    var variable = value[startIndex..endIndex];

    value = value.Replace("[[" + variable + "]]", GetVariableData(variable));

    return value;
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