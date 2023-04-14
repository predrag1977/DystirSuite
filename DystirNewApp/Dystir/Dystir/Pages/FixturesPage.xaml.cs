using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Dystir.ViewModels;

namespace Dystir.Pages
{
    public partial class FixturesPage : ContentPage
    {
        private readonly FixturesViewModel fixturesViewModel;

        public FixturesPage()
        {
            fixturesViewModel = new FixturesViewModel();
            InitializeComponent();
            BindingContext = fixturesViewModel;
        }

        protected override void OnAppearing()
        {
            _ = fixturesViewModel.LoadDataAsync();
        }

        void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private async void RefreshButton_Clicked(object sender, EventArgs e)
        {
            if (fixturesViewModel.IsLoading == false)
            {
                await fixturesViewModel.DystirService.LoadDataAsync(true);
            }
        }
    }
}

