using Google.Apis;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;

namespace Gooogle_Drive_OAuth2_Example
{
    public class GoogleDriveAPI
    {
        private static UserCredential Login(string googleClientId, string googleClientSecret)
        {
            ClientSecrets secrets = new ClientSecrets()
            {
                ClientId = googleClientId,
                ClientSecret = googleClientSecret
            };

            return GoogleWebAuthorizationBroker.AuthorizeAsync(secrets,
                new[] { "https://www.googleapis.com/auth/drive.file" },
                "user", System.Threading.CancellationToken.None).Result;
        }

        static void Main()
        {
            string googleClientId = "CLIENT ID HERE";
            string googleClientSecret = "CLIENT SECRET HERE";

            UserCredential credential = Login(googleClientId, googleClientSecret);
            using (var driveService = new DriveService(new BaseClientService.Initializer() { HttpClientInitializer = credential }))
            {

            }
        }
    }
}
