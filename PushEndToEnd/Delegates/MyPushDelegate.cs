using Shiny.Push;

namespace PushEndToEnd.Delegates;


public class MyPushDelegate(IDialogs dialogs) : PushDelegate
{
    public override Task OnEntry(PushNotification notification)
        => this.Message("Push Notification Entry");

    public override Task OnReceived(PushNotification notification)
        => this.Message("Push Notification Received");

    public override Task OnNewToken(string token)
        => this.Message("New Push Token Received");

    public override Task OnUnRegistered(string token)
        => this.Message("UnRegistered from Push");

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