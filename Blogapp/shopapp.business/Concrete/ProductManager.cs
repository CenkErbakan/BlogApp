using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using shopapp.business.Abstract;
using shopapp.data.Abstract;
using shopapp.data.Concrete.EfCore;
using shopapp.entity;

namespace shopapp.business.Concrete
{
    public class ProductManager : IProductService
    {
        private IProductRepository _productRepository;

        public ProductManager(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

       

        public bool Create(Product entity)
        {
            if(Validation(entity))
            {
                _productRepository.Create(entity);
                return true;
            }
            return false;
            

        }

        public void Delete(Product entity)
        {
            _productRepository.Delete(entity);
        }

        public List<Product> GetAll()
        {
            return _productRepository.GetAll();
        }

        public Product GetById(int id)
        {
            return _productRepository.GetById(id);
        }

        public Product GetByIdWithCategories(int id)
        {
            return _productRepository.GetByIdWithCategories(id);
        }

        public int GetCountByCategory(string category)
        {
            return _productRepository.GetCountByCategory(category);
        }

        public List<Product> GetHomePageProducts()
        {
            return _productRepository.GetHomePageProducts();
        }

        public Product GetProductDetails(string Url)
        {
            return _productRepository.GetProductDetails(Url);
            
        }

        public List<Product> GetProductsByCategory(string name,int page, int pageSize)
        {
            return _productRepository.GetProductByCategory(name,page,pageSize);

        }

        public List<Product> GetSearchResult(string SearchString)
        {
            return _productRepository.GetSearchResult(SearchString);
        }

        public void Update(Product entity)
        {
            _productRepository.Update(entity);
        }

        public bool Update(Product entity, int[] categoryIds)
        {
            if(Validation(entity))
            {
                if(categoryIds.Length==0)
                {
                    ErrorMesage += "Blog için en az bir katagori seçmelisiniz.";
                    return false;
                }
                _productRepository.Update(entity,categoryIds);
                return true;
            }
            return false;
            
        } 
        public string ErrorMesage { get; set; }

        public bool Validation(Product entity)
        {
            var isValid = true;
            if(string.IsNullOrEmpty(entity.Name))
            {
                ErrorMesage += "blog ismi girmelisiniz. \n";
                isValid=false;
            }
            
            return isValid;
        }
        
    }
}