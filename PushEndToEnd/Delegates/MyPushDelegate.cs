using System.Runtime.CompilerServices;
using PushTesting.Services;
using Shiny.Push;

namespace PushTesting.Delegates;


public class MyPushDelegate(
    AppSqliteConnection conn, 
    IApiClient apiClient,
    IConfiguration configuration, 
    IDialogs dialogs
) : PushDelegate
{
    public override async Task OnEntry(PushNotification notification)
    {
        await this.Store(notification.Data.Select(x => (x.Key, x.Value)).ToArray());
        await this.Message("Push Notification Entry");
    }

    public override async Task OnReceived(PushNotification notification)
    {
#if ANDROID
        // TODO: if in foreground
        ((AndroidPushNotification)notification).SendDefault(100);
#endif
        await this.Store(notification.Data.Select(x => (x.Key, x.Value)).ToArray());
        await this.Message("Push Notification Received");
    }

    public override async Task OnNewToken(string token)
    {
        await this.Store(new[] { ("Token", token) });
        await this.Message("New Push Token Received");
        if (configuration["PushProvider"] == "native")
        {
            await apiClient.UnRegister(OS, token);
        }
    }

    public override async Task OnUnRegistered(string token)
    {
        await this.Store([("Token", token)]);
        await this.Message("UnRegistered from Push");
        if (configuration["PushProvider"] == "native")
        {
            await apiClient.UnRegister(OS, token);
        }
    }

    const string OS =
        #if IOS
        "ios";
        #else
        "android";
        #endif

    Task Store((string Key, string Value)[] values, [CallerMemberName] string? eventName = null)
    {
        var e = String.Empty;
        foreach (var kv in values)
            e += $"{kv.Key}={kv.Value}&";

        return conn.InsertAsync(new AppEvent
        {
            EventName = eventName!,
            Description = e,
            DateCreated = DateTime.UtcNow
        });
    }


    async Task Message(string msg)
    {
        try
        {
            Console.WriteLine(msg);
            await dialogs.Snackbar(msg);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }
}