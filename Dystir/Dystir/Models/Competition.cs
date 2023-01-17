using System.ComponentModel;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace Dystir.Models
{
    public class Competition : INotifyPropertyChanged
    {
        //----------------------------//
        //          Properties        //
        //----------------------------//
        private string competitionName;
        public string CompetitionName
        {
            get { return competitionName; }
            set { competitionName = value; OnPropertyChanged(); }
        }

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