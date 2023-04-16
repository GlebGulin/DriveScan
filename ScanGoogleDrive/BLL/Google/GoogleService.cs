using Common;
using Common.Enums;
using DTOs.Responces;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Google.Apis.Drive.v3.DriveService;

namespace BLL.Google
{
    public class GoogleService
    {
        public AuthResponseDTO GetDriveService()
        {
            var response = new AuthResponseDTO();
            try
            {
                ClientSecrets clientSecrets = new ClientSecrets
                {
                    ClientId     = GlobalSettings.ClientId, 
                    ClientSecret = GlobalSettings.ClientSecret 
                };

                if(String.IsNullOrEmpty(clientSecrets.ClientId) || String.IsNullOrEmpty(clientSecrets.ClientSecret))
                {
                    response.ResultStatus = ResultStatus.AuthError;
                    response.Message      = "Please check ClientId and ClientSecret fields in google service. They cannot be empty.";
                    return response;
                }

                string path = Path.Combine(Environment.CurrentDirectory, "CurrentToken");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                
                string[] scopes = { DriveService.Scope.Drive, SheetsService.Scope.Spreadsheets };

                UserCredential authorizedUserCredential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                  clientSecrets,
                  scopes,
                  GlobalSettings.User,
                  CancellationToken.None,
                  new FileDataStore(path, true)
                ).Result;

                DriveService service = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = authorizedUserCredential,
                    ApplicationName = GlobalSettings.ProjectName, 
                });

                SheetsService serviceSheet = new SheetsService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = authorizedUserCredential,
                    ApplicationName = GlobalSettings.ProjectName,
                });

                var res = service.Files.List().Execute();
                response.DriveService  = service;
                response.SheetsService = serviceSheet;
                response.ResultStatus  = ResultStatus.Ok;
                return response;
            }
            catch(Exception ex)
            {
                response.Message       = ex.Message;
                response.ResultStatus  = ResultStatus.AuthError;
                return response;
            }
        }
    }
}
