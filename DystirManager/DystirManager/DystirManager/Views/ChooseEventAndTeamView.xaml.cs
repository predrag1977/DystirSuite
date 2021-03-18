using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using DystirManager.Models;
using System.Linq;
using System.Collections.Generic;
using DystirManager.ViewModels;
using System.ComponentModel;

namespace DystirManager.Views
{
    [DesignTimeVisible(true)]
    public partial class ChooseEventAndTeamView : ContentView
    {
        public List<String> DetailList { get; private set; }
        private Match _match { get; set; }
        private TypeDetails _typeDetails { get; set; }
        public Label ValueLabel { get; set; }
        public string SelectedValue { get; private set; }

        public ChooseEventAndTeamView()
        {
            InitializeComponent();
        }

        private void TeamsPickerList_SelectedIndexChanged(object sender, EventArgs e)
        {
            var eventOfMatch = BindingContext as EventOfMatch;
            if (eventOfMatch != null)
            {
                eventOfMatch.EventTeam = (sender as Picker).SelectedItem?.ToString();
            }
            BindingContext = eventOfMatch;
        }
    }
}