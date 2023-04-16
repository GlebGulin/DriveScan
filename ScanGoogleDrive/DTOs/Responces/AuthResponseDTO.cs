using Google.Apis.Drive.v3;
using Google.Apis.Sheets.v4;

namespace DTOs.Responces
{
    public class AuthResponseDTO : BaseResponseDTO
    {
        public DriveService DriveService { get; set; }
        public SheetsService SheetsService { get; set; }
    }
}
