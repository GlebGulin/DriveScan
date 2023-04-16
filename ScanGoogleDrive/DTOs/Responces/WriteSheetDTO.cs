using System;
using System.Collections.Generic;
using System.Text;

namespace DTOs.Responces
{
    public class WriteSheetDTO : BaseResponseDTO
    {
        public bool   Result        { get; set; }
        public string SpreedSheetId { get; set; }
        public string SheetTitle    { get; set; }
    }
}
