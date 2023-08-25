using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace Dystir.Models
{
    public class MatchDetailsTab : INotifyPropertyChanged
    {
        //----------------------------//
        //          Properties        //
        //----------------------------//
        public int TabIndex { get; set; }

        private string tabName;
        public string TabName
        {
            get { return tabName; }
            set { tabName = value; OnPropertyChanged(); }
        }

        private Color textColor = Color.White;
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

