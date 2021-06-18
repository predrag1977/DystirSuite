using Dystir.Services;
using Dystir.Services.DystirHubService;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Dystir.ViewModels
{
    public class DystirViewModel
    {

        internal IDystirHub GetDystirHubService()
        {
            return DependencyService.Get<IDystirHub>();
        }
    }
}
