using Common.Enums;
using System;

namespace Models
{
    public class BaseRequest
    {
        public ResultStatus Status  { get; set; }
        public string       Message { get; set; }
    }
}
