using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Dystir.Views
{
    public partial class DystirWebViewPage : ContentPage
    {
        public DystirWebViewPage()
        {
            InitializeComponent();
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
            //if (e.Url.StartsWith("file://"))
            //{
            //    return;
            //}
            //Device.OpenUri(new Uri(e.Url));
            return;
        }

        void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            DystirWebView.Reload();
        }
    }
}
