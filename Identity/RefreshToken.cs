using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductManagementApi.Identity
{
    public class RefreshToken
    {
        public Guid Id { get; set; }
        public bool IsSoftDelete { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Token { get; set; }
        public string JwtId { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsUsed { get; set; }
        public bool Invalidated { get; set; }
        public string UserId { get; set; }
    }
}
