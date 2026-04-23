using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VendingSystemClient.Models
{
    public class PaginatedResponse<T>
    {
        public List<T> Data { get; set; } = [];
        public int Total { get; set; }
        public int Page { get; set; }
        public int Limit { get; set; }
    }
}