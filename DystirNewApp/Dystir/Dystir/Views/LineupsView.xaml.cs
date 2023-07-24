using System;
using Xamarin.Forms;
using Dystir.Models;
using Xamarin.CommunityToolkit.Extensions;

namespace Dystir.Views
{
    public partial class LineupsView : ContentView
    {
        public LineupsView()
        {
           InitializeComponent();
        }

        async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            PlayerOfMatch playerOfMatch = (e as TappedEventArgs).Parameter as PlayerOfMatch;
            await App.Current.MainPage.Navigation.ShowPopupAsync(new PlayerInfoPopupView(playerOfMatch));
        }

        void ListView_BindingContextChanged(System.Object sender, System.EventArgs e)
        {

        }
    }
}
