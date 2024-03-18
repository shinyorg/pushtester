using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Azure.NotificationHubs;

namespace PushEndToEnd.Services.Impl;


public class AzureNotificationHubPushSender : IPushSender
{
    readonly NotificationHubClient client;
    readonly JsonSerializerOptions serializerOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };
    

    public AzureNotificationHubPushSender(IConfiguration configuration)
    {
        this.client = NotificationHubClient.CreateClientFromConnectionString(
            configuration["AzureNotificationHubs:FullConnectionString"],
            configuration["AzureNotificationHubs:HubName"],
            true
        );
    }


    public async Task Send(string token, bool silent)
    {
#if ANDROID
        var push = new AndroidPush();
        push.Message.Data.Add("DataTest", "Test for Android");
        if (!silent)
            push.Message.Notification.Body = "Test Message";

        var json = JsonSerializer.Serialize(push, this.serializerOptions);
        var outcome = await this.client.SendFcmV1NativeNotificationAsync(json, new[] { token });
        //var outcome = await this.client.SendFcmV1NativeNotificationAsync(json, new[] { "Testing" });
#else
        var push = new ApplePush { DataTest = "Testing data" };
        push.Aps.ContentAvailable = 1;
        if (!silent)
            push.Aps.Alert = "Test Message";
        
        var json = JsonSerializer.Serialize(push, this.serializerOptions);
        var outcome = await this.client.SendAppleNativeNotificationAsync(json, new[] { token });
        //var outcome = await this.client.SendAppleNativeNotificationAsync(json, new[] { "Testing" });
#endif 

        var result = outcome.Results.FirstOrDefault();
        if (result == null)
            throw new InvalidOperationException("No Push Receivers");

        if (outcome.Failure == 1 || outcome.Success == 0)
            throw new InvalidOperationException("Push Error - " + result.Outcome); 
    }
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
    public AndroidNotification Notification { get; set; } = new();

    [JsonPropertyName("data")]
    public Dictionary<string, string> Data { get; set; } = new();
}

public class AndroidNotification
{
    [JsonPropertyName("body")]
    public string? Body { get; set; }
}

    //"android": {
    //  "notification": {
    //    "click_action": "TOP_STORY_ACTIVITY"
    //  }
    //},

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