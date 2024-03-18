using PushEndToEnd.Services;
using Shiny.Push;

namespace PushEndToEnd;


public class MainViewModel : FuncViewModel
{
    public MainViewModel(
        BaseServices services,
        IPushSender pushSender,
        IPushManager pushManager,
        IConfiguration configuration
    ) : base(services)
    {
        this.PushPlatform = configuration["PushProvider"]!.Contains("azure")
            ? "Azure Notification Hubs"
            : "Firebase Message";
           
        this.WhenAnyValue(x => x.RegistrationToken)
            .Subscribe(x =>
            {
                this.NativeRegistrationToken = pushManager.NativeRegistrationToken;
                this.ActionText = x.IsEmpty() ? "Register" : "UnRegister";
            });

#if IOS
        this.WhenAnyValue(x => x.IsSilent)
            .Where(x => x == true)
            .Subscribe(async _ =>
            {
                await this.Dialogs.Alert("Don't send more than 3-4 silent notifications on iOS per hour or they will throttle you");
            });
#endif

        this.Appearing = () => this.RegistrationToken = pushManager.RegistrationToken;

        this.Register = ReactiveCommand.CreateFromTask(async () =>
        {
            if (pushManager.RegistrationToken == null)
            {
                var result = await pushManager.RequestAccess();
                if (result.Status != AccessState.Available)
                {
                    await this.Dialogs.Alert("Failed to register - " + result.Status);
                }
                else
                {
                    this.RegistrationToken = result.RegistrationToken;
                    await pushManager.TrySetTags(result.RegistrationToken!, "Testing");
                }  
            }
            else
            {
                await pushManager.UnRegister();
                this.RegistrationToken = null;
            }
        });

        this.Send = ReactiveCommand.CreateFromTask(
            async () =>
            {
                await pushSender.Send(pushManager.RegistrationToken!, this.IsSilent);
                var msg = this.IsSilent ? "Push Sent - there will be another toast when received" : "Push Sent";
                await this.Dialogs.Snackbar(msg);
            },
            this.WhenAny(
                x => x.RegistrationToken,
                x => !x.GetValue().IsEmpty()
            )
        );
    }


    public string PushPlatform { get; }

    public ICommand Register { get; }
    public ICommand Send { get; }

    [Reactive] public bool IsSilent { get; set; }
    [Reactive] public string ActionText { get; private set; }
    [Reactive] public string? RegistrationToken { get; private set; }
    [Reactive] public string? NativeRegistrationToken { get; private set; }
}
