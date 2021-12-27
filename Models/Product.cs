using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductManagementApi.Models
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public float Price { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool Disabled { get; set; }
    }
}
