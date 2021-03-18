using DystirManager.Helper;
using DystirManager.Models;
using DystirManager.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DystirManager.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LineUpsView : ContentView
    {
        public LineUpsView()
        {
            InitializeComponent();
        }

        private void ContentView_BindingContextChanged(object sender, System.EventArgs e)
        {
            Match selectedMatch = BindingContext as Match;
            HomeTeamPopulateFirstEleven(selectedMatch.PlayersOfMatch?.Where(x => x.TeamName?.Trim() == selectedMatch.HomeTeam?.Trim() && x.PlayingStatus == 1));
            AwayTeamPopulateFirstEleven(selectedMatch.PlayersOfMatch?.Where(x => x.TeamName?.Trim() == selectedMatch.AwayTeam?.Trim() && x.PlayingStatus == 1));
            HomeTeamPopulateSubstitutions(selectedMatch.PlayersOfMatch?.Where(x => x.TeamName?.Trim() == selectedMatch.HomeTeam?.Trim() && x.PlayingStatus == 2));
            AwayTeamPopulateSubstitutions(selectedMatch.PlayersOfMatch?.Where(x => x.TeamName?.Trim() == selectedMatch.AwayTeam?.Trim() && x.PlayingStatus == 2));
        }

        private void HomeTeamPopulateFirstEleven(IEnumerable<PlayerOfMatch> playerList)
        {
            new PlayersView().PopulatePlayersView(HomeFirstElevenView, playerList);
        }

        private void AwayTeamPopulateFirstEleven(IEnumerable<PlayerOfMatch> playerList)
        {
            new PlayersView().PopulatePlayersView(AwayFirstElevenView, playerList);
        }

        private void HomeTeamPopulateSubstitutions(IEnumerable<PlayerOfMatch> playerList)
        {
            new PlayersView().PopulatePlayersView(HomeSubstitutionsView, playerList);
        }

        private void AwayTeamPopulateSubstitutions(IEnumerable<PlayerOfMatch> playerList)
        {
            new PlayersView().PopulatePlayersView(AwaySubstitutionsView, playerList);
        }
    }
}