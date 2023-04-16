using System.Collections.Generic;

namespace DTOs.Responces
{
    public class FileListDTO : BaseResponseDTO
    {
        public List<FileDiskDTO> Files { get; set; }
        public FileListDTO()
        {
            Files = new List<FileDiskDTO>();
        }
    }

    public class FileDiskDTO
    {
        public string Name { get; set; }
        public string MimeType { get; set; }
        public string Id { get; set; }
    }
}
