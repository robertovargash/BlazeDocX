using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using BlazeDocX.Services;
using Blazored.LocalStorage;
using BlazeDocX;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Xceed.Words.NET;
using Xceed.Document.NET;
using Toolbelt.Blazor.Extensions.DependencyInjection;
using System.Globalization;

Xceed.Document.NET.Licenser.LicenseKey = "WDN23-B1ZHC-PKYEY-L4HA";
Xceed.Workbooks.NET.Licenser.LicenseKey = "WBN13-2SKLF-RUWUK-C41A";

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");

builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();

builder.Services.AddSingleton<ProfileCreator>();
builder.Services.AddSingleton<CVCreator>();
//builder.Services.AddSingleton<AccountingManager>();
builder.Services.AddSingleton<WorkBookCreator>();

builder.Services.AddScoped<StorageManager>();
builder.Services.AddI18nText(option => option.PersistanceLevel = Toolbelt.Blazor.I18nText.PersistanceLevel.SessionAndLocal);
//builder.Services.AddI18nText<Program>();


builder.Services.AddBlazoredLocalStorage(config =>
{
    config.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
    config.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    config.JsonSerializerOptions.IgnoreReadOnlyProperties = true;
    config.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    config.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    config.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    config.JsonSerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
    config.JsonSerializerOptions.WriteIndented = false;
});

builder.Services.AddScoped<ThemeManager>();

var app = builder.Build();

await app.RunAsync();