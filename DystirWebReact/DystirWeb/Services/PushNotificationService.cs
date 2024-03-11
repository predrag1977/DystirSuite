using System.Collections.ObjectModel;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using DystirWeb.Shared;
using Azure;

namespace DystirWeb.Services
{
	public class PushNotificationService
	{
        private FirebaseApp firebaseApp;
        private FirebaseMessaging messaging;

        public PushNotificationService()
		{
		}

        public async void SendNotificationFromMatchAsync(Matches match)
        {
            try
            {
                if (FirebaseApp.DefaultInstance == null)
                {
                    firebaseApp = FirebaseApp.Create(new AppOptions()
                    {
                        Credential = GoogleCredential.FromFile("PushNotificationFiles/serviceAccountKey.json")
                        .CreateScoped("https://www.googleapis.com/auth/firebase.messaging")
                    });
                }
                else
                {
                    firebaseApp = FirebaseApp.DefaultInstance;
                }

                IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "matchID", match.MatchID.ToString() }
            };
                ReadOnlyDictionary<string, string> readonlyDict = data.AsReadOnly();

                messaging = FirebaseMessaging.GetMessaging(firebaseApp);

                // This registration token comes from the client FCM SDKs.
                var registrationToken = "esyfRo3OO03aiHRsIMe8Vl:APA91bFV0NAGjOBp77fpxieG4trMyM4t_UAXIxrwy_KyFk1HHieaW9JrUvsdwIZESozY98tYiyl61r0iTz0CH8W_AT_3NIkw2_pTXiUMN1jcKh3nEOrHrceMT3d1JXfUouRY7Lmt5q-A";

                // See documentation on defining a message payload.
                var message = new Message
                {
                    Token = registrationToken,
                    Notification = new Notification()
                    {
                        Title = $"{match.HomeTeam} - {match.AwayTeam}",
                        Body = $"{match.HomeTeamScore}:{match.AwayTeamScore}" +
                        $"\n{match.StatusName}",
                        ImageUrl = "https://www.dystir.fo/team_logos/ab.png"
                    },
                    Apns = new ApnsConfig()
                    {
                        Aps = new Aps()
                        {
                            Sound = "PushNotificationFiles/referee_whistle.mp3",
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
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
            
        }
    }
}

