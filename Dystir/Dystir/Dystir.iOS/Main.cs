using System;
using Dystir.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(WebView), typeof(WebViewRenderer))]
namespace Dystir.iOS
{

    public class Application
    {
        // This is the main entry point of the application.
        [Obsolete]
        static void Main(string[] args)
        {
            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.
            UIApplication.Main(args, null, "AppDelegate");
        }
    }

    public class WebViewRenderer : WkWebViewRenderer
    {
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            // Setting the background as transparent
            this.Opaque = false;
            this.BackgroundColor = Color.Transparent.ToUIColor();
        }
    }
}
