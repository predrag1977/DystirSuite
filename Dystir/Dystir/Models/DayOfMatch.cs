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