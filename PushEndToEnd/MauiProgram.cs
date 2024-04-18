#if ANDROID
using Android.App;
#endif
using PushTesting.Delegates;
using PushTesting.Services;
using PushTesting.Services.Impl;
using Refit;
using Shiny.Push;

namespace PushTesting;


public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
        => MauiApp
            .CreateBuilder()
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseShinyFramework(
                new DryIocContainerExtension(),
                prism => prism.CreateWindow((_, nav) => nav
                    .CreateBuilder()
                    .AddTabbedSegment(tabs => tabs
                        .CreateTab(page => page
                            .AddNavigationPage()
                            .AddSegment(nameof(MainPage))
                        )
                        .CreateTab(page => page
                            .AddNavigationPage()
                            .AddSegment(nameof(EventsPage))
                        )
                    )
                ),
                new(ErrorAlertType.FullError)
            )
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            })
            .RegisterInfrastructure()
            .RegisterViews()
            .RegisterPush()
            .Build();


    static MauiAppBuilder RegisterInfrastructure(this MauiAppBuilder builder)
    {
        builder.Configuration.AddJsonPlatformBundle();
#if DEBUG
        builder.Logging.SetMinimumLevel(LogLevel.Trace);
        builder.Logging.AddDebug();
#endif
        builder.Services.AddSingleton<AppSqliteConnection>();
        builder.Services.AddSingleton(sp =>
        {
            var baseUri = sp.GetRequiredService<IConfiguration>()["NativeUri"] ?? "https://localhost";
            return RestService.For<IApiClient>(baseUri);
        });

        return builder;
    }


    static MauiAppBuilder RegisterViews(this MauiAppBuilder builder)
    {
        builder.Services.RegisterForNavigation<MainPage, MainViewModel>();
        builder.Services.RegisterForNavigation<EventsPage, EventsViewModel>();
        return builder;
    }


    static MauiAppBuilder RegisterPush(this MauiAppBuilder builder)
    {
        var provider = builder.Configuration["PushProvider"]?.ToLower();

        switch (provider)
        {
            case "azurenotificationhubs":
                RegisterAzure(builder);
                break;

            case "firebase":
                RegisterFirebase(builder);
                break;
            
            case "native":
                RegisterNative(builder);
                break;

            default:
                throw new InvalidOperationException("Invalid Push Provider - " + provider);
        }
        return builder;
    }


    static void RegisterAzure(MauiAppBuilder builder)
    {
        builder.Services.AddSingleton<IPushSender, AzureNotificationHubPushSender>();

        builder.Services.AddPushAzureNotificationHubs<MyPushDelegate>(
            builder.Configuration["AzureNotificationHubs:ListenerConnectionString"]!,
            builder.Configuration["AzureNotificationHubs:HubName"]!
#if ANDROID
            , new FirebaseConfig(
                false,
                builder.Configuration["Firebase:AndroidAppId"],
                builder.Configuration["Firebase:ProjectNumber"],
                builder.Configuration["Firebase:ProjectId"],
                builder.Configuration["Firebase:ApiKey"],
                DefaultChannel
            )
#endif
        );
    }


    static void RegisterFirebase(MauiAppBuilder builder)
    {
        builder.Services.AddSingleton<IPushSender, FirebasePushSender>();

        builder.Services.AddPushFirebaseMessaging<MyPushDelegate>(
            new FirebaseConfiguration(
                false,
#if IOS
                builder.Configuration["Firebase:AppleAppId"],
#elif ANDROID
                builder.Configuration["Firebase:AndroidAppId"],
#endif
                builder.Configuration["Firebase:ProjectNumber"],
                builder.Configuration["Firebase:ProjectId"],
                builder.Configuration["Firebase:ApiKey"]
#if ANDROID
                , DefaultChannel
#endif
            )
        );
    }


    static void RegisterNative(MauiAppBuilder builder)
    {
        builder.Services.AddSingleton<IPushSender, NativePushSender>();

        builder.Services.AddPush<MyPushDelegate>(
#if ANDROID
            new FirebaseConfiguration(
                false,
                builder.Configuration["Firebase:AndroidAppId"],
                builder.Configuration["Firebase:ProjectNumber"],
                builder.Configuration["Firebase:ProjectId"],
                builder.Configuration["Firebase:ApiKey"],
                DefaultChannel
            )
#endif
        );
    }

#if ANDROID
    static NotificationChannel DefaultChannel => new NotificationChannel(
        "default_channel",
        "Default Channel",
        NotificationImportance.Default
    )
    {
        LockscreenVisibility = NotificationVisibility.Public
    };
#endif
}