using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using shopapp.entity;

namespace shopapp.data.Concrete.EfCore
{
    public static class SeedDatabase
    {
        public static void Seed()
        {
            var context = new ShopContext();

            if (context.Database.GetPendingMigrations().Count()==0)
            {
              if (context.Categories.Count()==0)
              {
                  context.Categories.AddRange(Categories);
              } 
              
              if (context.Products.Count()==0)
              {
                  context.Products.AddRange(Products);
                  context.AddRange(ProductCategories);
              }   
            }
            context.SaveChanges();
            
        }
        private static Category[] Categories={
            new Category(){Name="Teklonoji",Url="teklonoji"},
            new Category(){Name="Yemek",Url="yemek"},
            new Category(){Name="Kişisel Bakım",Url="kisisel-bakım"},
            new Category(){Name="Kitap",Url="kitap"}

        };

        private static Product[] Products={
            new Product(){Name="Telefon",ImageUrl="1.jpg",Description="Telefon Makalesi",ProductId=1,Url="telefon"},
            new Product(){Name="Bilgisayar",ImageUrl="2.jpg",Description="Bilgisayar Makalesi",ProductId=2,Url="bilgisayar"},
            new Product(){Name="Robotik",ImageUrl="3.jpg",Description="Robot Makalesi",ProductId=3,Url="robotik"},
            new Product(){Name="Blockchain",ImageUrl="4.jpg",Description="Blockchain Makalesi",ProductId=4,Url="blockchain"}
        };

        private static ProductCategory[] ProductCategories=
        {
            new ProductCategory(){Product=Products[0],Category=Categories[0]},
            new ProductCategory(){Product=Products[0],Category=Categories[3]},
            new ProductCategory(){Product=Products[1],Category=Categories[0]},
            new ProductCategory(){Product=Products[2],Category=Categories[0]},
            new ProductCategory(){Product=Products[3],Category=Categories[0]},
        };

    }
}