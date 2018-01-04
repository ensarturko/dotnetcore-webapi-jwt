using System.Collections.Generic;
using System.Threading.Tasks;
using CoreApiUndockerized.Data.Entities;

namespace CoreApiUndockerized.Data
{
  public interface IProductRepository
  {
    // Basic DB Operations
    void Add<T>(T entity) where T : class;
    void Delete<T>(T entity) where T : class;
    Task<bool> SaveAllAsync();

    // Products
    IEnumerable<Product> GetAllProducts();
    Product GetProduct(int id);
  }
}