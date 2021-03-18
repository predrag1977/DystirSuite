using Dystir.Models;
using Dystir.ViewModels;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Dystir.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SelectedMatchDetailsView : ContentView
    {
        private MatchesViewModel _viewModel;

        public SelectedMatchDetailsView(MatchesViewModel viewModel)
        {
            _viewModel = viewModel;
            InitializeComponent();
            BindingContext = _viewModel;
        }

        private void DetailsMenuItemSelected_Tapped(object sender, EventArgs e)
        {
            int selectedIndex = int.Parse((e as TappedEventArgs).Parameter.ToString());
            MatchDetails matchDetails = (sender as Grid).BindingContext as MatchDetails;
            matchDetails.DetailsMatchTabIndex = selectedIndex;
            matchDetails.SummarySelected = false;
            matchDetails.FirstElevenSelected = false;
            matchDetails.CommentarySelected = false;
            matchDetails.StatisticSelected = false;
            
            switch (selectedIndex)
            {
                case 0:
                    matchDetails.SummarySelected = true;
                    break;
                case 1:
                    matchDetails.FirstElevenSelected = true;
                    break;
                case 2:
                    matchDetails.CommentarySelected = true;
                    break;
                case 3:
                    matchDetails.StatisticSelected = true;
                    break;
            }
        }
    }
}