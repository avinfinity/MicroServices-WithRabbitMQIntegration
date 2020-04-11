using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductCatalogue.API.Queries
{
    public class ProductDTO
    {
        public int id { get; set; }

        public Guid productid { get; set; }

        public int storeid { get; set; }

        public string name { get; set; }

        public string description { get; set; }

        public string productCategory { get; set; }

        public int units { get; set; }

        public decimal unitprice { get; set; }

        public string pictureurl { get; set; }
    }
}