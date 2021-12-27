using ProductManagementApi.Models;
using ProductManagementApi.Models.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductManagementApi.Repository.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        void Disable(Product product);
        ProductPricesDto CalculateAllProductPrices(DateTime date);
    }
}
