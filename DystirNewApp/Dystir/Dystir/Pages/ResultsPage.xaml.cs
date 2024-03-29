﻿using System;
using Xamarin.Forms;
using Dystir.ViewModels;
using Dystir.Services;

namespace Dystir.Pages
{
    public partial class ResultsPage : ContentPage
    {
        private readonly ResultsViewModel resultsViewModel;
        private readonly AnalyticsService analyticsService;
        private readonly LanguageService languageService;

        public ResultsPage()
        {
            analyticsService = DependencyService.Get<AnalyticsService>();
            languageService = DependencyService.Get<LanguageService>();
            
            InitializeComponent();
            BindingContext = resultsViewModel = new ResultsViewModel();
            resultsViewModel.IsLoading = true;
        }

        protected override void OnAppearing()
        {
            analyticsService.Results();
            _ = resultsViewModel.LoadDataAsync();
        }

        async void RefreshButton_Clicked(object sender, EventArgs e)
        {
            if (resultsViewModel.IsLoading == false)
            {
                await resultsViewModel.DystirService.LoadDataAsync(false);
            }
        }

        void Language_Clicked(object sender, EventArgs e)
        {
            languageService.LanguageChange();
        }
    }
}
