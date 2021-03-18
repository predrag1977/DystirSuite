using System;
using System.Collections.ObjectModel;
using System.Linq;
using DystirWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace DystirWeb.Controllers
{
    public class SponsorsViewController : Controller
    {
        private DystirDBContext dbContext;

        public SponsorsViewController(DystirDBContext dystirDBContext)
        {
            dbContext = dystirDBContext;
        }

        public IActionResult GetSponsors()
        {
            var sponsors = dbContext.Sponsors;
            ObservableCollection<Sponsors> sponsorsList = new ObservableCollection<Sponsors>(sponsors?.OrderBy(a => Guid.NewGuid()) ?? Enumerable.Empty<Sponsors>());
            return PartialView("~/Views/PartialViews/SponsorsPartial.cshtml", sponsorsList);
        }

        public IActionResult GetMainSponsors()
        {
            var sponsors = dbContext.Sponsors;
            ObservableCollection<Sponsors> sponsorsList = new ObservableCollection<Sponsors>(sponsors?.Where(x => x.SponsorId < 100).OrderBy(a => Guid.NewGuid()) ?? Enumerable.Empty<Sponsors>());
            return PartialView("~/Views/PartialViews/SponsorsPartial.cshtml", sponsorsList);
        }
    }
}