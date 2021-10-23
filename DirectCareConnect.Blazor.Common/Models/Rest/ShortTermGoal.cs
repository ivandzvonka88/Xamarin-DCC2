using System;
using System.Collections.Generic;
using System.Text;

namespace DirectCareConnect.Common.Models.Rest
{
    public class ShortTermGoal
    {
        private string _score;
        private int? _progress;
        public event EventHandler ScoreChanged;
        public event EventHandler ProgressChanged;
        public int goalId { get; set; }
        public int step { get; set; }
        public string shortTermGoal { get; set; }
        public string teachingMethod { get; set; }
        public string goalStatus { get; set; }
        public string frequencyId { get; set; }
        public string frequency { get; set; }
        public string progress { get; set; }
        public string score { 
            get
            {
                return this._score;
            }
            set
            {
                this._score = value;
                ScoreChanged?.Invoke(this, new EventArgs());
            }
    }

        public int? trialPct
        {
            get
            {
                return this._progress;
            }
            set
            {
                this._progress = value;
                ProgressChanged?.Invoke(this, new EventArgs());
            }
        }
        public List<TherapyScore> therapyScores { get; set; }
        public string recommendation { get; set; }
        public Er er = new Er();
    }
}
