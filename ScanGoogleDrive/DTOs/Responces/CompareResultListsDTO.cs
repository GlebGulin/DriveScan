using System.Collections.Generic;

namespace DTOs.Responces
{
    public class CompareResultListsDTO : BaseResponseDTO
    {
        public bool              Result       { get; set; }
        public List<FileDiskDTO> RemovedFiles { get; set; }
        public List<FileDiskDTO> AddedFiles   { get; set; }
        public CompareResultListsDTO()
        {
            RemovedFiles = new List<FileDiskDTO>();
            AddedFiles   = new List<FileDiskDTO>();
        }
    }
}
