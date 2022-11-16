using System;
using System.ComponentModel;
using Xamarin.Forms;
using Dystir.ViewModels;
using System.Collections.ObjectModel;
using Dystir.Models;

namespace Dystir.Views
{
    [DesignTimeVisible(true)]
    public partial class MatchesPage : ContentView
    {
        private DystirViewModel _viewModel;

        public MatchesPage(DystirViewModel viewModel)
        {
            _viewModel = viewModel;
            InitializeComponent();
            BindingContext = _viewModel;
        }

        private void DaysView_BindingContextChanged(object sender, EventArgs e)
        {
            PopulateMatchDays();
        }

        private void PopulateMatchDays()
        {
            
        }

        private void MatchDay_Tapped(object sender, EventArgs e)
        {
            _viewModel.MatchesDaySelected = (DayOfMatch)(e as TappedEventArgs)?.Parameter;
            PopulateMatchDays();
        }

        private void OnMatchSelected(object sender, SelectedItemChangedEventArgs args)
        {
            ListView listView = (ListView)sender;
            listView.SelectedItem = null;
        }
    }
}