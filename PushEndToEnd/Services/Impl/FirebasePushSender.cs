using System.Text;
using System.Text.Json;
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
        try
        {
            var json = JsonSerializer.Serialize(new
            {
                type = "service_account",
                project_id = configuration["Firebase:ProjectId"],
                private_key_id = configuration["Firebase:Sender:private_key_id"],
                private_key = configuration["Firebase:Sender:private_key"],
                client_email = configuration["Firebase:Sender:client_email"],
                client_id = configuration["Firebase:Sender:client_id"],
                client_x509_cert_url = configuration["Firebase:Sender:client_x509_cert_url"],
                auth_uri = "https://accounts.google.com/o/oauth2/auth",
                token_uri = "https://oauth2.googleapis.com/token",
                auth_provider_x509_cert_url = "https://www.googleapis.com/oauth2/v1/certs",
                universe_domain = "googleapis.com"
            });
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            var cred = GoogleCredential.FromServiceAccountCredential(
                ServiceAccountCredential.FromServiceAccountData(stream)
            );

            this.app = FirebaseApp.Create(new AppOptions
            {
                Credential = cred
            });
            this.messaging = FirebaseMessaging.GetMessaging(this.app);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
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
                Aps = new Aps
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
#if ANDROID
            message.Android = new AndroidConfig
            {
                
                Notification = new AndroidNotification
                {
                    Title = "Test Notification",
                    Body = "This is a test notification",
                    ClickAction = ShinyPushIntents.NotificationClickAction
                    //Icon = "notification"
                }
            };
#endif
        }
        var response = await this.messaging.SendAsync(message);
    }
}