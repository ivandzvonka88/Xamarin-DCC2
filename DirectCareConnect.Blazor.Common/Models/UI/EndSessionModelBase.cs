using DirectCareConnect.Common.Constants;
using DirectCareConnect.Common.Models.Db.Clients;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DirectCareConnect.Common.Models.UI
{
    public class EndSessionModelBase:ModelUIBase
    {
        private NoteResolution resolution;
        private string selectedDesignee;

        public event EventHandler Updated;
        public string ClientName { get; set; }
        public bool NoShow { get; set; }
        public bool Completed { get; set; }
        public string Note { get; set; }
        public string FileName { get; set; }
        public string AttachmentName { get; set; }
        public string FullAttachmentPath { get; set; }
        public int FileSize { get; set; }
        public int ProviderId { get; set; }
        public int DocId { get; set; }
        public int RemoteDocId { get; set; }
        public int CompanyId { get; set; }
        public Guid FileId { get; set; }
        public byte[] FileBuffer { get; set; }
        public List<Designee> Designees { get; set; }
        public NoteResolution Resolution {
            get
            {
                return this.resolution;
            }
            set
            {
                this.resolution = value;
                OnUpdated(new EventArgs());
            }
        }
        public string DesigneePin { get; set; }
        public string SelectedDesignee
        {
            get
            {
                return this.selectedDesignee;
            }
            set
            {
                this.selectedDesignee = value;
                OnUpdated(new EventArgs());
            }
        }
        public DateTime?  NoteDate{ get; set; }
        
        public bool HasErrors { get; set; }
        public bool IsValid { get; set; }
        public string ErrorMessage{ get; set; }
        public bool IsSaved { get; set; }

        public bool IsDesigneeLocked { get; set; }

        public double DesigneeLat { get; set; }
        public double DesigneeLon { get; set; }
        public int DesigneeLocationId { get; set; }
        public int DesigneeLocationTypeId { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        protected virtual void OnUpdated(EventArgs e)
        {
            EventHandler handler = Updated;
            handler?.Invoke(this, e);
            this.IsSaved = false;
        }
    }
}
