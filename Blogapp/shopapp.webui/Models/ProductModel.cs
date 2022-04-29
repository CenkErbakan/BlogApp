using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using shopapp.entity;

namespace shopapp.webui.Models
{
    public class ProductModel
    {
        public int ProductId { get; set; }  
        // [Required(ErrorMessage ="İsim zorunlu bir alan.")]
        // [StringLength(20,MinimumLength =3,ErrorMessage ="Blog ismi 3-20 karakter aralığında olmalıdır")]
        public string Name { get; set; }  
        [Required(ErrorMessage ="Url zorunlu bir alan.")]     
        public string Url { get; set; }  
        [Required(ErrorMessage ="Açıklama zorunlu bir alan.")]   
        [StringLength(60,MinimumLength =5,ErrorMessage ="Blog açıklaması 5-60 karakter aralığında olmalıdır")]  
         
        public string Description { get; set; } 
        [Required(ErrorMessage ="ImageUrl zorunlu bir alan.")]        
        public string ImageUrl { get; set; }
        
        
        public bool IsHome { get; set; }
        public List<Category> SelectedCategories { get; set; }
    }
}