using System;
using System.Collections.Generic;
using System.Net;
using Xamarin.Forms;

namespace Dystir.Views
{
    public partial class DystirWebViewPage : ContentPage
    {
        public DystirWebViewPage()
        {
            InitializeComponent();
            DystirWebView.Source = App.URL;
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
            if (e.Url.Equals(App.URL))
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
