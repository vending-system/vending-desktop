using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VendingSystemClient.Models
{
    public class Machine
    {
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string SerialNumber { get; set; } = "";
    public string InventoryNumber { get; set; } = "";
    public string ModelName { get; set; } = "";
    public string Manufacturer { get; set; } = "";
    public string Country { get; set; } = "";
    public MachineStatus? Status { get; set; }
    public Company? Company { get; set; }
    public string LocationAddress { get; set; } = "";
    public string? CommissioningDate { get; set; }
    public string? ManufactureDate { get; set; }
    public string? ModemId { get; set; }
    public string? LastCalibrationDate { get; set; }
    public int? CalibrationIntervalMonths { get; set; }
    public string? NextCalibrationDate { get; set; }
    public int? ResourceHoursTotal { get; set; }
    public int? ServiceTimeHours { get; set; }
    }
    public class MachineStatus { public int Id { get; set; } public string Name { get; set; } = ""; }
    public class Company { public int Id { get; set; } public string Name { get; set; } = ""; public string? Inn { get; set; } public string? Address { get; set; } }

}