using System;

namespace ProductCatalogue.Domain
{
    public class Product : Entity , IAggregateRoot
    {
        public Product() { }

        public Product(int id, int storeId, string name,
            string description, string productCategory, int units,
            decimal unitPrice, string picturePath)
        {
            Id = id;
            ProductId = Guid.NewGuid();
            StoreId = storeId;
            Name = name;
            Description = description;
            ProductCategory = productCategory;
            Units = units;
            UnitPrice = unitPrice;
            PicturePath = picturePath;
        }

        public Guid ProductId { get; private set; }
        public int StoreId { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string ProductCategory { get; private set; }
        public int Units { get; private set; }
        public decimal UnitPrice { get; private set; }
        public string PicturePath { get; private set; }

        //Sample domain method
        public void SetOutOfStock()
        {
            this.Units = 0;
            //Raise domain events for updating others
            //Use MediaTr to handle domain events and perform other jobs
            //Domain events may be also mapped as Notifications
        }
    }
}