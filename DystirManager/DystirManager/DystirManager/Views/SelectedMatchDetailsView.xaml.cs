using DystirManager.Models;
using DystirManager.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DystirManager.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SelectedMatchDetailsView : ContentPage
    {
        private int _detailsMatchitemIndex = 0;
        private MatchesViewModel _viewModel;

        public SelectedMatchDetailsView(MatchesViewModel viewModel)
        {
            _viewModel = viewModel;
            _viewModel.IsLoadingSelectedMatch = !_viewModel.IsDisconnected;
            InitializeComponent();
            BindingContext = viewModel;
            AddDetailsMenuItemView();
            ShowSelectedDetailsTab();
            _ = LoadSelectedMatchDataAsync(_viewModel);
        }

        public void ReloadSelectedMatches(MatchesViewModel viewModel, bool refreshSelectedMatch)
        {
            _viewModel = viewModel;
            BindingContext = viewModel;
            if (refreshSelectedMatch)
            {
                _ = LoadSelectedMatchDataAsync(viewModel);
            }
        }

        private async Task LoadSelectedMatchDataAsync(MatchesViewModel viewModel)
        {
            await viewModel.GetSelectedLiveMatch(viewModel.SelectedLiveMatch);
            viewModel.TimeCounter.MatchesTime(viewModel, new ObservableCollection<Match>());
            viewModel.IsLoadingSelectedMatch = false;
        }

        private void AddDetailsMenuItemView()
        {
            if (DetailsMenuItemView.Children != null)
            {
                DetailsMenuItemView.Children.Clear();
                DetailsMenuItemView.ColumnDefinitions.Clear();
                DetailsMenuItemView.RowDefinitions.Clear();
            }

            RowDefinition rd = new RowDefinition
            {
                Height = new GridLength(36)
            };
            DetailsMenuItemView.RowDefinitions.Add(rd);
            List<string> menuItemList = new List<string>()
            {
                "Events",
                "Summary",
                "Line ups"
            };

            int i = 0;
            foreach (string item in menuItemList)
            {
                ColumnDefinition cd = new ColumnDefinition
                {
                    Width = new GridLength(1, GridUnitType.Star)
                };
                DetailsMenuItemView.ColumnDefinitions.Add(cd);
                DetailMenuItem view = new DetailMenuItem
                {
                    BindingContext = item
                };
                Label menuTextLabel = (Label)view.FindByName("MenuTextLabel");
                menuTextLabel.FontSize = 12;
                StackLayout detailMenuItemPanel = (StackLayout)view.FindByName("DetailMenuItemPanel");
                detailMenuItemPanel.BackgroundColor = Color.DimGray;
                menuTextLabel.TextColor = Color.White;
                if (_detailsMatchitemIndex < menuItemList.Count && item == menuItemList[_detailsMatchitemIndex])
                {
                    detailMenuItemPanel.BackgroundColor = Color.FromHex("#2F4F2F");
                }
                Grid.SetColumn(view, i);
                var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += DetailsMenuItemSelected_Tapped;
                tapGestureRecognizer.CommandParameter = menuItemList.IndexOf(item);
                view.GestureRecognizers.Add(tapGestureRecognizer);
                DetailsMenuItemView.Children.Add(view);
                i++;
            }
        }

        private void ShowSelectedDetailsTab()
        {
            SendEventsView.IsVisible = _detailsMatchitemIndex == 0;
            SummaryEventsView.IsVisible = _detailsMatchitemIndex == 1;
            LineUpsView.IsVisible = _detailsMatchitemIndex == 2;
        }

        private void DetailsMenuItemSelected_Tapped(object sender, EventArgs e)
        {
            int selectedIndex = int.Parse((e as TappedEventArgs).Parameter.ToString());
            if (selectedIndex != _detailsMatchitemIndex)
            {
                _detailsMatchitemIndex = selectedIndex;
                AddDetailsMenuItemView();
                ShowSelectedDetailsTab();
            }
        }

        private async void BackToMatchesView_Tapped(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void Refresh_Tapped(object sender, EventArgs e)
        {
            _viewModel.IsLoadingSelectedMatch = !_viewModel.IsDisconnected;
            await LoadSelectedMatchDataAsync(_viewModel);
        }

        private async void SetLineUps_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SetLineUpsPage(_viewModel));
        }

        private async void SetMatchTime_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SetMatchTimeAndPeriodPage(_viewModel.SelectedLiveMatch));
        }
    }
}