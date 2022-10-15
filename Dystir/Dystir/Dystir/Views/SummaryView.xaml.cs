using System;
using System.Collections.ObjectModel;
using Dystir.Helper;
using Dystir.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Dystir.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SummaryView : ContentView
    {
        public SummaryView()
        {
            InitializeComponent();
        }

        void ParentLayout_BindingContextChanged(object sender, EventArgs e)
        {
            try
            {
                Grid parentLayout = (sender as Grid);
                parentLayout.Children.Clear();
                var eventOfMatch = (parentLayout.BindingContext as SummaryEventOfMatch).EventOfMatch;
                new Summary().PopulateSummaryView(parentLayout, eventOfMatch);
            }
            catch (Exception)
            {

            }
        }

        void ContentView_BindingContextChanged(System.Object sender, System.EventArgs e)
        {
            var t = BindingContext;
        }
    }
}