using System;
using Dystir.Helper;
using Dystir.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Dystir.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CommentaryView : ContentView
    {
        public CommentaryView()
        {
            InitializeComponent();
        }

        void ParentLayout_BindingContextChanged(object sender, EventArgs e)
        {
            try
            {
                Grid parentLayout = (sender as Grid);
                parentLayout.Children.Clear();
                var eventOfMatch = parentLayout.BindingContext as EventOfMatch;
                new Commentary().PopulateCommentaryView(parentLayout, eventOfMatch);
            }
            catch (Exception)
            {

            }
        }
    }
}