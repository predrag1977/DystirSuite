using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Dystir.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DetailMenuItem : ContentView
    {
        public DetailMenuItem(string item)
        {
            InitializeComponent();
            BindingContext = item;
        }
    }
}