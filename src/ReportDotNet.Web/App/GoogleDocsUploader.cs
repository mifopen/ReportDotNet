using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using File = Google.Apis.Drive.v3.Data.File;

namespace ReportDotNet.Web.App
{
    public class GoogleDocsUploader
    {
        private static readonly string[] Scopes = {DriveService.Scope.DriveFile};

        public async Task<string> Update(byte[] source)
        {
            UserCredential credential;

            using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                                 GoogleClientSecrets.Load(stream).Secrets,
                                 Scopes,
                                 "user",
                                 CancellationToken.None,
                                 new FileDataStore("token.json", true)
                             );
            }

            var driveService = new DriveService(new BaseClientService.Initializer
                                                {
                                                    HttpClientInitializer = credential,
                                                    ApplicationName = "Google Docs API .NET Quickstart"
                                                });


            var fileMetadata = new File
                               {
                                   // Name = Guid.NewGuid().ToString(),
                                   // MimeType = "application/vnd.google-apps.document"
                               };
            await using (var stream = new MemoryStream(source))
            {
                var request = driveService.Files.Update(fileMetadata,
                                                        "1K4_XzsMcgzIYjgNDgbMQUAMSNQibZy3Z92Y188AjTzk",
                                                        stream,
                                                        "application/vnd.openxmlformats-officedocument.wordprocessingml.document");

                request.Fields = "id";
                await request.UploadAsync();
                return request.ResponseBody.Id;
            }
        }
    }
}