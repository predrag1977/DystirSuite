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
    public class PlayersOfMatchesController : ControllerBase
    {
        private DystirDBContext _dystirDBContext;

        private readonly IHubContext<DystirHub> _hubContext;

        public PlayersOfMatchesController(IHubContext<DystirHub> hubContext, DystirDBContext dystirDBContext)
        {
            if (hubContext != null)
            {
                _hubContext = hubContext;
            }
            _dystirDBContext = dystirDBContext;
        }

        // GET: api/PlayersOfMatches?matchID=1203
        [HttpGet("{matchID}", Name = "GetPlayersOfMatchByMatchID")]
        public IEnumerable<PlayersOfMatches> GetPlayersOfMatchesByMatchID(int matchID)
        {
            var playersOfMatchList = _dystirDBContext.PlayersOfMatches.Where(x => x.MatchId == matchID);
            return SortedPlayersOfMatchList(playersOfMatchList) ?? Enumerable.Empty<PlayersOfMatches>();
        }

        private IQueryable<PlayersOfMatches> SortedPlayersOfMatchList(IQueryable<PlayersOfMatches> playersOfMatchList)
        {
            return playersOfMatchList?
                .OrderBy(x => x.PlayingStatus == 3)
                .ThenBy(x => x.PlayingStatus == 0)
                .ThenBy(x => x.PlayingStatus == 2)
                .ThenBy(x => x.PlayingStatus == 1)
                .ThenByDescending(x => x.Position == "GK")
                .ThenBy(x => x.Number == null)
                .ThenBy(x => x.Number)
                .ThenBy(x => x.Position == null)
                .ThenBy(x => x.Position == "ATT")
                .ThenBy(x => x.Position == "MID")
                .ThenBy(x => x.Position == "DEF")
                .ThenBy(x => x.Position == "GK")
                .ThenBy(x => x.FirstName)
                .ThenBy(x => x.Lastname);
        }

        // GET: api/PlayersOfMatches?matchID=1203
        [HttpGet(Name = "GetPlayersOfMatchByTeam")]
        public IQueryable<PlayersOfMatches> GetPlayersOfMatchByTeam([FromQuery] string hometeamname, [FromQuery] string awayteamname, [FromQuery] int competitionid, [FromQuery] int selectedmatchid)
        {
            hometeamname = hometeamname != null ? hometeamname.Trim() : string.Empty;
            awayteamname = awayteamname != null ? awayteamname.Trim() : string.Empty;
            Matches selectedMatch = _dystirDBContext.Matches?.FirstOrDefault(x => x.MatchId == selectedmatchid);
            if (selectedMatch == null)
            {
                return Enumerable.Empty<PlayersOfMatches>().AsQueryable();
            }
            selectedMatch.MatchTypeName = selectedMatch.MatchTypeName != null ? selectedMatch.MatchTypeName.Trim() : string.Empty;

            var lastmatches = _dystirDBContext.Matches?.Where(x => x.MatchTypeName.ToLower() == selectedMatch.MatchTypeName.ToLower()
            && (x.StatusId == 12 || x.StatusId == 13)
            && (x.HomeTeam.ToLower() == hometeamname.ToLower()
            || x.AwayTeam.ToLower() == hometeamname.ToLower()
            || x.HomeTeam.ToLower() == awayteamname.ToLower()
            || x.AwayTeam.ToLower() == awayteamname.ToLower())).OrderByDescending(x => x.Time);

            int? hometeamLastMatchID = lastmatches?.FirstOrDefault(x => x.HomeTeam.ToLower() == hometeamname.ToLower()
            || x.AwayTeam.ToLower() == hometeamname.ToLower())?.MatchId;

            int? awayteamLastMatchID = lastmatches?.FirstOrDefault(x => x.HomeTeam.ToLower() == awayteamname.ToLower()
            || x.AwayTeam.ToLower() == awayteamname.ToLower())?.MatchId;

            var playersByMatchID = GetPlayersOfMatchesByMatchID(selectedmatchid).ToList();

            var homePlayersFromLastMatch = hometeamLastMatchID != null ?
                GetPlayersOfMatchesByMatchID((int)hometeamLastMatchID).Where(x => x.TeamName == hometeamname).ToList() : new List<PlayersOfMatches>();
            var awayPlayersFromLastMatch = awayteamLastMatchID != null ?
                GetPlayersOfMatchesByMatchID((int)awayteamLastMatchID).Where(x => x.TeamName == awayteamname).ToList() : new List<PlayersOfMatches>();
            homePlayersFromLastMatch.AddRange(awayPlayersFromLastMatch);

            var playersFromLastMatch = homePlayersFromLastMatch?.Where(x => !playersByMatchID.Any(p => p.PlayerId == x.PlayerId));

            foreach (PlayersOfMatches player in playersFromLastMatch ?? new List<PlayersOfMatches>())
            {
                PlayersOfMatches newPlayerOfMatch = new PlayersOfMatches()
                {
                    MatchId = selectedmatchid,
                    FirstName = player.FirstName,
                    Lastname = player.Lastname,
                    TeamName = player.TeamName,
                    TeamId = player.TeamId,
                    Number = player.Number,
                    PlayerId = player.PlayerId,
                    Position = player.Position,
                    Captain = player.Captain,
                    MatchTypeName = player.MatchTypeName,
                    MatchTypeId = player.MatchTypeId,
                    PlayingStatus = 0,
                    Goal = 0,
                    OwnGoal = 0,
                    SubIn = -1,
                    SubOut = -1,
                    RedCard = 0,
                    YellowCard = 0
                };
                if (newPlayerOfMatch.PlayerId != null)
                {
                    _dystirDBContext.PlayersOfMatches.Add(newPlayerOfMatch);
                }
            }
            try
            {
                _dystirDBContext.SaveChanges();
            }
            catch {}

            var allPlayersList = GetPlayersOfMatchesByMatchID(selectedmatchid)?.ToList();
            var playersListByTeams = _dystirDBContext.Players?
                .Where(x => x.Team.ToLower() == hometeamname.ToLower()
            || x.Team.ToLower() == awayteamname.ToLower()).ToList();

            foreach (Players player in playersListByTeams ?? new List<Players>())
            {
                PlayersOfMatches playerOfMatches = new PlayersOfMatches()
                {
                    FirstName = player.FirstName,
                    Lastname = player.LastName,
                    PlayingStatus = 3,
                    PlayerId = player.PlayerId,
                    MatchId = selectedmatchid,
                    MatchTypeId = competitionid,
                    TeamId = player.TeamId,
                    TeamName = player.Team
                };
                if (playerOfMatches.PlayerId != null && !allPlayersList.Any(pm => pm.PlayerId == playerOfMatches.PlayerId))
                {
                    allPlayersList.Add(playerOfMatches);
                }
                else
                {
                    string name = player.FirstName + " " + player.LastName;
                }
            }
            return SortedPlayersOfMatchList(allPlayersList.AsQueryable());
        }

        // PUT: api/PlayersOfMatches??id=1203
        [HttpPut(Name = "PutPlayersOfMatches")]
        public IActionResult PutPlayersOfMatches([FromQuery]int id, PlayersOfMatches playersOfMatches)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != playersOfMatches.PlayerOfMatchId)
            {
                return BadRequest();
            }

            _dystirDBContext.Entry(playersOfMatches).State = EntityState.Modified;

            try
            {
                _dystirDBContext.SaveChanges();
                Matches match = _dystirDBContext.Matches.FirstOrDefault(x => x.MatchId == playersOfMatches.MatchId);
                HubSend(match);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlayersOfMatchesExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return StatusCode(StatusCodes.Status204NoContent);
        }

        // POST: api/PlayersOfMatches
        [HttpPost]
        public IActionResult PostPlayersOfMatches(PlayersOfMatches playersOfMatches)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (playersOfMatches.PlayerId == null)
            {
                Players newPlayer = SetNewPlayer(playersOfMatches);

                playersOfMatches.PlayerId = newPlayer.PlayerId;
            }
            _dystirDBContext.PlayersOfMatches.Add(playersOfMatches);
            _dystirDBContext.SaveChanges();
            Matches match = _dystirDBContext.Matches.FirstOrDefault(x => x.MatchId == playersOfMatches.MatchId);
            HubSend(match);

            return CreatedAtRoute("DefaultApi", new { id = playersOfMatches.PlayerOfMatchId }, playersOfMatches);
        }

        private Players SetNewPlayer(PlayersOfMatches playersOfMatches)
        {
            Players newPlayer = new Players()
            {
                FirstName = playersOfMatches.FirstName,
                LastName = playersOfMatches.Lastname,
                Team = playersOfMatches.TeamName,
                TeamId = playersOfMatches.TeamId
            };
            _dystirDBContext.Players.Add(newPlayer);
            _dystirDBContext.SaveChanges();
            return newPlayer;
        }

        // DELETE: api/PlayersOfMatches/5
        [HttpDelete("{id}")]
        public IActionResult DeletePlayersOfMatches(int id)
        {
            PlayersOfMatches playersOfMatches = _dystirDBContext.PlayersOfMatches.Find(id);
            if (playersOfMatches == null)
            {
                return NotFound();
            }

            _dystirDBContext.PlayersOfMatches.Remove(playersOfMatches);
            _dystirDBContext.SaveChanges();
            Matches match = _dystirDBContext.Matches.FirstOrDefault(x => x.MatchId == playersOfMatches.MatchId);
            HubSend(match);
            return Ok(playersOfMatches);
        }

        private bool PlayersOfMatchesExists(int id)
        {
            return _dystirDBContext.PlayersOfMatches.Count(e => e.PlayerOfMatchId == id) > 0;
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
            MatchDetails matchDetails = new MatchDetailsController(_dystirDBContext).Get(match.MatchId);
            hubSender.SendMatchDetails(_hubContext, matchDetails);
        }
    }
}