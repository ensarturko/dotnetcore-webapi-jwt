using System;
using System.Collections.Generic;
using CoreApiUndockerized.Data.Entities;

namespace CoreApiUndockerized.Data.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public double Price  { get; set; }
    }
}
