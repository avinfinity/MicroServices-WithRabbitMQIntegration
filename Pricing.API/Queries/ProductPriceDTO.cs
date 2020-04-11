using System.Runtime.Serialization;

namespace Pricing.API
{
    [DataContract]
    public class ProductPriceDTO
    {
        [DataMember]
        public decimal Price { get; set; }
    }
}