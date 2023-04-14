using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Dystir.Models;

namespace Dystir.ViewModels
{
    public class SummaryViewModel : INotifyPropertyChanged
    {
        Match match;
        public Match Match
        {
            get { return match; }
            set { match = value; OnPropertyChanged(); }
        }

        ObservableCollection<SummaryEventOfMatch> summary = new ObservableCollection<SummaryEventOfMatch>();
        public ObservableCollection<SummaryEventOfMatch> Summary
        {
            get { return summary; }
            set { summary = value; OnPropertyChanged(); }
        }

        //**************************//
        //  INOTIFYPROPERTYCHANGED  //
        //**************************//
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}