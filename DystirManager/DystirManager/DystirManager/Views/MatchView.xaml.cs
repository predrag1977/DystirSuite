using System;
using System.Globalization;
using DystirManager.Converter;
using DystirManager.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DystirManager.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MatchView : ContentView
    {
        public MatchView()
        {
            InitializeComponent();
        }

        private void SeeMore_Tapped(object sender, System.EventArgs e)
        {
            Match selectedMatch = (Match)(e as TappedEventArgs).Parameter;
            LiveMatchVisibilityConverter liveMatchVisibilityConverter = new LiveMatchVisibilityConverter();
            var result = liveMatchVisibilityConverter.Convert(selectedMatch.StatusID, null, null, CultureInfo.CurrentCulture);
            if (result != null && !(bool)result)
            {
                return;
            }

            var navigationPage = Application.Current.MainPage as NavigationPage;
            if (navigationPage?.RootPage is MatchesPage)
            {
                var currentPage = navigationPage?.RootPage as MatchesPage;
                _= currentPage.SeeMoreMatchDetail(selectedMatch);
            }
            if (navigationPage?.RootPage is ResultsPage)
            {
                var currentPage = navigationPage?.RootPage as ResultsPage;
                _= currentPage.SeeMoreMatchDetail(selectedMatch);
            }
            else if (navigationPage?.RootPage is FixturesPage)
            {
                var currentPage = navigationPage?.RootPage as FixturesPage;
                _= currentPage.SeeMoreMatchDetail(selectedMatch);
            }
        }

        private void Edit_Tapped(object sender, System.EventArgs e)
        {
            Match selectedMatch = (Match)(e as TappedEventArgs).Parameter;
            var navigationPage = Application.Current.MainPage as NavigationPage;
            if (navigationPage?.RootPage is MatchesPage)
            {
                var currentPage = navigationPage?.RootPage as MatchesPage;
                _ = currentPage.EditMatchDetails(selectedMatch);

            }
            if (navigationPage?.RootPage is ResultsPage)
            {
                var currentPage = navigationPage?.RootPage as ResultsPage;
                _ = currentPage.EditMatchDetails(selectedMatch);
            }
            else if (navigationPage?.RootPage is FixturesPage)
            {
                var currentPage = navigationPage?.RootPage as FixturesPage;
                _ = currentPage.EditMatchDetails(selectedMatch);
            }
        }
    }
}