using DTOs.Responces;
using System.Collections.Generic;

namespace BLL.Services.Abstractions
{
    public interface IDriveService
    {
        FileListDTO GetFiles();
        CompareResultListsDTO CompareLists(List<FileDiskDTO> currentList, List<FileDiskDTO> googleList);
        WriteSheetDTO WriteUpdatedList(List<FileDiskDTO> updatedList, string? SpreedSheetId, string? SheetTitle);
        void TrackAddedFiles(List<FileDiskDTO> addedFiles);
        void TrackDeletedFiles(List<FileDiskDTO> deletedFiles);
    }
}
