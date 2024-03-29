﻿using System.Collections.ObjectModel;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using DystirWeb.Shared;
using DystirWeb.Models;

namespace DystirWeb.Services
{
    public class PushNotificationService
    {
        private readonly DystirService _dystirService;
        private FirebaseApp firebaseApp;
        private FirebaseMessaging messaging;

        public PushNotificationService(DystirService dystirService)
        {
            _dystirService = dystirService;
        }

        public void SendNotificationFromMatchAsync(Matches match, bool isMatchTimeCorrection, bool isMatchStatusChanged)
        {
            string matchID = match.MatchID.ToString();
            string eventType = (isMatchTimeCorrection && !isMatchStatusChanged) ? $"{EventType.TIME_CORRECTION}" : $"{EventType.TIME}";

            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "matchID", matchID },
                { "event", eventType }
            };
            var eventTypeText = eventType.Equals(EventType.TIME) ? "" : $"{eventType}\n";
            var statusName = GetStatusName(match.StatusID ?? 0);
            
            var title = $"{eventTypeText}{match.HomeTeam} - {match.AwayTeam}";
            var body = $"{match.HomeTeamScore}:{match.AwayTeamScore}\n{statusName}";
            var sound = "whistle.mp3";

            ReadOnlyDictionary<string, string> readOnlyData= data.AsReadOnly();
            SendNotificationAsync(matchID, title, body, sound, readOnlyData);
        }

        public void SendNotificationFromEventAsync(Matches match, EventsOfMatches eventOfMatch, string eventAction)
        {
            string matchID = match.MatchID.ToString();
            var sound = "whistle.mp3";

            var homeTeamScore = (match.HomeTeamScore ?? 0) - (match.HomeTeamPenaltiesScore ?? 0);
            var awayTeamScore = (match.AwayTeamScore ?? 0) - (match.AwayTeamPenaltiesScore ?? 0);
            var homeTeamPenaltiesScore = match.HomeTeamPenaltiesScore ?? 0;
            var awayTeamPenaltiesScore = match.AwayTeamPenaltiesScore ?? 0;

            var homeTeamPenaltiesScoreText = "";
            var awayTeamPenaltiesScoreText = "";
            if ((homeTeamPenaltiesScore > 0 || awayTeamPenaltiesScore > 0) && match.StatusID >= 10)
            {
                homeTeamPenaltiesScoreText = $"({homeTeamPenaltiesScore}) ";
                awayTeamPenaltiesScoreText = $" ({awayTeamPenaltiesScore})";
            }

            var mainPlayer = $"({eventOfMatch.MainPlayerOfMatchNumber}) {eventOfMatch.MainPlayerFullName}";
            var secondPlayer = "";
            if (!string.IsNullOrEmpty(eventOfMatch.SecondPlayerOfMatchNumber) || !string.IsNullOrEmpty(eventOfMatch.SecondPlayerFullName))
            {
                secondPlayer = $"\n({eventOfMatch.SecondPlayerOfMatchNumber}) {eventOfMatch.SecondPlayerFullName}";
            }

            var players = mainPlayer + secondPlayer;
            if(eventOfMatch.EventName.ToUpper() == EventType.SUBSTITUTION)
            {
                players = $"IN: {secondPlayer?.Trim()}\n" +
                          $"OUT: {mainPlayer?.Trim()}";
            }

            var result = $"{homeTeamPenaltiesScoreText}{homeTeamScore}:{awayTeamScore}{awayTeamPenaltiesScoreText}";
            if (!string.IsNullOrEmpty(result))
            {
                result = $"{result}\n";
            }

            if (!string.IsNullOrEmpty(eventAction))
            {
                eventAction = $" - {eventAction}";
            }

            string eventType;
            switch (eventOfMatch.EventName.ToUpper())
            {
                case EventType.GOAL:
                    eventType = EventType.GOAL;
                    sound = "crowd.mp3";
                    break;
                case EventType.OWN_GOAL:
                    eventType = "OWN GOAL";
                    break;
                case EventType.PENALTY_SCORED:
                    eventType = "PENALTY SCORED";
                    break;
                case EventType.PENALTY_MISSED:
                    eventType = "PENALTY MISSED";
                    result = "";
                    break;
                case EventType.YELLOW_CARD:
                    eventType = "YELLOW CARD";
                    result = "";
                    break;
                case EventType.RED_CARD:
                    eventType = "RED CARD";
                    result = "";
                    break;
                case EventType.SUBSTITUTION:
                    eventType = EventType.SUBSTITUTION;
                    result = "";
                    break;
                default:
                    return;
            }

            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "matchID", matchID },
                { "event", eventType }
            };
            var eventTypeText = $"{eventOfMatch.EventMinute} - {eventType}";

            var title = $"{eventTypeText}";
            var body = $"{match.HomeTeam} - {match.AwayTeam}\n" +
                $"{result}" +
                $"{eventOfMatch.EventTeam}{eventAction}\n" +
                $"{players}";

            ReadOnlyDictionary<string, string> readOnlyData = data.AsReadOnly();
            SendNotificationAsync(matchID, title, body, sound, readOnlyData);
        }

        private async void SendNotificationAsync(string matchID, string title, string body, string sound, ReadOnlyDictionary<string, string> readOnlyData)
        {
            if (FirebaseApp.DefaultInstance == null)
            {
                firebaseApp = FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile("serviceAccountKey.json")
                    .CreateScoped("https://www.googleapis.com/auth/firebase.messaging")
                });
            }
            else
            {
                firebaseApp = FirebaseApp.DefaultInstance;
            }

            messaging = FirebaseMessaging.GetMessaging(firebaseApp);

            var managers = _dystirService.DystirDBContext.Managers;

            foreach (var manager in managers?.ToList() ?? new List<Manager>())
            {
                try
                {
                    var matchIDs = manager.MatchID.Split(";");
                    if ((matchIDs?.Length ?? 0) > 0 && matchIDs.Contains(matchID))
                    {
                        // This registration token comes from the client FCM SDKs.
                        var registrationToken = manager.DeviceToken;

                        // See documentation on defining a message payload.
                        var message = new Message
                        {
                            Token = registrationToken,
                            Notification = new Notification()
                            {
                                Title = title,
                                Body = body,
                                ImageUrl = default
                            },

                            Apns = new ApnsConfig()
                            {
                                Aps = new Aps()
                                {
                                    Sound = sound,
                                    Badge = 0
                                }
                            },
                            Data = readOnlyData
                        };

                        // Send a message to the device corresponding to the provided
                        // registration token.
                        string response = await messaging.SendAsync(message);
                        // Response is a message ID string.
                        Console.WriteLine("Successfully sent message: " + response);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception: " + ex.Message);
                }

            }
        }

        private string GetStatusName(int statusID)
        {
            switch (statusID)
            {
                case 1:
                    return "START SOON";
                case 2:
                    return "1.HALF";
                case 3:
                    return "HALF TIME";
                case 4:
                    return "2.HALF";
                case 5:
                    return "FULL TIME";
                case 6:
                    return "1.EXTRA TIME";
                case 7:
                    return "EXTRA TIME PAUSE";
                case 8:
                    return "2.EXTRA TIME";
                case 9:
                    return "EXTRA TIME FINISHED";
                case 10:
                    return "PENALTIES";
                case 11:
                case 12:
                case 13:
                    return "FINISHED";
                default:
                    return "NO DEFINED";
            }
        }
    }

    struct EventType
    {
        public const string TIME = "TIME";
        public const string TIME_CORRECTION= "TIME CORRECTION";
        public const string GOAL = "GOAL";
        public const string OWN_GOAL = "OWNGOAL";
        public const string PENALTY_SCORED = "PENALTYSCORED";
        public const string PENALTY_MISSED = "PENALTYMISSED";
        public const string YELLOW_CARD = "YELLOW";
        public const string RED_CARD = "RED";
        public const string SUBSTITUTION = "SUBSTITUTION";
    }
}

