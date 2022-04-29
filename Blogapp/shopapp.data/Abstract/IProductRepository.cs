using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using shopapp.entity;

namespace shopapp.data.Abstract
{
    public interface IProductRepository : IRepository<Product>
    {
        Product GetByIdWithCategories(int id);
       Product GetProductDetails(string Url);
       List<Product> GetProductByCategory(string name, int page, int pageSize);
       List<Product> GetSearchResult(string SearchString);

       List<Product> GetHomePageProducts();
       int GetCountByCategory(string category);
       void Update(Product entity, int[] categoryIds);

       

       
    }
} 