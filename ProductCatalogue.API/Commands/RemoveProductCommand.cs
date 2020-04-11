using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace ProductCatalogue.API.Commands
{
    [DataContract]
    public class RemoveProductCommand : IRequest<bool>
    {
        [DataMember]
        public int Id { get; private set; }

        public RemoveProductCommand(int productid)
        {
            Id = productid;
        }
    }
}