using System;
using System.Collections.Generic;
using Dystir.ViewModels;
using Dystir.Views;
using Dystir.Pages;
using Xamarin.Forms;

namespace Dystir
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(MatchDetailPage), typeof(MatchDetailPage));
        }
    }
}

