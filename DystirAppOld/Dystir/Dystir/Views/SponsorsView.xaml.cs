using Dystir.Models;
using System;
using System.Collections.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Dystir.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SponsorsView : ContentView
    {
        public SponsorsView()
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

        private void SponsorsCarouselView_Scrolled(object sender, ItemsViewScrolledEventArgs e)
        {  
            //_firstItemIndex = e.FirstVisibleItemIndex;
        }

        private void SponsorsCarouselView_BindingContextChanged(object sender, EventArgs e)
        {
            //SponsorsCarouselView.ScrollTo(_firstItemIndex, -1, ScrollToPosition.MakeVisible, false);
        }

        
    }
}