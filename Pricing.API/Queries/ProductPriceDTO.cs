using System;
using System.Runtime.Serialization;

namespace Pricing.API
{
    [DataContract]
    public class ProductPriceDTO
    {
        [DataMember]
        public decimal Price { get; set; }

        [DataMember]
        public Guid ProductId { get; set; }
    }
}