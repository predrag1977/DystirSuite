using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
 

namespace Dystir.Models
{
    public class DayOfMatch : INotifyPropertyChanged
    {
        //----------------------------//
        //          Properties        //
        //----------------------------//
        public DateTime Date { get; set; } = DateTime.Now.ToLocalTime().Date;

        public string DateText { get; set; }
        public string DayText { get; set; }

        private Color textColor = Colors.White;
        public Color TextColor
        {
            get { return textColor; }
            set { textColor = value; OnPropertyChanged(); }
        }

        //**************************//
        //  INotifyPropertyChanged  //
        //**************************//
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}