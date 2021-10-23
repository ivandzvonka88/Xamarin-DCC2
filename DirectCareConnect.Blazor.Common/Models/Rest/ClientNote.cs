using DirectCareConnect.Common.Models.Db.Clients;
using System;
using System.Collections.Generic;
using System.Text;

namespace DirectCareConnect.Common.Models.Rest
{

    public class ClientNote
    {
        public string signee { get; set; }

        public string signeeCredentials { get; set; }

        public bool verification { get; set; }

        public string clientName { get; set; }

        public int providerId { get; set; }
        public string docType { get; set; }

        public string svc { get; set; }

        public int docId { get; set; }
        public int remoteDocId { get; set; }
        public int clientId { get; set; }

        public string note { get; set; }

        public bool hasAttachment { get; set; }

        public string attachmentName { get; set; }

        public string extension { get; set; }
        public string coId { get; set; }
        public string dt { get; set; }
        public byte[] fileContents { get; set; }

        public List<LongTermObjective> longTermObjectives { get; set; }
        public List<Scoring> scoring { get; set; }
        public List<CareArea> careAreas { get; set; }
        public List<Designee> designees { get; set; }

        #region Reasons Not To Sign
        public bool noShow { get; set; }
        public bool designeeUnableToSign { get; set; }
        public bool designeeRefusedToSign { get; set; }
        public bool clientRefusedService { get; set; }
        public bool unsafeToWork { get; set; }
        #endregion

        #region designee signer info
        public int designeeId { get; set; }
        public int guardianId { get; set; }
        public double designeeLat { get; set; }
        public double designeeLon { get; set; }
        public int designeeLocationId { get; set; }
        public int designeeLocationTypeId { get; set; }
        #endregion

        public bool supervisorPresent { get; set; }
        public bool completed { get; set; }

        public bool designeedLocked { get; set; }

        public Er er = new Er();
    }
}
