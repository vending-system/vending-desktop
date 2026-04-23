using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VendingSystemClient.Models
{
    public class CreateMachineDto
    {
    public string Name { get; set; } = "";
    public string SerialNumber { get; set; } = "";
    public string InventoryNumber { get; set; } = "";
    public string ModelName { get; set; } = "";
    public string Manufacturer { get; set; } = "";
    public string Country { get; set; } = "";
    public int StatusId { get; set; }
    public int CompanyId { get; set; }
    public string LocationAddress { get; set; } = "";
    public DateTime CommissioningDate { get; set; }
    public DateTime ManufactureDate { get; set; }
    public string? ModemId { get; set; }
    public DateTime? LastCalibrationDate { get; set; }
    public int? CalibrationIntervalMonths { get; set; }
    public int? ResourceHoursTotal { get; set; }
    public int? ServiceTimeHours { get; set; }
    public decimal? CurrentCash { get; set; }
    public decimal? TotalRevenue { get; set; }
    }
}