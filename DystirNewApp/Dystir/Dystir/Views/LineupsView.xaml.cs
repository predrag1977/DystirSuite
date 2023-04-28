using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Dystir.Models;
using Xamarin.CommunityToolkit.Extensions;
using Dystir.ViewModels;
using System.Threading;
using System.Threading.Tasks;

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
    }
}
