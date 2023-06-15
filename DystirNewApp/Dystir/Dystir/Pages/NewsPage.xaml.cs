using System;
using System.Collections.Generic;
using Xamarin.Forms;
namespace Dystir.Pages
{	
	public partial class NewsPage : ContentPage
	{	
		public NewsPage ()
		{
			InitializeComponent ();
		}

        protected override void OnAppearing()
        {
            Shell.SetTabBarIsVisible(this, false);
        }

        private async void BackButton_Clicked(object sender, EventArgs e)
        {
            Shell.SetTabBarIsVisible(this, true);
            await Navigation.PopAsync(true);
        }
    }
}

