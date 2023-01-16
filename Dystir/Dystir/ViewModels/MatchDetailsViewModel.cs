using System;
using System.Collections.ObjectModel;
using System.IO;
using Dystir.Models;
using Dystir.Services;
using Dystir.Views;

namespace Dystir.ViewModels
{
    public class MatchDetailsViewModel : DystirViewModel
    {
        //**********************//
        //      PROPERTIES      //
        //**********************//

        Match selectedMatch;
        public Match SelectedMatch
        {
            get { return selectedMatch; }
            set { selectedMatch = value; OnPropertyChanged(); }
        }

        

        MatchDetailsTab selectedMatchDetailsTab = new MatchDetailsTab()
        {
            TabName = Resources.Localization.Resources.Summary,
            TextColor = Colors.LimeGreen
        };
        public MatchDetailsTab SelectedMatchDetailsTab
        {
            get { return selectedMatchDetailsTab; }
            set { selectedMatchDetailsTab = value; }
        }

        

        

        
        
    }
}

