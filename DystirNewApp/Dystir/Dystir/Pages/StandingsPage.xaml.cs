using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Dystir.ViewModels;
using Dystir.Services;
using Dystir.Models;

namespace Dystir.Pages
{
    public partial class StandingsPage : ContentPage
    {
        private readonly StandingsViewModel standingsViewModel;

        public StandingsPage()
        {
            standingsViewModel = new StandingsViewModel();

            InitializeComponent();
            BindingContext = standingsViewModel;
        }

        protected override void OnAppearing()
        {
            _ = standingsViewModel.LoadDataAsync();
        }

        async void RefreshButton_Clicked(object sender, EventArgs e)
        {
            if (standingsViewModel.IsLoading == false)
            {
                await standingsViewModel.DystirService.LoadDataAsync(true);
            }
        }
    }
}
