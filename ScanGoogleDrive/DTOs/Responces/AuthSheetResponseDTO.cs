using Google.Apis.Sheets.v4;

namespace DTOs.Responces
{
    public class AuthSheetResponseDTO : BaseResponseDTO
    {
        public SheetsService SheetsService { get; set; }
    }
}
