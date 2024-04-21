using Shiny.Push;

namespace PushTesting.Services;


public class MyNativePushProvider(IApiClient apiClient) : IPushProvider
{
#if IOS
    public async Task<string> Register(Foundation.NSData nativeToken)
    {
        var token = nativeToken.ToPushTokenString();
        await apiClient.Register(new PushRegister(OS, token));
        return token;
    }
#else
    public async Task<string> Register(string nativeToken)
    {
        await apiClient.Register(new PushRegister(OS, nativeToken));
        return nativeToken;
    }
#endif
    
    // TODO: pass in token and native token since it is stored in pushmanager
    public Task UnRegister() => apiClient.UnRegister(OS, "TODO");
    
    
    const string OS =
#if IOS
        "ios";
#else
        "android";
#endif
}