using System;
using System.Collections.Generic;
using System.Text;

namespace DirectCareConnect.Common.Models.ComponentCommunication
{
    public class FilePickerResult
    {
        public Guid FileId { get; set; }
        public string PathToUpload { get; set; }
        public string OriginalFileName { get; set; }
        public int FileLength { get; set; }
    }
}
