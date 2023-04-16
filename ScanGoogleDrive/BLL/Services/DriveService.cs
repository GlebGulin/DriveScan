using BLL.Services.Abstractions;
using System.Collections.Generic;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using BLL.Google;
using DTOs.Responces;
using Common.Enums;
using System;
using System.Linq;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Sheets.v4;
using BLL.Helper;

namespace BLL.Services
{
    public class DriveService : IDriveService
    {
        public FileListDTO GetFiles()
        {
            var fileListDTO = new FileListDTO();
            try
            {
                var googleService = new GoogleService();
                var service = googleService.GetDriveService();
                List<File> allFiles = new List<File>();

                FileList result = null;
                while (true)
                {
                    if (result != null && string.IsNullOrWhiteSpace(result.NextPageToken))
                        break;

                    FilesResource.ListRequest listRequest = service.DriveService.Files.List();
                    listRequest.PageSize = 1000;
                    listRequest.Fields = "nextPageToken, files(id, name, mimeType)";
                    if (result != null)
                        listRequest.PageToken = result.NextPageToken;

                    result = listRequest.Execute();
                    allFiles.AddRange(result.Files);
                }

                if (allFiles.Count == 0)
                {
                    fileListDTO.ResultStatus = ResultStatus.NotFound;
                    fileListDTO.Message = "No files were found on your google drive. Please add files to your google drive space.";
                }

                foreach (var file in allFiles)
                    fileListDTO.Files.Add(new FileDiskDTO() { Name = file.Name, MimeType = file.MimeType, Id = file.Id });

                return fileListDTO;
            }

            catch(Exception ex)
            {
                fileListDTO.Message      = ex.Message;
                fileListDTO.ResultStatus = ResultStatus.InvalidOperation;
                return fileListDTO;
            }
        }

        public CompareResultListsDTO CompareLists(List<FileDiskDTO> currentList, List<FileDiskDTO> googleList)
        {
            var resultCompare = new CompareResultListsDTO();
            var addedFiles   = googleList.Except(currentList, new ListFileComparer());
            var removedFiles = currentList.Except(googleList, new ListFileComparer());

            resultCompare.AddedFiles   = addedFiles.ToList();
            resultCompare.RemovedFiles = removedFiles.ToList();

            resultCompare.Result = (addedFiles.Count() == 0 && removedFiles.Count() == 0) ? true : false;
            return resultCompare;
        }

        public WriteSheetDTO WriteUpdatedList(List<FileDiskDTO> updatedList, string? SpreedSheetId, string? SheetTitle)
        {
            WriteSheetDTO result = new WriteSheetDTO();

            List<IList<object>> data = new List<IList<object>>();

            try
            {

                var googleService = new GoogleService();
                var service = googleService.GetDriveService();
                if(service.ResultStatus == ResultStatus.Ok)
                {
                    //Check if there is a tabular document with the given id
                    if (String.IsNullOrEmpty(SpreedSheetId))
                    {
                        var myNewSheet = new Spreadsheet();
                        myNewSheet.Properties = new SpreadsheetProperties();
                        myNewSheet.Properties.Title = String.Format("Check List File {0}", DateTime.Now.ToString());

                        var newSheetPrapare = service.SheetsService.Spreadsheets.Create(myNewSheet);

                        var awsomNewSheet = newSheetPrapare.Execute();
                        result.SheetTitle = awsomNewSheet.Sheets[0].Properties.Title;

                        result.SpreedSheetId = awsomNewSheet.SpreadsheetId;
                    }

                    else
                    {
                        result.SheetTitle = SheetTitle;
                        result.SpreedSheetId = SpreedSheetId;
                    }

                    
                    String range = String.Format("{0}!A1:Y", result.SheetTitle);
                    service.SheetsService.Spreadsheets.Values.Clear(new ClearValuesRequest(), result.SpreedSheetId, range).Execute();
                    string valueInputOption = "USER_ENTERED";

                    var header = new List<object> { "File Name", "Mime Type", "Id" };
                    data.Add(header);

                    foreach(var file in updatedList)
                    {
                        var contentRow = new List<object> { file.Name, file.MimeType, file.Id };
                        data.Add(contentRow);
                    }

                    // The new values to apply to the spreadsheet.
                    List<ValueRange> updateData = new List<ValueRange>();
                    var dataValueRange = new ValueRange();
                    dataValueRange.Range = range;
                    dataValueRange.Values = data;
                    updateData.Add(dataValueRange);

                    BatchUpdateValuesRequest requestBody = new BatchUpdateValuesRequest();
                    requestBody.ValueInputOption = valueInputOption;
                    requestBody.Data = updateData;

                    var request = service.SheetsService.Spreadsheets.Values.BatchUpdate(requestBody, result.SpreedSheetId);

                    BatchUpdateValuesResponse response = request.Execute();
                }
                
                return result;
            }
            catch(Exception ex)
            {
                result.Message = ex.Message;
                return result;
            }
        }

        public void TrackAddedFiles(List<FileDiskDTO> addedFiles)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Added Files:");
            foreach (var file in addedFiles)
            {
                Console.WriteLine("\t{0}", file.Name);
            }
            Console.ResetColor();
            return;
        }

        public void TrackDeletedFiles(List<FileDiskDTO> deletedFiles)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Added Files:");
            foreach (var file in deletedFiles)
            {
                Console.WriteLine("\t{0}", file.Name);
            }
            Console.ResetColor();
            return;
        }
    }
}
