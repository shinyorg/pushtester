using Refit;

namespace PushTesting.Services;


public interface IApiClient
{
    [Post("/push/register")]
    Task Register([Body] PushRegister pushToken);

    [Delete("/push/{platform}/{pushToken}")]
    Task UnRegister(string platform, string pushToken);


    [Post("")]
    Task Send();
}

public record PushRegister(
    string Platform,
    string DeviceToken,
    string[]? Tags = null
);