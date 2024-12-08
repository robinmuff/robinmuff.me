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
var jsonContent = File.ReadAllText(staticFolder + "/infos.json");
var json = JsonConvert.DeserializeObject<dynamic>(jsonContent) ?? throw new Exception("Invalid JSON format or file not found.");


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
app.MapGet("/", async (HttpContext httpContext) => await httpContext.Response.SendFileAsync(executionPath + "/" + staticGlobalFolder + "/index.html"));
app.MapGet("/favicon", async (HttpContext httpContext) => await httpContext.Response.SendFileAsync(executionPath + "/" + staticGlobalFolder + "/assets/image/project-robinmuff.me.png"));
app.MapGet("/api/{item}", async (string item, HttpContext httpContext) => await httpContext.Response.WriteAsync(GetInfoValueByKey(item)));
app.Run();
return;

// Functions for the infos.json
string GetInfoValueByKey(string key)
{
    if (json == null) return string.Empty;

    key = string.Join('-', key.Split('-').Select(word => 
        string.IsNullOrEmpty(word) ? word : char.ToUpper(word[0]) + word.Substring(1)));

    if (json[key] == null) return string.Empty;

    string value = json[key].ToString();
    if (!value.Contains("[[") || !value.Contains("]]")) return value;

    var variable = value.Substring(value.IndexOf("[[", StringComparison.Ordinal) + 2, value.IndexOf("]]", StringComparison.Ordinal) - value.IndexOf("[[", StringComparison.Ordinal) - 2);
    return value.Replace($"[[{variable}]]", GetVariableData(variable));
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