using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace ProductCatalogue.API.Commands
{
    [DataContract]
    public class AddNewProductCommand : IRequest<bool>
    {
        [DataMember]
        public int StoreId { get; private set; }

        [DataMember]
        public Guid ProductId { get; set; }

        [DataMember]
        public string Name { get; private set; }

        [DataMember]
        public string Description { get; private set; }

        [DataMember]
        public string ProductCategory { get; private set; }

        [DataMember]
        public int Units { get; private set; }

        [DataMember]
        public decimal UnitPrice { get; private set; }

        [DataMember]
        public string PictureUrl { get; private set; }

        public AddNewProductCommand(int storeid, string name, 
            string description, string category, int units, 
            decimal unitprice, string picUrl )
        {
            ProductId = Guid.NewGuid();
            StoreId = storeid;
            Name = name;
            Description = description;
            ProductCategory = category;
            Units = units;
            UnitPrice = unitprice;
            PictureUrl = picUrl;
        }
    }
}