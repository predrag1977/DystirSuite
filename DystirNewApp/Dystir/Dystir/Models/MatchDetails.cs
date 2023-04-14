using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Dystir.Views;
using Dystir.Services;
using Dystir.ViewModels;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Dystir.Models
{
    
    public class MatchDetails : DystirViewModel
    {
        //*****************************//
        //         PROPERTIES          //
        //*****************************//
        public int MatchDetailsID { get; set; }

        public bool IsDataLoaded = false;

        private Match match;
        public Match Match
        {
            get { return match; }
            set { match = value; }
        }

        public ObservableCollection<EventOfMatch> EventsOfMatch { get; set; }

        public ObservableCollection<PlayerOfMatch> PlayersOfMatch { get; set; }

        //**********************//
        //      CONSTRUCTOR     //
        //**********************//


        //**********************//
        //    PUBLIC METHODS    //
        //**********************//


        //**********************//
        //    PRIVATE METHODS   //
        //**********************//



    }
}