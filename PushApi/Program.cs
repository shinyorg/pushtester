using PushApi;
using Shiny.Extensions.Push;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var appleCfg = builder.Configuration.GetSection("Push:Apple");
var googleCfg = builder.Configuration.GetSection("Push:Google");
builder.Services.AddPushManagement(x => x
    .AddApple(new AppleConfiguration
    {
        AppBundleIdentifier = appleCfg["AppBundleIdentifier"]!,
        TeamId = appleCfg["TeamId"]!,
        Key = appleCfg["Key"]!,
        KeyId = appleCfg["KeyId"]!,
        IsProduction = appleCfg["Production"] == "true"
    })
    .AddGoogleFirebase(new GoogleConfiguration
    {
        ServerKey = googleCfg["ServerKey"]!,
        SenderId = googleCfg["SenderId"]!,
        DefaultChannelId = googleCfg["DefaultChannelId"]!
    })
    // .UseAdoNetRepository<SqliteConnection>(new DbRepositoryConfig(
    //     "Data Source=shiny.db",
    //     "@",
    //     "PushRegistrations",
    //     true
    // ))
    .AddShinyAndroidClickAction()
);

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.MapPushEndpoints("push", false);

app.RegisterEndpoints();
app.Run();