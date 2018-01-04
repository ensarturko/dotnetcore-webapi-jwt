using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreApiUndockerized.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoreApiUndockerized.Data
{
    public class ProductRepository : IProductRepository
    {
        private ProductContext _context;

        public ProductRepository(ProductContext context)
        {
            _context = context;
        }

        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<bool> SaveAllAsync()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }

        public IEnumerable<Product> GetAllProducts()
        {
            return _context.Products;
        }

        public Product GetProduct(int id)
        {
            return _context.Products.FirstOrDefault(p => p.Id.Equals(id));
        }
    }
}
