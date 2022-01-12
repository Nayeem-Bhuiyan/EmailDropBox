using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DropBoxTest.Models
{
    public class FolderDetails
    {
        public string folderName { get; set; }
        public string folderPath { get; set; }
        public string downloadLink { get; set; }
        public int? count { get; set; }
        public IEnumerable<FileDetails> fileDetails { get; set; }
    }


    public class FileDetails
    {
        public string fileName { get; set; }
        public string filePath { get; set; }
        public string fileDownloadLink { get; set; }
    }
}
