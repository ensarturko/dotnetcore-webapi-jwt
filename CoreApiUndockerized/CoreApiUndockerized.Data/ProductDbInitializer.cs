using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreApiUndockerized.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoreApiUndockerized.Data
{
  public class ProductDbInitializer
  {
    private ProductContext _ctx;

    public ProductDbInitializer(ProductContext ctx)
    {
      _ctx = ctx;
    }
            
    public async Task Seed()
    {
      if (!_ctx.Products.Any())
      {
        // Add Data
        _ctx.AddRange(_sample);
        await _ctx.SaveChangesAsync();
      }
    }

    List<Product> _sample = new List<Product>
    {
      new Product()
      {
        Title = "Product1",
        CreatedDate = DateTime.Now,
        ModifiedDate = DateTime.Now,
        Price = 0.32
      }
    };

  }
}
