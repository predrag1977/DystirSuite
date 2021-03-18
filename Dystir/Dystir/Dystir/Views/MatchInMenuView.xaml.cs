using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Dystir.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MatchInMenuView : Frame
    {
        public MatchInMenuView()
        {
            InitializeComponent();
        }

        private void Frame_BindingContextChanged(object sender, System.EventArgs e)
        {

        }
    }
}