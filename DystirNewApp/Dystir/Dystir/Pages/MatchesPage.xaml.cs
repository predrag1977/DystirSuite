using System.Collections.ObjectModel;
using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Dystir.ViewModels;
using Dystir.Models;
using System.Text.RegularExpressions;

namespace Dystir.Pages
{
    public partial class MatchesPage : ContentPage
    {
        private readonly MatchesViewModel matchesViewModel;

        public MatchesPage()
        {
            InitializeComponent();
            BindingContext = matchesViewModel = new MatchesViewModel();
        }

        protected override void OnAppearing()
        {
            _ = matchesViewModel.LoadDataAsync();
        }

        private async void RefreshButton_Clicked(object sender, EventArgs e)
        {
            if (matchesViewModel.IsLoading == false)
            {
                await matchesViewModel.DystirService.LoadDataAsync(true);
            }
        }
    }
}
