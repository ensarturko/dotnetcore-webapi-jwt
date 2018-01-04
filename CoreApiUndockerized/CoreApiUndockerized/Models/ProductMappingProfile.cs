using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CoreApiUndockerized.Data.Entities;

namespace CoreApiUndockerized.Models
{
    // To map entity model and view model.
    public class ProductMappingProfile : Profile    
    {
        public ProductMappingProfile()
        {
            CreateMap<Product, ProductModel>();
        }
    }
}
