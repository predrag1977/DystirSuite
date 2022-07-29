using System;
using Xamarin.Forms;

namespace Dystir.Views
{
    public partial class DystirWebViewPage : ContentPage
    {
        private const string URL = "http://localhost:51346/client/mobileclient";
        //private const string URL = "https://www.dystir.fo/client/mobileclient";

        public DystirWebViewPage()
        {
            InitializeComponent();
            DystirWebView.Source = URL;
            DystirWebView.Reload();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        [Obsolete]
        public void OnNavigated(object sender, WebNavigatedEventArgs e)
        {
            DystirWebView.Navigating += DystirWebView_Navigating;
        }

        [Obsolete]
        private void DystirWebView_Navigating(object sender, WebNavigatingEventArgs e)
        {
            if (e.Url.Equals(URL))
            {
                return;
            }
            Device.OpenUri(new Uri(e.Url));
            e.Cancel = true;
        }

        void Button_Clicked(object sender, EventArgs e)
        {
            DystirWebView.Reload();
        }
    }
}
