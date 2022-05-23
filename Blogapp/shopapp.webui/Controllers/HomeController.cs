using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using shopapp.business.Abstract;
using shopapp.data.Abstract;
using shopapp.webui.Models;
using shopapp.webui.Services;
using StackExchange.Redis;

namespace shopapp.webui.Controllers
{
    
    
    public class HomeController:Controller
    {
        private readonly RedisService _redisService;
        private readonly IDatabase db;
        private string listkey= "İletişim Ziyaretçisi";

         
        private IProductService _productService;

         public HomeController(IProductService productService ,RedisService redisService )
         {
             this._productService = productService;
             _redisService=redisService;
             db=_redisService.GetDb(0);
             
         }
        public IActionResult Index()
        {
            db.StringSet(listkey,0);
            var productViewModel = new ProductListViewModel()
            {
                Products = _productService.GetHomePageProducts()
            };

            return View(productViewModel);
        }

         // localhost:5000/home/about
        public IActionResult About()
        {
            return View();
        }

         public IActionResult Contact(string name)
        {
             var value = db.StringGet("name");
            // var count = db.StringDecrementAsync("ziyaretci",1).Result;
            db.StringIncrementAsync(listkey,1).Wait();
            
            return View("MyView");
        }
    }
}