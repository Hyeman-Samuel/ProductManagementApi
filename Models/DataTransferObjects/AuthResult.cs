using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductManagementApi.Models.DataTransferObjects
{
    public class AuthResult
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string ExpiryTimeinMinutes { get; set; }
        public bool HasError { get; set; }
        public string Message { get; set; }
    }
}
