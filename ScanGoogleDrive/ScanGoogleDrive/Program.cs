﻿using BLL.Google;
using BLL.Services;
using BLL.Services.Abstractions;
using Common;
using Common.Enums;
using DTOs.Responces;
using System;

namespace ScanGoogleDrive
{
    class Program
    {
        private static FileListDTO currentList = new FileListDTO();
        private static string    SpreedSheetId = String.Empty;
        private static string       SheetTitle = String.Empty;
        private static bool      FirstChecking = true;

        static void Main(string[] args)
        {
            Console.WriteLine("Starting...");
            if(String.IsNullOrEmpty(GlobalSettings.ClientId) 
                || String.IsNullOrEmpty(GlobalSettings.ClientSecret) 
                || String.IsNullOrEmpty(GlobalSettings.ProjectName)
                || String.IsNullOrEmpty(GlobalSettings.User))
            {
                Console.WriteLine("For the application to work correctly, you must first configure the GlobalSettings.cs file");
                Console.ReadKey();
            }

            //Authentication
            GoogleService googleService = new GoogleService();
            var authResponse = googleService.GetDriveService();
            if (authResponse.ResultStatus != ResultStatus.Ok)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(String.Format("Google services authentication error. Cause: {0}" , authResponse.Message));
                Console.ResetColor();
                Console.WriteLine("Please check your Google account settings. Set the correct values ClientId and ClientSecret. Press any key");
                Console.ReadKey();
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(String.Format("Google services were successfully authorised. Current time {0}. The next console and google sheets file updates are expected at {1}", DateTime.Now.ToString(), DateTime.Now.AddMinutes(GlobalSettings.IntervalMins).ToString()));
            Console.ResetColor();
            Console.WriteLine("Starts the process of retrieving all google drive files of a given account...");

            //Getting a list of all files
            var startTimeSpan = TimeSpan.Zero;
            var periodTimeSpan = TimeSpan.FromMinutes(GlobalSettings.IntervalMins);

            var timer = new System.Threading.Timer((e) =>
            {
                IDriveService service = new DriveService();
                var res = service.GetFiles();
                if (res.ResultStatus != ResultStatus.Ok)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(String.Format("Failed to get a list of files. Cause: {0}", res.Message));
                    Console.ResetColor();
                }
                else
                {

                    Console.WriteLine(String.Format("The following files count {0} were found:", res.Files.Count));
                    foreach (var file in res.Files)
                    {
                        Console.WriteLine(String.Format("File Name: {0}  File Type: {1}   File Id: {2}", file.Name, file.MimeType, file.Id));
                    }
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(String.Format("{0} files in total found. Next search will take {1} minutes, at {2}", res.Files.Count, GlobalSettings.IntervalMins, DateTime.Now.AddMinutes(GlobalSettings.IntervalMins).ToString()));
                    Console.ResetColor();

                    var compareResult = service.CompareLists(currentList.Files, res.Files);
                    
                    if (!compareResult.Result)
                    {
                        if (!FirstChecking)
                        {
                            Console.ForegroundColor = ConsoleColor.DarkBlue;
                            Console.WriteLine("Changes were found on Google Drive. Changes in the google registered files file will be made accordingly");
                            Console.ResetColor();
                            if (compareResult.RemovedFiles.Count > 0)
                                service.TrackDeletedFiles(compareResult.RemovedFiles);
                            if (compareResult.AddedFiles.Count > 0)
                                service.TrackAddedFiles(compareResult.AddedFiles);
                        }

                        FirstChecking = false;
                        var writeSheets = service.WriteUpdatedList(res.Files, SpreedSheetId, SheetTitle);
                        SpreedSheetId = writeSheets.SpreedSheetId;
                        SheetTitle = writeSheets.SheetTitle;
                        currentList = res;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("There have been no changes to the google drive. The software will not make any changes to the file list of all registered files on Google Drive");
                        Console.ResetColor();
                    }
                }
            }, null, startTimeSpan, periodTimeSpan);
            
            Console.ReadKey();
        }
    }
}
