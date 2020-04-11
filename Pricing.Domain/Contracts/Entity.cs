using System;
using System.Collections.Generic;
using System.Text;

namespace Pricing.Domain
{
    public class Entity
    {
        private int? _requestedHashCode;
        private int _Id;

        public override int GetHashCode()
        {
            if (!_requestedHashCode.HasValue)
                _requestedHashCode = this.Id.GetHashCode() ^ 31;

            return _requestedHashCode.Value;
        }

        public virtual int Id
        {
            get
            {
                return _Id;
            }
            protected set
            {
                _Id = value;
            }
        }
    }
}
