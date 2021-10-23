using System;
using System.Collections.Generic;
using System.Text;

namespace DirectCareConnect.Common.Impl.Communication.DDezModels
{
    public class SetNoteResponse
    {
        public Er Er { get; set; }
        public List<PendingDocumentation> PendingDocumentations { get; set; }
    }
}
