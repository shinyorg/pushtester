using Refit;

namespace PushTesting.Services;


public interface IApiClient
{
    [Post("/push/register")]
    Task Register([Body] PushRegister pushToken);

    [Delete("/push/{platform}/{pushToken}")]
    Task UnRegister(string platform, string pushToken);
    
    [Post("/push/send")]
    Task Send([Body] PushSendRequest args);
}

public record PushRegister(
    string Platform,
    string DeviceToken,
    string[]? Tags = null
);

public record PushSendRequest(
    string NotificationTitle,
    string NotificationMessage,
    bool IsSilent,
    string PushToken,
    Dictionary<string, string> Data
);