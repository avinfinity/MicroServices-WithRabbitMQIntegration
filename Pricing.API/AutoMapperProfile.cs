using AutoMapper;
using Pricing.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pricing.API
{
    internal sealed class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ProductPrice, ProductPriceDTO>();
            CreateMap<ProductPriceDTO, ProductPrice>();
        }
    }
}
