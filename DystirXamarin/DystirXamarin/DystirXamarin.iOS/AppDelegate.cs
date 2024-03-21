using System;
using System.Collections.Generic;
using FFImageLoading.Forms.Platform;
using Foundation;
using Plugin.FirebasePushNotification;
using UIKit;

namespace DystirXamarin.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            CachedImageRenderer.Init();

            FirebasePushNotificationManager.Initialize(options, true);

            if (options != null)
            {
                try
                {
                    if (options.TryGetValue(UIApplication.LaunchOptionsRemoteNotificationKey, out NSObject result))
                    {
                        var aps = ((NSDictionary)result)["aps"];
                        var alert = ((NSDictionary)aps)["alert"];
                        var title = ((NSDictionary)alert)["title"];
                        var body = ((NSDictionary)alert)["body"];
                        var matchID = ((NSDictionary)result)["matchID"];
                        IDictionary<string, object> data = new Dictionary<string, object>
                    {
                        { "matchID", matchID },
                        { "aps.alert.title", title },
                        { "aps.alert.body", body}
                    };
                        LoadApplication(new App(data));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex?.Message ?? "Unknown error");
                }
            }
            else
            {
                LoadApplication(new App());
            }

            return base.FinishedLaunching(app, options);
        }

        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            FirebasePushNotificationManager.DidRegisterRemoteNotifications(deviceToken);
        }

        public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
        {
            FirebasePushNotificationManager.RemoteNotificationRegistrationFailed(error);
        }
        // To receive notifications in foregroung on iOS 9 and below.
        // To receive notifications in background in any iOS version
        public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        {
            // If you are receiving a notification message while your app is in the background,
            // this callback will not be fired 'till the user taps on the notification launching the application.

            // If you disable method swizzling, you'll need to call this method. 
            // This lets FCM track message delivery and analytics, which is performed
            // automatically with method swizzling enabled.
            FirebasePushNotificationManager.DidReceiveMessage(userInfo);
            // Do your magic to handle the notification data
            Console.WriteLine(userInfo);

            completionHandler(UIBackgroundFetchResult.NewData);
        }
    }
}
