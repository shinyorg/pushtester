using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Notification = FirebaseAdmin.Messaging.Notification;

namespace PushTesting.Services.Impl;


public class FirebasePushSender : IPushSender
{
    readonly FirebaseApp app;
    readonly FirebaseMessaging messaging;


	public FirebasePushSender(IConfiguration configuration)
	{
        this.app = FirebaseApp.Create(new AppOptions
        {
            Credential = GoogleCredential.FromAccessToken(configuration["Firebase:AccessToken"])
        });
        this.messaging = FirebaseMessaging.GetMessaging(this.app);
    }


    public async Task Send(string token, bool silent)
    {
        var message = new Message
        {
            Data = new Dictionary<string, string>
            {
                { "TestKey", "TestData" }
            },
            Token = token,
#if IOS
            Apns = new ApnsConfig
            {
                Aps = new FirebaseAdmin.Messaging.Aps
                {
                    ContentAvailable = true
                }
            }
#endif
        };
        if (!silent)
        {
            message.Notification = new Notification
            {
                Title = "Test Notification",
                Body = "This is a test notification"
            };
        }
        var response = await this.messaging.SendAsync(message);
    }
}