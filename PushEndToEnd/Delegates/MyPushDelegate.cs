using System.Runtime.CompilerServices;
using PushTesting.Services;
using Shiny.Push;

namespace PushTesting.Delegates;


public class MyPushDelegate(
    AppSqliteConnection conn, 
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
    }

    public override async Task OnUnRegistered(string token)
    {
        await this.Store([("Token", token)]);
        await this.Message("UnRegistered from Push");
    }
    

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