﻿using DystirWeb.DystirDB;
using DystirWeb.Hubs;
using DystirWeb.Services;
using DystirWeb.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace DystirWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsOfMatchesController : ControllerBase
    {
        private readonly IHubContext<DystirHub> _hubContext;
        private readonly AuthService _authService;
        private DystirDBContext _dystirDBContext;
        private readonly DystirService _dystirService;
        private readonly MatchDetailsService _matchDetailsService;
        private readonly PushNotificationService _pushNotificationService;

        public EventsOfMatchesController(IHubContext<DystirHub> hubContext,
            AuthService authService,
            DystirDBContext dystirDBContext,
            DystirService dystirService,
            MatchDetailsService matchDetailsService,
            PushNotificationService pushNotificationService)
        {
            _hubContext = hubContext;
            _authService = authService;
            _dystirDBContext = dystirDBContext;
            _dystirService = dystirService;
            _matchDetailsService = matchDetailsService;
            _pushNotificationService = pushNotificationService;
        }

        // GET: api/EventsOfMatches?MatchId=5
        public IEnumerable<EventsOfMatches> GetEventsOfMatchesByMatchId(int matchID)
        {
            return _matchDetailsService.GetEventsOfMatches(matchID);
        }

        // GET: api/EventsOfMatches/5
        [HttpGet("{id}", Name = "GetEventOfMatch")]
        public IActionResult GetEventsOfMatches(int id)
        {
            EventsOfMatches eventsOfMatches = _dystirDBContext.EventsOfMatches.Find(id);
            if (eventsOfMatches == null)
            {
                return NotFound();
            }

            return Ok(eventsOfMatches);
        }

        // PUT: api/EventsOfMatches/5
        [HttpPut("{id}/{token}")]
        public IActionResult PutEventsOfMatches(int id, string token, [FromBody] EventsOfMatches eventsOfMatches)
        {
            if (!_authService.IsAuthorized(token))
            {
                return BadRequest(new UnauthorizedAccessException().Message);
            }

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
                    _dystirDBContext.Entry(eventsOfMatches).State = EntityState.Modified;
                    RemoveSecondEvent(eventsOfMatches);
                    string sendingText = eventsOfMatches.EventText;
                    eventsOfMatches.EventMinute = GetEventMinute(eventsOfMatches);
                    CreateTextOfEvent(eventsOfMatches);
                    _dystirDBContext.SaveChanges();
                    AddSecondEvent(sendingText, eventsOfMatches);
                    SetNewEventsList((int)eventsOfMatches.MatchId);
                    Matches match = _dystirDBContext.Matches.Find(eventsOfMatches.MatchId);
                    HubSend(match);
                    _pushNotificationService.SendNotificationFromEventAsync(match, eventsOfMatches, "CORRECTION");
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
        [HttpPost("{token}")]
        public IActionResult PostEventsOfMatches(string token, [FromBody] EventsOfMatches eventsOfMatches)
        {
            if (!_authService.IsAuthorized(token))
            {
                return BadRequest(new UnauthorizedAccessException().Message);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                if (eventsOfMatches != null && !string.IsNullOrEmpty(eventsOfMatches.EventName))
                {
                    string sendingText = eventsOfMatches.EventText;
                    eventsOfMatches.EventMinute = GetEventMinute(eventsOfMatches);
                    CreateTextOfEvent(eventsOfMatches);
                    _dystirDBContext.EventsOfMatches.Add(eventsOfMatches);
                    _dystirDBContext.SaveChanges();

                    AddSecondEvent(sendingText, eventsOfMatches);
                    SetNewEventsList((int)eventsOfMatches.MatchId);
                }
                Matches match = _dystirDBContext.Matches.Find(eventsOfMatches.MatchId);
                HubSend(match);
                _pushNotificationService.SendNotificationFromEventAsync(match, eventsOfMatches, "");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EventsOfMatchesExists(eventsOfMatches?.EventOfMatchId ?? 0))
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

        // DELETE: api/EventsOfMatches/5
        [HttpDelete("{id}/{token}")]
        public IActionResult DeleteEventsOfMatches(int id, string token)
        {
            if (!_authService.IsAuthorized(token))
            {
                return BadRequest(new UnauthorizedAccessException().Message);
            }

            EventsOfMatches eventsOfMatches = _dystirDBContext.EventsOfMatches.Find(id);
            if (eventsOfMatches == null)
            {
                return NotFound();
            }

            RemoveSecondEvent(eventsOfMatches);
            _dystirDBContext.EventsOfMatches.Remove(eventsOfMatches);
            _dystirDBContext.SaveChanges();
            SetNewEventsList((int)eventsOfMatches.MatchId);
            Matches match = _dystirDBContext.Matches.Find(eventsOfMatches.MatchId);
            HubSend(match);
            _pushNotificationService.SendNotificationFromEventAsync(match, eventsOfMatches, "DELETE");
            return Ok(eventsOfMatches.EventOfMatchId);
        }

        private void SetPlayersListByEventsOfMatch(List<EventsOfMatches> eventsOfMatch, Matches selectedMatch)
        {
            selectedMatch.HomeTeamScore = eventsOfMatch?.Where(x => (x.EventName?.ToUpper() == "GOAL" || x.EventName?.ToUpper() == "PENALTYSCORED" || x.EventName?.ToUpper() == "OWNGOAL" || x.EventName?.ToUpper() == "DIRECTFREEKICKGOAL") && x.EventTeam?.ToUpper().Trim() == selectedMatch.HomeTeam.ToUpper().Trim())?.Count();
            selectedMatch.AwayTeamScore = eventsOfMatch?.Where(x => (x.EventName?.ToUpper() == "GOAL" || x.EventName?.ToUpper() == "PENALTYSCORED" || x.EventName?.ToUpper() == "OWNGOAL" || x.EventName?.ToUpper() == "DIRECTFREEKICKGOAL") && x.EventTeam?.ToUpper().Trim() == selectedMatch.AwayTeam.ToUpper().Trim())?.Count();
            selectedMatch.HomeTeamOnTarget = eventsOfMatch?.Where(x => x.EventName?.ToUpper() == "ONTARGET" && x.EventTeam?.ToUpper().Trim() == selectedMatch.HomeTeam.ToUpper().Trim())?.Count();
            selectedMatch.AwayTeamOnTarget = eventsOfMatch?.Where(x => x.EventName?.ToUpper() == "ONTARGET" && x.EventTeam?.ToUpper().Trim() == selectedMatch.AwayTeam.ToUpper().Trim())?.Count();
            selectedMatch.HomeTeamCorner = eventsOfMatch?.Where(x => x.EventName?.ToUpper() == "CORNER" && x.EventTeam?.ToUpper().Trim() == selectedMatch.HomeTeam.ToUpper().Trim())?.Count();
            selectedMatch.AwayTeamCorner = eventsOfMatch?.Where(x => x.EventName?.ToUpper() == "CORNER" && x.EventTeam?.ToUpper().Trim() == selectedMatch.AwayTeam.ToUpper().Trim())?.Count();
            selectedMatch.HomeTeamPenaltiesScore = eventsOfMatch?.Where(x => (x.EventName?.ToUpper() == "GOAL" || x.EventName?.ToUpper() == "PENALTYSCORED" || x.EventName?.ToUpper() == "OWNGOAL" || x.EventName?.ToUpper() == "DIRECTFREEKICKGOAL")
            && x.EventTeam?.ToUpper().Trim() == selectedMatch.HomeTeam.ToUpper().Trim()
            && x.EventPeriodId == 10)?.Count();
            selectedMatch.AwayTeamPenaltiesScore = eventsOfMatch?.Where(x => (x.EventName?.ToUpper() == "GOAL" || x.EventName?.ToUpper() == "PENALTYSCORED" || x.EventName?.ToUpper() == "OWNGOAL" || x.EventName?.ToUpper() == "DIRECTFREEKICKGOAL")
            && x.EventTeam?.ToUpper().Trim() == selectedMatch.AwayTeam.ToUpper().Trim()
            && x.EventPeriodId == 10)?.Count();

            var playersOfMatch = _dystirDBContext.PlayersOfMatches.Where(x => x.MatchId == selectedMatch.MatchID)?.ToList();
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
            _dystirDBContext.SaveChanges();
        }

        private void PlayersOfMatchValuesFromEvent(PlayersOfMatches playerOfMatch, EventsOfMatches eventMatch)
        {
            try
            {
                switch (eventMatch?.EventName?.ToUpper())
                {
                    case "GOAL":
                    case "PENALTYSCORED":
                    case "DIRECTFREEKICKGOAL":
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
                        PlayersOfMatches secondPlayersOfMatches = _dystirDBContext.PlayersOfMatches.FirstOrDefault(x => x.PlayerOfMatchId == eventMatch.SecondPlayerOfMatchId);
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
                        return "46";
                    case 4:
                        if (minutes > 90)
                        {
                            addTime = "90+";
                            minutes = minutes - 90;
                        }
                        break;
                    case 5:
                        return "91";
                    case 6:
                        if (minutes > 105)
                        {
                            addTime = "105+";
                            minutes = minutes - 105;
                        }
                        break;
                    case 7:
                        return "106";
                    case 8:
                        if (minutes > 120)
                        {
                            addTime = "120+";
                            minutes = minutes - 120;
                        }
                        break;
                    case 9:
                        return "120";
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
            PlayersOfMatches playersOfMatches = _dystirDBContext.PlayersOfMatches.FirstOrDefault(x => x.PlayerOfMatchId == eventsOfMatches.MainPlayerOfMatchId);
            string mainPlayerFullName = (playersOfMatches?.FirstName?.Trim() + " " + playersOfMatches?.Lastname?.Trim())?.Trim();
            eventsOfMatches.MainPlayerOfMatchNumber = playersOfMatches?.Number?.ToString();
            eventsOfMatches.MainPlayerFullName = mainPlayerFullName;
            switch (eventsOfMatches?.EventName?.ToUpper())
            {
                case "GOAL":
                    eventsOfMatches.EventText = "MÁL til " + eventsOfMatches.EventTeam + ". " + (string.IsNullOrWhiteSpace(mainPlayerFullName) ? "" : "Málskjútti " + mainPlayerFullName + ".");
                    break;
                case "DIRECTFREEKICKGOAL":
                    eventsOfMatches.EventText = "MÁL til " + eventsOfMatches.EventTeam + ". Beinleiðis fríspark. " + (string.IsNullOrWhiteSpace(mainPlayerFullName) ? "" : "Málskjútti " + mainPlayerFullName + ".");
                    eventsOfMatches.EventName = "GOAL";
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
                    (eventsOfMatches.EventText = "HORNASPARK til " + eventsOfMatches.EventTeam + ".").Replace("..", ".");
                    break;
                case "ONTARGET":
                    eventsOfMatches.EventText = "ROYND Á MÁL. " + eventsOfMatches.EventTeam + " leikari" + (string.IsNullOrWhiteSpace(mainPlayerFullName) ? "" : " " + mainPlayerFullName) + " roynd á mál.";
                    break;
                case "OFFTARGET":
                    eventsOfMatches.EventText = "ROYND FRAMVIÐ MÁL. " + eventsOfMatches.EventTeam + " leikari" + (string.IsNullOrWhiteSpace(mainPlayerFullName) ? "" : " " + mainPlayerFullName) + " roynd framvið mál.";
                    break;
                case "BLOCKEDSHOT":
                    eventsOfMatches.EventText = "BLOKERA SKOT. " + eventsOfMatches.EventTeam + " leikari" + (string.IsNullOrWhiteSpace(mainPlayerFullName) ? "" : " " + mainPlayerFullName) + " roynd er blokerað.";
                    break;
                case "BIGCHANCE":
                    eventsOfMatches.EventText = "STÓRUR MØGULEIKI. " + eventsOfMatches.EventTeam + " leikari" + (string.IsNullOrWhiteSpace(mainPlayerFullName) ? "" : " " + mainPlayerFullName) + " stórur mál møguleiki.";
                    break;
                case "SUBSTITUTION":
                    PlayersOfMatches secondPlayersOfMatches = _dystirDBContext.PlayersOfMatches.FirstOrDefault(x => x.PlayerOfMatchId == eventsOfMatches.SecondPlayerOfMatchId);
                    string secongPlayerFullName = (secondPlayersOfMatches?.FirstName?.Trim() + " " + secondPlayersOfMatches?.Lastname?.Trim())?.Trim();
                    eventsOfMatches.SecondPlayerOfMatchNumber = secondPlayersOfMatches?.Number?.ToString();
                    eventsOfMatches.SecondPlayerFullName = secongPlayerFullName;
                    eventsOfMatches.EventText = "ÚTSKIFTING " + eventsOfMatches.EventTeam + ". " + (string.IsNullOrWhiteSpace(mainPlayerFullName) ? "" : "Leikari " + mainPlayerFullName + " út. ") + (string.IsNullOrWhiteSpace(secongPlayerFullName) ? "" : "Leikari " + secongPlayerFullName + " inn.");
                    break;
                case "ASSIST":
                    eventsOfMatches.EventText = "UPPLEGG " + eventsOfMatches.EventTeam + (string.IsNullOrWhiteSpace(mainPlayerFullName) ? "." : " leikari " + mainPlayerFullName + ".");
                    break;
                case "PENALTY":
                    (eventsOfMatches.EventText = "BROTSSPARK til " + eventsOfMatches.EventTeam + ".").Replace("..", ".");
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
                case "PLAYEROFTHEMATCH":
                    eventsOfMatches.EventText = eventsOfMatches.EventTeam + (string.IsNullOrWhiteSpace(mainPlayerFullName) ? "." : " leikari " + mainPlayerFullName + ".");
                    break;
            }
        }

        private void SetNewEventsList(int matchId)
        {
            Matches match = _dystirDBContext.Matches.Find(matchId);
            if (match != null)
            {
                var selectedMatch = match;
                var eventsList = _dystirDBContext.EventsOfMatches.Where(x => x.MatchId == matchId);
                SetPlayersListByEventsOfMatch(eventsList.ToList(), selectedMatch);
            }
        }

        private bool EventsOfMatchesExists(int id)
        {
            return _dystirDBContext.EventsOfMatches.Count(e => e.EventOfMatchId == id) > 0;
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
                    _dystirDBContext.EventsOfMatches.Add(newBigChanceEvent);
                    _dystirDBContext.SaveChanges();
                    EventsOfMatches currentEvent = _dystirDBContext.EventsOfMatches.Find(eventID);
                    if (currentEvent != null)
                    {
                        currentEvent.SecondPlayerOfMatchId = newBigChanceEvent.EventOfMatchId;
                    }
                    _dystirDBContext.SaveChanges();
                }
            }
        }

        private void RemoveSecondEvent(EventsOfMatches eventsOfMatches)
        {
            if (eventsOfMatches.EventName == "ONTARGET"
                        || eventsOfMatches.EventName == "OFFTARGET"
                        || eventsOfMatches.EventName == "BLOCKEDSHOT")
            {
                EventsOfMatches bindEvent = _dystirDBContext.EventsOfMatches.Find(eventsOfMatches.SecondPlayerOfMatchId);
                if (bindEvent != null)
                {
                    _dystirDBContext.EventsOfMatches.Remove(bindEvent);
                    _dystirDBContext.SaveChanges();
                }
                eventsOfMatches.SecondPlayerOfMatchId = 0;
            }
        }

        private void HubSend(Matches match)
        {
            HubSender hubSender = new HubSender();
            hubSender.SendMatch(_hubContext, match);
            HubSendMatchDetails(hubSender, match);
        }

        private async void HubSendMatchDetails(HubSender hubSender, Matches match)
        {
            await _dystirService.UpdateAllMatchesAsync(match);
            MatchDetails matchDetails = _matchDetailsService.GetMatchDetails(match);
            await _dystirService.UpdateMatchesDetailsAsync(matchDetails);
            hubSender.SendMatchDetails(_hubContext, matchDetails);
        }
    }
}