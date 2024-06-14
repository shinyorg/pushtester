using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Azure.NotificationHubs;
using Shiny.Push;

namespace PushTesting.Services.Impl;


public class AzureNotificationHubPushSender : IPushSender
{
    readonly NotificationHubClient client;
    readonly IPushManager pushManager;
    readonly ILogger logger;
    readonly JsonSerializerOptions serializerOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };
    

    public AzureNotificationHubPushSender(IPushManager pushManager, IConfiguration configuration, ILogger<AzureNotificationHubPushSender> logger)
    {
        this.pushManager = pushManager;
        this.client = NotificationHubClient.CreateClientFromConnectionString(
            configuration["AzureNotificationHubs:FullConnectionString"],
            configuration["AzureNotificationHubs:HubName"],
            true
        );
        this.logger = logger;
    }


    public async Task Send(string token, bool silent)
    {
        await this.pushManager.TrySetTags(token);
#if ANDROID
        var push = new AndroidPush();
        push.Message.Data.Add("DataTest", "Test for Android");
        if (!silent)
        {
            push.Message.Notification.Body = "Test Message";
            push.Message.Android = new AndroidConfig
            {
                Notification = new AndroidNotification
                {
                    ClickAction = ShinyPushIntents.NotificationClickAction
                }
            };
        }

        var json = JsonSerializer.Serialize(push, this.serializerOptions);
        this.logger.LogDebug("PUSH JSON: " + json);
        var outcome = await this.client.SendFcmV1NativeNotificationAsync(json, new[] { token });
#else
        var push = new ApplePush { DataTest = "Testing data" };
        push.Aps.ContentAvailable = 1;
        if (!silent)
            push.Aps.Alert = "Test Message";
        
        var json = JsonSerializer.Serialize(push, this.serializerOptions);
        this.logger.LogDebug("PUSH JSON: " + json);
        var outcome = await this.client.SendAppleNativeNotificationAsync(json, new[] { token });
#endif 

        var result = outcome.Results.FirstOrDefault();
        if (result == null)
            throw new InvalidOperationException("No Push Receivers");

        if (outcome.Failure == 1 || outcome.Success == 0)
            throw new InvalidOperationException("Push Error - " + result.Outcome); 
    }


    public class AndroidPush
    {
        [JsonPropertyName("message")]
        public AndroidMessage Message { get; set; } = new();
    }

    public class AndroidMessage
    {
        [JsonPropertyName("topic")]
        public string? Topic { get; set; }

        [JsonPropertyName("notification")]
        public StandardNotification Notification { get; set; } = new();

        [JsonPropertyName("data")]
        public Dictionary<string, string> Data { get; set; } = new();

        [JsonPropertyName("android")]
        public AndroidConfig? Android { get; set; }
    }

    public class StandardNotification
    {
        [JsonPropertyName("body")]
        public string? Body { get; set; }
    }

    public class AndroidConfig
    {
        [JsonPropertyName("notification")]
        public AndroidNotification? Notification { get; set; }
    }

    public class AndroidNotification
    {
        [JsonPropertyName("click_action")]
        public string? ClickAction { get; set; }
    }

    public class ApplePush
    {
        [JsonPropertyName("aps")]
        public Aps Aps { get; set; } = new();

        public string? DataTest { get; set; }
    }

    public class Aps
    {
        [JsonPropertyName("alert")]
        public string? Alert { get; set; }

        [JsonPropertyName("content-available")]
        public int? ContentAvailable { get; set; }
    }
}