using System;
using Dystir.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Dystir.Views
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
            Match selectedMatch = BindingContext as Match;
            ((Application.Current.MainPage as NavigationPage).CurrentPage as DystirPage).SeeMatchDetailsAsync(selectedMatch, true);
        }
    }
}