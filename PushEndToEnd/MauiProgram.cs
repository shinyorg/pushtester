using PushEndToEnd.Delegates;
using PushEndToEnd.Services;
using PushEndToEnd.Services.Impl;
using Shiny.Push;

namespace PushEndToEnd;


public static class MauiProgram
{
    public static MauiApp CreateMauiApp() => MauiApp
        .CreateBuilder()
        .UseMauiApp<App>()
        .UseMauiCommunityToolkit()
        .UseShinyFramework(
            new DryIocContainerExtension(),
            prism => prism.OnAppStart("NavigationPage/MainPage"),
            new (
#if DEBUG
                ErrorAlertType.FullError
#else
                ErrorAlertType.NoLocalize
#endif
            )
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
        builder.Services.AddDataAnnotationValidation();
        return builder;
    }


    static MauiAppBuilder RegisterViews(this MauiAppBuilder builder)
    {
        var s = builder.Services;

        s.RegisterForNavigation<MainPage, MainViewModel>();
        return builder;
    }

#if AZURE
    static MauiAppBuilder RegisterPush(this MauiAppBuilder builder)
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
                builder.Configuration["Firebase:ApiKey"]
            )
#endif
        );
        return builder;
    }

#elif FIREBASE

    static MauiAppBuilder RegisterPush(this MauiAppBuilder builder)
    {
        builder.Services.AddSingleton<IPushSender, FirebasePushSender>();

        // TODO: watch android/ios specific var below
        builder.Services.AddPushFirebaseMessage<MyPushDelegate>(
            new FirebaseConfig(
                false,
#if IOS
                builder.Configuration["Firebase:AppleAppId"],
#elif ANDROID
                builder.Configuration["Firebase:AndroidAppId"],
#endif
                builder.Configuration["Firebase:ProjectNumber"],
                builder.Configuration["Firebase:ProjectId"],
                builder.Configuration["Firebase:ApiKey"]
            )
        );
        return builder;
    }

#endif
}