using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VendingSystemClient.Models.DTO
{
    public class CompanyDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? Inn { get; set; }
        public string? Address { get; set; }
    }
}