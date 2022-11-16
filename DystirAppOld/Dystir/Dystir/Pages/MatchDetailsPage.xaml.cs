using System;
using System.Threading.Tasks;
using Dystir.Models;
using Dystir.Services;
using Dystir.ViewModels;
using Xamarin.Forms;

namespace Dystir.Pages
{
    public partial class MatchDetailsPage : ContentPage
    {
        public MatchDetailsPage(DystirViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        private void DetailsMenuItemSelected_Tapped(object sender, EventArgs e)
        {
            var dystirViewModel = BindingContext as DystirViewModel;
            int selectedIndex = int.Parse((e as TappedEventArgs).Parameter.ToString());
            dystirViewModel.SelectedMatch.DetailsMatchTabIndex = selectedIndex;
        }

        async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            await Application.Current.MainPage.Navigation.PopAsync();
        }

        void MatchesBySelectedDateCarouselView_CurrentItemChanged(object sender, CurrentItemChangedEventArgs e)
        {
            if (e.CurrentItem is Match selectedMatch && selectedMatch.Details == null)
            {
                _ = DependencyService.Get<DystirService>().LoadMatchDetailsAsync(selectedMatch);
            }
        }
    }
}

