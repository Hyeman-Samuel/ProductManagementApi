using ProductManagementApi.Models;
using ProductManagementApi.Models.DataTransferObjects;
using ProductManagementApi.Persistence;
using ProductManagementApi.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ProductManagementApi.Repository.Implementation
{
    public class ProductRepository : Repository<Product>,IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context):base(context)
        {
            _context = context;
        }

        public override IEnumerable<Product> GetAll(Expression<Func<Product, bool>> predicate)
        {
            var products = base.GetAll(predicate);
            return products.OrderByDescending(x => x.CreatedAt);
        }

        public override void Create(Product entity)
        {
            base.Create(entity);
        }

        public override IEnumerable<Product> GetAll()
        {
            var products = base.GetAll();
            return products.Where(x=>x.Disabled == false);
        }

        public void Disable(Product product)
        {
            product.Disabled = true;
            _context.Products.Update(product);
            _context.SaveChanges();
        }

        public ProductPricesDto CalculateAllProductPrices(DateTime date)
        {
            var products = _context.Products.Where(x => x.CreatedAt > date).Where(x=>x.Disabled == false);
            float totalPrice = 0;
            foreach (var product in products)
            {
                totalPrice += product.Price;
            }

            var pricesDto = new ProductPricesDto
            {
                TotalPrice = totalPrice,
                Items = products
            };

            return pricesDto;

        }
    }
}
