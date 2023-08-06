using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using Dystir.Models;
using Dystir.Services;
using System.Threading.Tasks;
using System.Linq;
using Dystir.Pages;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace Dystir.ViewModels
{
    public class DystirViewModel : INotifyPropertyChanged
    {
        //**************************//
        //        PROPERTIES        //
        //**************************//
        public DystirService DystirService;
        public TimeService TimeService;
        public LiveStandingService LiveStandingService;
        private readonly AnalyticsService AnalyticsService;

        public Command<Match> MatchTapped { get; }
        public Command<Sponsor> SponsorTapped { get; }
        public Command NewsTapped { get; }

        ObservableCollection<Sponsor> firstCategorySponsors = new ObservableCollection<Sponsor>();
        public ObservableCollection<Sponsor> FirstCategorySponsors
        {
            get { return firstCategorySponsors; }
            set { firstCategorySponsors = value; OnPropertyChanged(); }
        }

        ObservableCollection<Sponsor> secondCategorySponsors = new ObservableCollection<Sponsor>();
        public ObservableCollection<Sponsor> SecondCategorySponsors
        {
            get { return secondCategorySponsors; }
            set { secondCategorySponsors = value; OnPropertyChanged(); }
        }

        ObservableCollection<Sponsor> thirdCategorySponsors = new ObservableCollection<Sponsor>();
        public ObservableCollection<Sponsor> ThirdCategorySponsors
        {
            get { return thirdCategorySponsors; }
            set { thirdCategorySponsors = value; OnPropertyChanged(); }
        }

        bool isLoading;
        public bool IsLoading
        {
            get { return isLoading; }
            set { isLoading = value; OnPropertyChanged(); }
        }

        string pageTitle = string.Empty;
        public string PageTitle
        {
            get { return pageTitle; }
            set { pageTitle = value; OnPropertyChanged(); }
        }

        //**********************//
        //      CONSTRUCTOR     //
        //**********************//
        public DystirViewModel()
        {
            DystirService = DependencyService.Get<DystirService>();
            AnalyticsService = DependencyService.Get<AnalyticsService>();
            TimeService = DependencyService.Get<TimeService>();

            DystirService.OnShowLoading += DystirService_OnShowLoading;

            MatchTapped = new Command<Match>(OnMatchSelected);
            SponsorTapped = new Command<Sponsor>(OnSponsorTapped);
            NewsTapped = new Command(OnNewsTapped);
        }

        public void DystirService_OnShowLoading()
        {
            IsLoading = true;
        }

        //**************************//
        //       PUBLIC METHODS     //
        //**************************//
        public async Task SetSponsors()
        {
            var sponsors = DystirService.AllSponsors;
            
            var firstCategorySponsors = sponsors.Where(x => x.SponsorID < 20).OrderBy(a => Guid.NewGuid());
            firstCategorySponsors.ToList().ForEach(x => x.Size = new Size(180, 60));
            FirstCategorySponsors = new ObservableCollection<Sponsor>(firstCategorySponsors);

            var secondCategorySponsors = sponsors.Where(x => x.SponsorID >= 20 && x.SponsorID < 100).OrderBy(a => Guid.NewGuid());
            secondCategorySponsors.ToList().ForEach(x => x.Size = new Size(150, 50));
            SecondCategorySponsors = new ObservableCollection<Sponsor>(secondCategorySponsors);

            var thirdCategorySponsors = sponsors.Where(x => x.SponsorID >= 100).OrderBy(a => Guid.NewGuid());
            thirdCategorySponsors.ToList().ForEach(x => x.Size = new Size(90, 30));
            ThirdCategorySponsors = new ObservableCollection<Sponsor>(thirdCategorySponsors);

            await Task.CompletedTask;
        }

        //**************************//
        //      PRIVATE METHODS     //
        //**************************//
        async void OnMatchSelected(Match match)
        {
            if (match == null)
                return;

            await Shell.Current.GoToAsync($"{nameof(MatchDetailPage)}?MatchID={match.MatchID}");
        }

        async void OnSponsorTapped(Sponsor sponsor)
        {
            try
            {
                if (sponsor == null) return;
                await Launcher.OpenAsync(new Uri(sponsor.SponsorWebSite));
                AnalyticsService.Sponsors(sponsor.SponsorWebSite);
            }
            catch { }
        }

        async void OnNewsTapped()
        {
            await Shell.Current.GoToAsync($"{nameof(NewsPage)}");
        }

        //**************************//
        //  INOTIFYPROPERTYCHANGED  //
        //**************************//
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
