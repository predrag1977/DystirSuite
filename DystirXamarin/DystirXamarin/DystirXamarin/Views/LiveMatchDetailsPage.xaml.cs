using DystirXamarin.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DystirXamarin.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LiveMatchDetailsPage : ContentPage
    {
        private MatchesViewModel _viewModel;

        public LiveMatchDetailsPage(MatchesViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected async override void OnDisappearing()
        {
            await _viewModel.GetMatches();
            base.OnDisappearing();
        }

        private async void LineUp_Tapped(object sender, EventArgs e)
        {
            string teamName = (e as TappedEventArgs).Parameter?.ToString();
            await Navigation.PushAsync(new LineUpsPage(_viewModel, teamName), false);
        }

        private void Tactic_Tapped(object sender, EventArgs e)
        {
            int teamID = (int)(e as TappedEventArgs).Parameter;
        }

        private void CoachAndStaff_Tapped(object sender, EventArgs e)
        {
            int teamID = (int)(e as TappedEventArgs).Parameter;
        }

        private void Refeerees_Tapped(object sender, EventArgs e)
        {

        }

        private void PitchAndWeather_Tapped(object sender, EventArgs e)
        {

        }

        private void MatchEvents_Tapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new EventsOfMatchPage(_viewModel), false);
        }
    }
}