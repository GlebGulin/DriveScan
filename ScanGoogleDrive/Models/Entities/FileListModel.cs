using System.Collections.Generic;

namespace Models.Entities
{
    public class FileListModel
    {
        public List<FileDisk> Files { get; set; }
        public FileListModel()
        {
            Files = new List<FileDisk>();
        }
    }

    public class FileDisk
    {
        public string Name     { get; set; }
        public string MimeType { get; set; }
        public string Id       { get; set; }
    }
}
