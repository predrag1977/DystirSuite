using System;
using System.Collections.ObjectModel;
using Dystir.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Dystir.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SubstitutionsView : ContentView
    {
        private MatchDetails _matchDetails;

        public SubstitutionsView(MatchDetails matchDetails)
        {
            _matchDetails = matchDetails;
            InitializeComponent();
            BindingContext = matchDetails;
        }

        private void CollectionView_BindingContextChanged(object sender, EventArgs e)
        {
            
        }
    }
}