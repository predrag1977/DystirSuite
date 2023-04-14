using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Dystir.ViewModels;
using Dystir.Services;
using Dystir.Models;

namespace Dystir.Pages
{
    public partial class ResultsPage : ContentPage
    {
        private readonly ResultsViewModel resultsViewModel;
        private readonly DystirService dystirService;
        private readonly LiveStandingService liveStandingService;

        public ResultsPage()
        {
            resultsViewModel = new ResultsViewModel();
            dystirService = DependencyService.Get<DystirService>();
            liveStandingService = DependencyService.Get<LiveStandingService>();

            resultsViewModel.IsLoading = true;
            InitializeComponent();
            BindingContext = resultsViewModel;
        }

        protected override void OnAppearing()
        {
            _ = resultsViewModel.LoadDataAsync();
        }

        async void RefreshButton_Clicked(object sender, EventArgs e)
        {
            if (resultsViewModel.IsLoading == false)
            {
                await resultsViewModel.DystirService.LoadDataAsync(true);
            }
        }
    }
}
