# Azure Notification Hub & Firebase Messaging End-to-End Testing Applications for Shiny

This app allows you to test Azure Notification Hubs or Firebase end to end (minus your infrastructure)
to ensure you have setup everything properly from APP to Provider.

* No need to spin up server side to send messages to Firebase or Azure, we'll do that here.
* Tired of trying to send a "Test Send" from Azure only for it to pick random devices, this will force it to send to YOUR device.
* Easy to configure - Change the ApplicationId and appsettings.json

## Setup

1. Open PushEndToEnd.csproj and set <ApplicationId>.  This must match your iOS provisioning or Android identifier that you are pushing from
2. Open appsettings.json and set PushProvider to azurenotificationhubs or firebase
3. Open appsettings.json and set appropriate variables
4. All AzureNotificationHubs variables required for Azure tests
5. Ensure to set Firebase/Sender variables from service account key in Firebase

## NOTES
* If you're on iOS & debugging, you will need to be pointed at a sandbox setup environment
* iOS requires you have a specific bundleID setup with push provisioning setup - Your <ApplicationId> must match this bundleID
* Push does not work on iOS Simulators at all
* Push only works on Android simulators if you're signed into Google Play
* You can compile for MacCatalyst for Azure Notification Hubs, but NOT firebase (edit csproj to set this up)
* iOS: if you're server is pointed at development, you can have to compile in debug.  If you're server is pointed at development, you must compile with an adhoc/appstore profile

## FAQ
> Q. Why can I not test pure Apple Push Notifications APNS with this?
> A. There is some special encryption requirements to pushing things to Apple, that I haven't found work on iOS
