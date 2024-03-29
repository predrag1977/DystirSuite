﻿using Android.App;
using Android.Content.PM;
using Android.OS;
using FFImageLoading.Forms.Platform;
using FFImageLoading.Svg.Forms;

namespace DystirXamarin.Droid
{
    [Activity(Label = "Dystir Manager", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            CachedImageRenderer.Init(true);
            SvgCachedImage.Init();

            LoadApplication(new App());
        }
    }
}