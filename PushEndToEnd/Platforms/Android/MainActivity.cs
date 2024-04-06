using Android.App;
using Android.Content.PM;

namespace PushTesting;


[Activity(
    LaunchMode = LaunchMode.SingleTop,
    Theme = "@style/Maui.SplashTheme", 
    MainLauncher = true, 
    ConfigurationChanges = 
        ConfigChanges.ScreenSize | 
        ConfigChanges.Orientation | 
        ConfigChanges.UiMode | 
        ConfigChanges.ScreenLayout | 
        ConfigChanges.SmallestScreenSize | 
        ConfigChanges.Density
)]
[IntentFilter(
    new[] { 
        ShinyPushIntents.NotificationClickAction 
    },    
    Categories = new[] { 
        global::Android.Content.Intent.CategoryDefault
    }
)]
public class MainActivity : MauiAppCompatActivity
{
}