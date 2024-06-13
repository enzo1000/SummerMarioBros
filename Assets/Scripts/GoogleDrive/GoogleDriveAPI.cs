using Google.Apis;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
using System.Threading.Tasks;

public class GoogleDriveAPI : MonoBehaviour
{
    public static GoogleDriveAPI instance;

    private void Awake()
    {
        if (instance != null)
        {
            UnityEngine.Debug.LogWarning("More than one instance of GoogleDriveAPI found!");
            return;
        }
        instance = this;
    }

    string UploadFileName = Path.Combine(Application.dataPath, "GameData.csv");
    string keyFilePath = Path.Combine(Application.streamingAssetsPath + "/key.p12");

    public async void SendData()
    {
        try
        {
            string serviceAccountEmail = "utilisateur01@unitysummermario.iam.gserviceaccount.com";
            var certificate = new X509Certificate2(keyFilePath, "notasecret", X509KeyStorageFlags.Exportable);

            ServiceAccountCredential credential = new ServiceAccountCredential(
                    new ServiceAccountCredential.Initializer(serviceAccountEmail)
                    {
                        Scopes = new[] { DriveService.Scope.Drive }
                    }.FromCertificate(certificate)
            );

            //Create the service
            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "SummerMario"
            });

            string fileName = DateTime.Now.ToString();
            //Metadata du fichier à envoyer
            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = fileName,
                Parents = new List<string>() { "13AYz0s3EncObzSWeIXo5G0RkJqkS2E8F" },
            };

            //Envoie du fichier
            await using (var fsSource = new FileStream(UploadFileName, FileMode.Open, FileAccess.Read))
            {
                var request = service.Files.Create(fileMetadata, fsSource, "text/plain");
                request.Fields = "*";
                var results = await request.UploadAsync(CancellationToken.None);
                    
                if (results.Status == Google.Apis.Upload.UploadStatus.Failed)
                {
                    UnityEngine.Debug.Log("Error Uploading File");
                }
                else
                {
                    UnityEngine.Debug.Log("File uploaded");
                }
            }
        }
        catch (Exception e)
        {
            if (e is AggregateException)
            {
                UnityEngine.Debug.Log("Credential Not Found");
            }
            else
            {
                throw;
            }
        }
    }
}
