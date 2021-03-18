using System.ComponentModel;
using Xamarin.Forms;
using DystirManager.ViewModels;

namespace DystirManager.Views
{
    [DesignTimeVisible(true)]
    public partial class StartPage : ContentPage
    {
        public StartPage(MatchesViewModel viewModel)
        {
            InitializeComponent();
            Title = Properties.Resources.Matches;
        }

        //protected override void OnAppearing()
        //{
        //    base.OnAppearing();
        //    ((App)Application.Current).StartHubConnection();
        //}
    }
}