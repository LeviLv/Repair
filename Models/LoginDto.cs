using System;
using Microsoft.AspNetCore.Http;

namespace Repair.Models
{
    public class LoginDto
    {
#if !DEBUG
        public int? CurrentId { get; set; }
#endif
#if DEBUG
        public int? CurrentId { get; set; } = 9;
#endif
        public string CurrentRole { get; set; }

        
    }
}