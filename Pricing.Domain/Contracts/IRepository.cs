using System;
using System.Collections.Generic;
using System.Text;

namespace Pricing.Domain
{
    public interface IRepository
    {
        IUnitOfWork UnitOfWork { get; }
    }
}
