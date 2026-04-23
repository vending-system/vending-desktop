using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VendingSystemClient.Models
{
    public class SalesSummary
    {
        public decimal TodaySales { get; set; }
        public decimal YesterdaySales { get; set; }
        public decimal TotalSales { get; set; }
        public double AverageCheck { get; set; }
        public int ServiceToday { get; set; }
        public int ServiceYesterday { get; set; }
    }
}