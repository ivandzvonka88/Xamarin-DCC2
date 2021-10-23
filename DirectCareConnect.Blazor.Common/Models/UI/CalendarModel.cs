using DirectCareConnect.Common.Impl.Communication.DDezModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace DirectCareConnect.Common.Models.UI
{
    public class CalendarModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<SchedulerEvent> Events { get; set; }
}
}
