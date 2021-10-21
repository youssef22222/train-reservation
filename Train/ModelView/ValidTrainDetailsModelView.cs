using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Train.ModelView
{
    public class ValidTrainDetailsModelView
    {
        public double Price { get; set; }
        public DateTime Date { get; set; }
        public Train.Models.Train train { get; set; }
        public int TrainID { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Type { get; set; }
    }
}