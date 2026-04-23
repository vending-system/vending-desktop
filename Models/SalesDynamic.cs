using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VendingSystemClient.Models
{
    public class SalesDynamic
    {
        public DateOnly Date { get; set; }
        public string DayName { get; set; } = "";
        public decimal TotalAmount { get; set; }
        public int TotalQuantity { get; set; }
        public int TransactionsCount { get; set; }
        public double BarHeight { get; set; }
        public string DateStr { get; set; } = "";
    }
}