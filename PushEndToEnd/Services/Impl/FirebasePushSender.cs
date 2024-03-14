#if FIREBASE
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Notification = FirebaseAdmin.Messaging.Notification;

namespace PushEndToEnd.Services.Impl;


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
            //Topic = token,
            Notification = new Notification
            {
                Title = "Test Notification",
                Body = "This is a test notification"
            },
            Token = token
        };
        var response = await this.messaging.SendAsync(message);
    }
}
#endif