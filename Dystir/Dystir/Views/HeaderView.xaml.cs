using Dystir.Services;
using Dystir.ViewModels;
using Microsoft.AppCenter.Analytics;
using System.Globalization;

namespace Dystir.Views
{
    public partial class HeaderView : ContentView
    {
        private MatchesViewModel _viewModel;
        private readonly DystirService dystirService;

        public HeaderView()
        {
            dystirService = DependencyService.Get<DystirService>();
            InitializeComponent();
        }

        private void ContentView_BindingContextChanged(object sender, EventArgs e)
        {
            _viewModel = BindingContext as MatchesViewModel;
            ShowLanguageFlag();
        }

        private void Refresh_Tapped(object sender, EventArgs e)
        {
            MatchesViewModel viewModel = (e as TappedEventArgs).Parameter as MatchesViewModel;
            RefreshAllData(viewModel);
        }

        private void RefreshAllData(MatchesViewModel viewModel)
        {
            if (viewModel != null)
            {
                viewModel.IsLoading = true;
            }
            _ = dystirService.LoadDataAsync(false);
            //await (Application.Current as App).ReloadAsync(LoadDataType.MainData);
        }

        private void Language_Tapped(object sender, EventArgs e)
        {

        }

        private void ShowLanguageFlag()
        {

        }
    }
}

