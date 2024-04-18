namespace PushTesting.Services.Impl;

public class NativePushSender(IApiClient apiClient) : IPushSender
{
    public async Task Send(string token, bool silent)
    {
        //apiClient.Send
    }
}