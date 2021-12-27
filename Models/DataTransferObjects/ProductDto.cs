using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProductManagementApi.Models.DataTransferObjects
{
    public class ProductDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public float Price { get; set; }
    }


    public class ProductEditDto
    {
        public string Name { get; set; }
        public float Price { get; set; }
    }

    public class ProductPricesDto
    {
        public float TotalPrice { get; set; }
        public IEnumerable<Product> Items { get; set; }
    }
}
