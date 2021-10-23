using System;
using System.Collections.Generic;
using System.Text;

namespace DirectCareConnect.Common.Models.Rest
{
    public class LongTermObjective
    {
        public int objectiveId { get; set; }

        public int serviceId { get; set; }
        public int clsvId { get; set; }
        public int clsvidId { get; set; }
        public string longTermVision { get; set; }
        public string longTermGoal { get; set; }
        public int goalAreaId { get; set; }
        public string objectiveStatus { get; set; }

        public string changes { get; set; }
        public List<ShortTermGoal> shortTermGoals { get; set; }

        public Er er = new Er();
    }
}
