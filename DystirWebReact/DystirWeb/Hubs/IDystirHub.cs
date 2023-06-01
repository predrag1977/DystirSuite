using DystirWeb.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DystirWeb.Hubs
{
    public interface IDystirHub
    {
        Task SendMatchDetails(MatchDetails matchDetails);
    }
}
