using System;
using System.Collections.Generic;
using System.Linq;
using DystirWeb.ApiControllers;
using DystirWeb.Models;
using DystirWeb.ModelViews;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace DystirWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsOfMatchesController : ControllerBase
    {
        private DystirDBContext db;
        private readonly IHubContext<DystirHub> _hubContext;

        public EventsOfMatchesController(IHubContext<DystirHub> hubContext, DystirDBContext dystirDBContext)
        {
            if (hubContext != null)
            {
                _hubContext = hubContext;
            }
            db = dystirDBContext;
        }

        // GET: api/EventsOfMatches?MatchId=5
        public IEnumerable<EventsOfMatches> GetEventsOfMatchesByMatchId(int MatchId)
        {
            IEnumerable<EventsOfMatches> eventsList = db.EventsOfMatches.Where(x => x.MatchId == MatchId);
            var sortedEventList = eventsList?
                .OrderByDescending(x => x.EventPeriodId ?? 0)
                .ThenByDescending(x => x.EventTotalTime)
                .ThenByDescending(x => x.EventMinute)
                .ThenByDescending(x => x.EventOfMatchId);
            return sortedEventList ?? Enumerable.Empty<EventsOfMatches>();
        }

        private void SetPlayersListByEventsOfMatch(List<EventsOfMatches> eventsOfMatch, Matches selectedMatch)
        {
            if (selectedMatch != null)
            {
                selectedMatch.HomeTeamScore = eventsOfMatch?.Where(x => (x.EventName?.ToUpper() == "GOAL" || x.EventName?.ToUpper() == "PENALTYSCORED" || x.EventName?.ToUpper() == "OWNGOAL") && x.EventTeam?.ToUpper().Trim() == selectedMatch.HomeTeam.ToUpper().Trim())?.Count();
                selectedMatch.AwayTeamScore = eventsOfMatch?.Where(x => (x.EventName?.ToUpper() == "GOAL" || x.EventName?.ToUpper() == "PENALTYSCORED" || x.EventName?.ToUpper() == "OWNGOAL") && x.EventTeam?.ToUpper().Trim() == selectedMatch.AwayTeam.ToUpper().Trim())?.Count();
                selectedMatch.HomeTeamOnTarget = eventsOfMatch?.Where(x => x.EventName?.ToUpper() == "ONTARGET" && x.EventTeam?.ToUpper().Trim() == selectedMatch.HomeTeam.ToUpper().Trim())?.Count();
                selectedMatch.AwayTeamOnTarget = eventsOfMatch?.Where(x => x.EventName?.ToUpper() == "ONTARGET" && x.EventTeam?.ToUpper().Trim() == selectedMatch.AwayTeam.ToUpper().Trim())?.Count();
                selectedMatch.HomeTeamCorner = eventsOfMatch?.Where(x => x.EventName?.ToUpper() == "CORNER" && x.EventTeam?.ToUpper().Trim() == selectedMatch.HomeTeam.ToUpper().Trim())?.Count();
                selectedMatch.AwayTeamCorner = eventsOfMatch?.Where(x => x.EventName?.ToUpper() == "CORNER" && x.EventTeam?.ToUpper().Trim() == selectedMatch.AwayTeam.ToUpper().Trim())?.Count();

                var playersOfMatch = db.PlayersOfMatches.Where(x => x.MatchId == selectedMatch.MatchId)?.ToList();
                foreach (PlayersOfMatches playerOfMatch in playersOfMatch ?? new List<PlayersOfMatches>())
                {
                    playerOfMatch.Goal = 0;
                    playerOfMatch.OwnGoal = 0;
                    playerOfMatch.YellowCard = 0;
                    playerOfMatch.RedCard = 0;
                    playerOfMatch.SubIn = -1;
                    playerOfMatch.SubOut = -1;
                    playerOfMatch.Assist = 0;
                }
                foreach (EventsOfMatches eventMatch in eventsOfMatch ?? new List<EventsOfMatches>())
                {
                    PlayersOfMatches playerOfMatch = playersOfMatch?.FirstOrDefault(x => x.PlayerOfMatchId == eventMatch.MainPlayerOfMatchId);
                    if (playerOfMatch != null)
                    {
                        PlayersOfMatchValuesFromEvent(playerOfMatch, eventMatch);
                    }
                }
                db.SaveChanges();
            }
        }

        private void PlayersOfMatchValuesFromEvent(PlayersOfMatches playerOfMatch, EventsOfMatches eventMatch)
        {
            try
            {
                switch (eventMatch?.EventName?.ToUpper())
                {
                    case "GOAL":
                    case "PENALTYSCORED":
                        playerOfMatch.Goal += 1;
                        break;
                    case "OWNGOAL":
                        playerOfMatch.OwnGoal += 1;
                        break;
                    case "YELLOW":
                        playerOfMatch.YellowCard += 1;
                        break;
                    case "RED":
                        playerOfMatch.RedCard += 1;
                        break;
                    case "ASSIST":
                        playerOfMatch.Assist += 1;
                        break;
                    case "SUBSTITUTION":
                        int subsMinute = 0;
                        string eventMinute = eventMatch.EventMinute.TrimStart('0').Replace("'", "");
                        if (string.IsNullOrWhiteSpace(eventMinute))
                        {
                            eventMinute = "0";
                        }
                        switch (eventMatch.EventPeriodId)
                        {
                            case 3:
                                eventMinute = "45";
                                break;
                            case 5:
                                eventMinute = "90";
                                break;
                            case 7:
                                eventMinute = "105";
                                break;
                            case 9:
                            case 10:
                            case 12:
                            case 13:
                            case 14:
                            case 20:
                            case 30:
                                eventMinute = "120";
                                break;
                        }
                        int index = eventMinute.IndexOf('+');
                        if (index > 0)
                        {
                            subsMinute = Convert.ToInt32(eventMinute.Substring(0, index));
                        }
                        else
                        {
                            subsMinute = Convert.ToInt32(eventMinute);
                        }
                        playerOfMatch.SubOut = subsMinute;
                        PlayersOfMatches secondPlayersOfMatches = db.PlayersOfMatches.FirstOrDefault(x => x.PlayerOfMatchId == eventMatch.SecondPlayerOfMatchId);
                        if (secondPlayersOfMatches != null)
                        {
                            secondPlayersOfMatches.SubIn = subsMinute;
                        }
                        break;
                }
            }
            catch
            {

            };
        }


        // GET: api/EventsOfMatches/5
        [HttpGet("{id}", Name = "GetEventOfMatch")]
        public IActionResult GetEventsOfMatches(int id)
        {
            EventsOfMatches eventsOfMatches = db.EventsOfMatches.Find(id);
            if (eventsOfMatches == null)
            {
                return NotFound();
            }

            return Ok(eventsOfMatches);
        }

        // PUT: api/EventsOfMatches/5
        [HttpPut("{id}")]
        public IActionResult PutEventsOfMatches(int id, EventsOfMatches eventsOfMatches)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != eventsOfMatches?.EventOfMatchId)
            {
                return BadRequest();
            }
            try
            {
                if (eventsOfMatches.EventName != null)
                {
                    db.Entry(eventsOfMatches).State = EntityState.Modified;
                    RemoveSecondEvent(eventsOfMatches);
                    string sendingText = eventsOfMatches.EventText;
                    eventsOfMatches.EventMinute = GetEventMinute(eventsOfMatches);
                    CreateTextOfEvent(eventsOfMatches);
                    db.SaveChanges();
                    AddSecondEvent(sendingText, eventsOfMatches);
                    SetNewEventsList((int)eventsOfMatches.MatchId);
                    Matches match = db.Matches.FirstOrDefault(x => x.MatchId == eventsOfMatches.MatchId);
                    HubSend(match);
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EventsOfMatchesExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok();
        }

        // POST: api/EventsOfMatches
        [HttpPost]
        public IActionResult PostEventsOfMatches(EventsOfMatches eventsOfMatches)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (eventsOfMatches != null && !string.IsNullOrEmpty(eventsOfMatches.EventName))
            {
                string sendingText = eventsOfMatches.EventText;
                eventsOfMatches.EventMinute = GetEventMinute(eventsOfMatches);
                CreateTextOfEvent(eventsOfMatches);
                db.EventsOfMatches.Add(eventsOfMatches);
                db.SaveChanges();

                AddSecondEvent(sendingText, eventsOfMatches);
                SetNewEventsList((int)eventsOfMatches.MatchId);
            }
            Matches match = db.Matches.FirstOrDefault(x => x.MatchId == eventsOfMatches.MatchId);
            HubSend(match);

            return Ok();
        }

        // DELETE: api/EventsOfMatches/5
        [HttpDelete("{id}")]
        public IActionResult DeleteEventsOfMatches(int id)
        {
            EventsOfMatches eventsOfMatches = db.EventsOfMatches.Find(id);
            if (eventsOfMatches == null)
            {
                return NotFound();
            }

            RemoveSecondEvent(eventsOfMatches);
            db.EventsOfMatches.Remove(eventsOfMatches);
            db.SaveChanges();
            SetNewEventsList((int)eventsOfMatches.MatchId);
            Matches match = db.Matches.FirstOrDefault(x => x.MatchId == eventsOfMatches.MatchId);
            HubSend(match);
            return Ok(eventsOfMatches.EventOfMatchId);
        }

        private string GetEventMinute(EventsOfMatches eventsOfMatches)
        {
            string liveMatchTime = string.Empty;
            if (eventsOfMatches?.MatchId != null)
            {
                int minutes = 0;
                int seconds = 0;
                string[] totalTimeArray = eventsOfMatches?.EventTotalTime?.Split(':');
                if (totalTimeArray != null && totalTimeArray.Length == 2)
                {
                    string totalMinutes = totalTimeArray[0].TrimStart('0').TrimStart('0');
                    totalMinutes = string.IsNullOrWhiteSpace(totalMinutes) ? "0" : totalMinutes;
                    minutes = int.Parse(totalMinutes) + 1;
                    string totalSeconds = totalTimeArray[1].TrimStart('0').TrimStart('0');
                    totalSeconds = string.IsNullOrWhiteSpace(totalSeconds) ? "0" : totalSeconds;
                    seconds = int.Parse(totalSeconds);
                }

                string addTime = string.Empty;
                switch (eventsOfMatches.EventPeriodId)
                {
                    case 1:
                        return "00'";
                    case 2:
                        if (minutes > 45)
                        {
                            addTime = "45+";
                            minutes = minutes - 45;
                        }
                        break;
                    case 3:
                        return "hálvleikur";
                    case 4:
                        if (minutes > 90)
                        {
                            addTime = "90+";
                            minutes = minutes - 90;
                        }
                        break;
                    case 5:
                        return "liðugt";
                    case 6:
                        if (minutes > 105)
                        {
                            addTime = "105+";
                            minutes = minutes - 105;
                        }
                        break;
                    case 7:
                        return "longd leiktíð hálvleikur";
                    case 8:
                        if (minutes > 120)
                        {
                            addTime = "120+";
                            minutes = minutes - 120;
                        }
                        break;
                    case 9:
                        return "longd leiktíð liðugt";
                    case 10:
                        return "brotsspark";
                    default:
                        return "liðugt";
                }
                string min = minutes.ToString();
                string sec = seconds.ToString();
                if (minutes < 10)
                {
                    min = "0" + minutes;
                }
                if (seconds < 10)
                {
                    sec = "0" + seconds;
                }

                liveMatchTime = addTime + " " + min + ":" + sec;
            }
            return (liveMatchTime.Split(':')?[0] + "'").Trim();
        }

        private void CreateTextOfEvent(EventsOfMatches eventsOfMatches)
        {
            PlayersOfMatches playersOfMatches = db.PlayersOfMatches.FirstOrDefault(x => x.PlayerOfMatchId == eventsOfMatches.MainPlayerOfMatchId);
            string mainPlayerFullName = (playersOfMatches?.FirstName?.Trim() + " " + playersOfMatches?.Lastname?.Trim())?.Trim();
            switch (eventsOfMatches?.EventName?.ToUpper())
            {
                case "GOAL":
                    eventsOfMatches.EventText = "MÁL til " + eventsOfMatches.EventTeam + ". " + (string.IsNullOrWhiteSpace(mainPlayerFullName) ? "" : "Málskjútti " + mainPlayerFullName + ".");
                    break;
                case "OWNGOAL":
                    eventsOfMatches.EventText = "MÁL til " + eventsOfMatches.EventTeam + ". " + (string.IsNullOrWhiteSpace(mainPlayerFullName) ? "" : "Sjálvmál " + mainPlayerFullName + ".");
                    break;
                case "YELLOW":
                    eventsOfMatches.EventText = "GULKORT " + eventsOfMatches.EventTeam + (string.IsNullOrWhiteSpace(mainPlayerFullName) ? "." : " leikari " + mainPlayerFullName + ".");
                    break;
                case "RED":
                    eventsOfMatches.EventText = "REYTTKORT " + eventsOfMatches.EventTeam + (string.IsNullOrWhiteSpace(mainPlayerFullName) ? "." : " leikari " + mainPlayerFullName + ".");
                    break;
                case "CORNER":
                    eventsOfMatches.EventText = "HORNASPARK til " + eventsOfMatches.EventTeam + ".";
                    break;
                case "ONTARGET":
                    eventsOfMatches.EventText = "ROYND Á MÁL. " + eventsOfMatches.EventTeam + " leikari" + (string.IsNullOrWhiteSpace(mainPlayerFullName) ? "" : " " + mainPlayerFullName) + " roynd á mál.";
                    break;
                case "OFFTARGET":
                    eventsOfMatches.EventText = "ROYND FRAMMVIÐ MÁL. " + eventsOfMatches.EventTeam + " leikari" + (string.IsNullOrWhiteSpace(mainPlayerFullName) ? "" : " " + mainPlayerFullName) + " roynd frammvið mál.";
                    break;
                case "BLOCKEDSHOT":
                    eventsOfMatches.EventText = "BLOKERA SKOT. " + eventsOfMatches.EventTeam + " leikari" + (string.IsNullOrWhiteSpace(mainPlayerFullName) ? "" : " " + mainPlayerFullName) + " roynd er blokerað.";
                    break;
                case "BIGCHANCE":
                    eventsOfMatches.EventText = "STÓRUR MØGULEIKI. " + eventsOfMatches.EventTeam + " leikari" + (string.IsNullOrWhiteSpace(mainPlayerFullName) ? "" : " " + mainPlayerFullName) + " stórur mál møguleiki.";
                    break;
                case "SUBSTITUTION":
                    PlayersOfMatches secondPlayersOfMatches = db.PlayersOfMatches.FirstOrDefault(x => x.PlayerOfMatchId == eventsOfMatches.SecondPlayerOfMatchId);
                    string secongPlayerFullName = (secondPlayersOfMatches?.FirstName?.Trim() + " " + secondPlayersOfMatches?.Lastname?.Trim())?.Trim();
                    eventsOfMatches.EventText = "ÚTSKIFTING " + eventsOfMatches.EventTeam + ". " + (string.IsNullOrWhiteSpace(mainPlayerFullName) ? "" : "Leikari " + mainPlayerFullName + " út. ") + (string.IsNullOrWhiteSpace(secongPlayerFullName) ? "" : "Leikari " + secongPlayerFullName + " inn.");
                    break;
                case "ASSIST":
                    eventsOfMatches.EventText = "UPPLEGG " + eventsOfMatches.EventTeam + (string.IsNullOrWhiteSpace(mainPlayerFullName) ? "." : " leikari " + mainPlayerFullName + ".");
                    break;
                case "PENALTY":
                    eventsOfMatches.EventText = "BROTSSPARK til " + eventsOfMatches.EventTeam + ".";
                    break;
                case "PENALTYSCORED":
                    eventsOfMatches.EventText = "BROTSSPARK SKORA " + eventsOfMatches.EventTeam + (string.IsNullOrWhiteSpace(mainPlayerFullName) ? "." : " leikari " + mainPlayerFullName + ".");
                    break;
                case "PENALTYMISSED":
                    eventsOfMatches.EventText = "BROTSSPARK BRENT " + eventsOfMatches.EventTeam + (string.IsNullOrWhiteSpace(mainPlayerFullName) ? "." : " leikari " + mainPlayerFullName + ".");
                    break;
                case "COMMENTARY":
                    eventsOfMatches.EventText = "HENDINGAR: " + eventsOfMatches.EventText;
                    break;
            }
        }

        private void SetNewEventsList(int MatchId)
        {
            Matches match = (new MatchesController(null, db).GetMatches(MatchId) as OkObjectResult).Value as Matches;
            if (match != null)
            {
                var selectedMatch = db.Matches.FirstOrDefault(x => x.MatchId == MatchId);
                var eventsList = db.EventsOfMatches.Where(x => x.MatchId == MatchId);
                SetPlayersListByEventsOfMatch(eventsList.ToList(), selectedMatch);
            }
        }

        private bool EventsOfMatchesExists(int id)
        {
            return db.EventsOfMatches.Count(e => e.EventOfMatchId == id) > 0;
        }

        private void AddSecondEvent(string sendingText, EventsOfMatches eventsOfMatches)
        {
            if (eventsOfMatches.EventName == "ONTARGET"
                        || eventsOfMatches.EventName == "OFFTARGET"
                        || eventsOfMatches.EventName == "BLOCKEDSHOT")
            {
                var eventID = eventsOfMatches.EventOfMatchId;
                if (!string.IsNullOrEmpty(sendingText))
                {
                    EventsOfMatches newBigChanceEvent = eventsOfMatches;
                    newBigChanceEvent.EventOfMatchId = 0;
                    newBigChanceEvent.EventName = "BIGCHANCE";
                    newBigChanceEvent.EventMinute = GetEventMinute(newBigChanceEvent);
                    CreateTextOfEvent(newBigChanceEvent);
                    db.EventsOfMatches.Add(newBigChanceEvent);
                    db.SaveChanges();
                    EventsOfMatches currentEvent = db.EventsOfMatches.Find(eventID);
                    if (currentEvent != null)
                    {
                        currentEvent.SecondPlayerOfMatchId = newBigChanceEvent.EventOfMatchId;
                    }
                    db.SaveChanges();
                }
            }
        }

        private void RemoveSecondEvent(EventsOfMatches eventsOfMatches)
        {
            if (eventsOfMatches.EventName == "ONTARGET"
                        || eventsOfMatches.EventName == "OFFTARGET"
                        || eventsOfMatches.EventName == "BLOCKEDSHOT")
            {
                EventsOfMatches bindEvent = db.EventsOfMatches.Find(eventsOfMatches.SecondPlayerOfMatchId);
                if (bindEvent != null)
                {
                    db.EventsOfMatches.Remove(bindEvent);
                    db.SaveChanges();
                }
                eventsOfMatches.SecondPlayerOfMatchId = 0;
            }
        }

        private void HubSend(Matches match)
        {
            HubSender hubSender = new HubSender();
            hubSender.SendMatch(_hubContext, match);
            HubSendMatchDetails(match);
        }

        private void HubSendMatchDetails(Matches match)
        {
            HubSender hubSender = new HubSender();
            MatchDetails matchDetails = new MatchDetailsController(db).Get(match.MatchId);
            hubSender.SendMatchDetails(_hubContext, matchDetails);
            HubFullMatchedData(matchDetails);
        }

        private void HubFullMatchedData(MatchDetails matchDetails)
        {
            HubSender hubSender = new HubSender();
            var allMatches = new MatchesController(null, db).GetMatches("")?.ToList();
            hubSender.SendFullMatchesData(_hubContext, matchDetails, allMatches);
        }
    }
}