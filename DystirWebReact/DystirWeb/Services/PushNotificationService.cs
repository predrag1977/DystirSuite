using System.Collections.ObjectModel;
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

        public async void SendNotificationFromMatchAsync(Matches match)
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

            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "matchID", match.MatchID.ToString() },
                { "event", "TIME" }
            };
            ReadOnlyDictionary<string, string> readonlyDict = data.AsReadOnly();

            messaging = FirebaseMessaging.GetMessaging(firebaseApp);

            var managers = _dystirService.DystirDBContext.Managers;

            foreach (var manager in managers?.ToList() ?? new List<Manager>())
            {
                try
                {
                    var matchIDs = manager.MatchID.Split(";");
                    if ((matchIDs?.Length ?? 0) > 0 && matchIDs.Contains(match.MatchID.ToString()))
                    {
                        // This registration token comes from the client FCM SDKs.
                        var registrationToken = manager.DeviceToken;

                        // See documentation on defining a message payload.
                        var message = new Message
                        {
                            Token = registrationToken,
                            Notification = new Notification()
                            {
                                Title = $"{match.HomeTeam} - {match.AwayTeam}",
                                Body = $"{match.HomeTeamScore}:{match.AwayTeamScore}" +
                                $"\n{match.StatusName}",
                                ImageUrl = default
                            },

                            Apns = new ApnsConfig()
                            {
                                Aps = new Aps()
                                {
                                    Sound = readonlyDict["event"].Equals("GOAL") ? "crowd.mp3" : "whistle.mp3",
                                    Badge = 0
                                }
                            },
                            Data = readonlyDict
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
    }
}

