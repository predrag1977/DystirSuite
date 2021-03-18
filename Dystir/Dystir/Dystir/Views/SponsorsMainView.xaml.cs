using Dystir.Models;
using System;
using System.Collections.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Dystir.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SponsorsMainView : ContentView
    {
        public SponsorsMainView()
        {
            InitializeComponent();
        }

        private async void Sponsor_Tapped(object sender, EventArgs e)
        {
            string sponsorWebSite = (e as TappedEventArgs).Parameter?.ToString();
            var supportsUri = await Launcher.CanOpenAsync(sponsorWebSite);
            if (supportsUri)
                await Launcher.OpenAsync(sponsorWebSite);
        }
    }
}