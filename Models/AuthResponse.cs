using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VendingSystemClient.Models
{
    public class AuthResponse
    {
        public string Token { get; set; } = "";
        public string Username { get; set; } = "";
    }

    public class LoginRequest
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
    }
}