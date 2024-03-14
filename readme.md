# Azure Notification Hub & Firebase Messaging End-to-End Testing Applications for Shiny

## Setup

1. Edit csproj and set <ApplicationId>.  This must match your iOS provisioning or Android identifier that you are pushing from
2. Edit csproj and set <PushProvider> to firebase OR azurenotificationhubs
3. Open appsettings.json and set appropriate variables
    * All firebase variables BUT Firebase:AccessToken required for Android on azure & firebase setups
    * Firebase:AccessToken is required only for sending to Firebase
    * All AzureNotificationHubs variables required for Azure tests

## NOTES
* If you're on iOS & debugging, you will need to be pointed at a sandbox setup environment
* iOS requires you have a specific bundleID setup with push provisioning setup - Your <ApplicationId> must match this bundleID
* Push does not work on iOS Simulators at all
* Push only works on Android simulators if you're signed into Google Play
