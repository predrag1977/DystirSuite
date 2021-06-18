using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using DystirXamarin.Models;
using System.Linq;
using System.Collections.Generic;
using DystirXamarin.ViewModels;

namespace DystirXamarin.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ManagerPickerPage : ContentPage
    {
        public List<Administrator> AdministratorList { get; private set; }
        private Match _match { get; set; }
        private TypeDetails _typeDetails { get; set; }

        private MatchesViewModel _viewModel;

        public Label ValueLabel { get; set; }
        public string SelectedValue { get; private set; }

        public ManagerPickerPage(MatchesViewModel viewModel, Match match, Label mainManagerLabel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            ValueLabel = mainManagerLabel;
            SelectedValue = ValueLabel.Text;
            Populate(viewModel, match);
            BindingContext = this;
        }

        private void Populate(MatchesViewModel viewModel, Match match)
        {
            Title = "Pick manager";
            AdministratorList = viewModel.Administrators?.OrderBy(x=>x.AdministratorFirstName).ToList() ?? new List<Administrator>();
        }

        private void Managers_Tapped(object sender, EventArgs e)
        {
            //string selectedValue = (e as TappedEventArgs).Parameter?.ToString() ?? string.Empty;
            //if (_typeDetails == TypeDetails.Squad)
            //    ValueEntry.Text = _match.Squads.FirstOrDefault(x => x.SquadName?.ToLower() == selectedValue.ToLower()).SquadShortName;
            //else
            //    ValueEntry.Text = selectedValue;
        }

        private async void OK_Tapped(object sender, EventArgs e)
        {
            //ValueLabel.Text = ValueEntry.Text;
            await Navigation.PopAsync(false);
        }

        private void ManagersListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ListView listView = (ListView)sender;
            listView.SelectedItem = null;
        }
    }
}