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
        public SelectedMatchDetailsView()
        {
            InitializeComponent();
        }

        private void DetailsMenuItemSelected_Tapped(object sender, EventArgs e)
        {
            int selectedIndex = int.Parse((e as TappedEventArgs).Parameter.ToString());
            (BindingContext as DystirViewModel).SelectedMatch.DetailsMatchTabIndex = selectedIndex;
        }
    }
}