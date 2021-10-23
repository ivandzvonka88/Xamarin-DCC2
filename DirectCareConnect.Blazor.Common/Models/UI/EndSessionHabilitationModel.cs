using DirectCareConnect.Common.Models.Rest;
using System;
using System.Collections.Generic;
using System.Text;

namespace DirectCareConnect.Common.Models.UI
{
    public class EndSessionHabilitationModel: EndSessionModelBase
    {
        public List<LongTermObjective> LongTermObjectives { get; set; }
        public List<Scoring> Scoring { get; set; }

    }
}
