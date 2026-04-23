using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VendingSystemClient.Models
{
    public class MashineStats
    {
        public int TotalMachines { get; set; }
        public int WorkingMachines { get; set; }
        public int BrokenMachines { get; set; }
        public int MaintenanceMachines { get; set; }
        public double EfficiencyPercent { get; set; }
        public decimal TotalCash { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}