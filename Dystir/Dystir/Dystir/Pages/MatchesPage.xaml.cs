using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dystir.Models;
using Dystir.ViewModels;
using Xamarin.Forms;

namespace Dystir.Pages
{
    public partial class MatchesPage : ContentPage
    {
        public MatchesPage()
        {
            InitializeComponent();
        }

        private async void SeeMore_Tapped(object sender, System.EventArgs e)
        {
            var viewModel = BindingContext as DystirViewModel;
            viewModel.SelectedMatch = (e as TappedEventArgs).Parameter as Match;

            await Application.Current.MainPage.Navigation.PushAsync(new MatchDetailsPage(viewModel));
        }
    }
}

