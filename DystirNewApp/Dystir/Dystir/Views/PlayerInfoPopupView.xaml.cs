using Xamarin.CommunityToolkit.UI.Views;
using Dystir.Models;
using System;

namespace Dystir.Views
{
    public partial class PlayerInfoPopupView : Popup
    {
        public PlayerInfoPopupView(PlayerOfMatch playerOfMatch)
        {
            InitializeComponent();
            BindingContext = playerOfMatch;
        }

        void Close_Button_Tapped(object sender, EventArgs e)
        {
            this.Dismiss(null);
        }
    }
}