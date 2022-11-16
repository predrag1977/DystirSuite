using System;
using System.ComponentModel;
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
            set { SetProperty(ref tabName, value); }
        }

        private Color textColor = Colors.White;
        public Color TextColor
        {
            get { return textColor; }
            set { SetProperty(ref textColor, value); }
        }

        //----------------------------//
        //    INotifyPropertyChanged  //
        //----------------------------//
        #region INotifyPropertyChanged
        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName] string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}

