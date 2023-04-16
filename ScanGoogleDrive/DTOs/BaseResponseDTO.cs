using Common.Enums;

namespace DTOs
{
    public class BaseResponseDTO
    {
        public ResultStatus ResultStatus { get; set; }
        public string Message { get; set; }
    }
}
