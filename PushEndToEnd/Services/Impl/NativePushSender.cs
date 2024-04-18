namespace PushTesting.Services.Impl;

public class NativePushSender(IApiClient apiClient) : IPushSender
{
    public Task Send(string token, bool silent)
        => apiClient.Send(new PushSendRequest(
            "Test Notification", 
            "Notification message", 
            silent, 
            token,
            new Dictionary<string, string>
            {
                { "TestKey", "TestData" }
            }
        ));
}

