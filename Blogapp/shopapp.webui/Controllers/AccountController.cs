using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using shopapp.business.Abstract;
using shopapp.webui.EmailServices;
using shopapp.webui.Extensions;
using shopapp.webui.Identity;
using shopapp.webui.Models;

namespace shopapp.webui.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class AccountController : Controller
    {
        private IMemoryCache _memoryCache;
        private UserManager<User> _userManager;
        private SignInManager<User> _signInManager;
        private IEmailSender _emailSender;
        private ICartService _cartService;
        public AccountController(UserManager<User> userManager,SignInManager<User> signInManager,IEmailSender emailSender,ICartService cartService,IMemoryCache memoryCache)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _cartService = cartService;
            _memoryCache = memoryCache;
        }
        public IActionResult Login(string ReturnUrl=null)
        {
            return View(new LoginModel()
            {
                ReturnUrl = ReturnUrl
            });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>  Login(LoginModel model)
        {
            if(!ModelState.IsValid)
            {
              return View(model);  
            }
            // Kullanıcı adı girişi için
            // var user = await _userManager.FindByNameAsync(model.UserName);
            var user = await _userManager.FindByEmailAsync(model.Email);
            

            if (user==null)
            {
                // ModelState.AddModelError("","Bu kullanıcı adı ile daha önce hesap oluşturulmamış. ");
                ModelState.AddModelError("","Bu Mail İle Daha Önce Hesap Oluşturulmamış. ");
                return View(model);
            }
            if(!await _userManager.IsEmailConfirmedAsync(user))
            {
                ModelState.AddModelError("","Lütfen Email hesabınıza gelen link ile hesabınızı onaylayın. ");
                return View(model);
            }
            var result = await _signInManager.PasswordSignInAsync(user,model.Password,true,false);

            if(result.Succeeded)
            {
                TempData.Put("message",new AlertMessage()
                {
                    Title="Hesabınızda Oturum Açıldı.",
                    Message="Oturum Açma İşlemi Başarılı.",
                    AlertType="success"
                });
                return Redirect(model.ReturnUrl??"~/");
            }
            ModelState.AddModelError("","Girilen Kullanıcı Adı Veya Parola Yanlış.");
            return View(model);
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new User()
            {
                FirstName = model .FirstName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.UserName,
                
            };

             var result = await _userManager.CreateAsync(user,model.Password);
            if(result.Succeeded)
            {
                // token oluştur

                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var url = Url.Action("ConfirmEmail","Account",new{
                    userId = user.Id,
                    token = code 
                });
                Console.WriteLine(url);
                //email
                await _emailSender.SendEmailAsync(model.Email,"Hesabınızı onaylayınız.",$"Lütfen Email hesabınız onaylamak için linke <a href='http://localhost:5000{url}'>tıklayınız</a>");

                return RedirectToAction("Login","Account");
            }
            ModelState.AddModelError("","Bilinmeyen bir hata oldu lütfen tekrar deneyiniz.");
            return View(model);
        }


        public async Task<IActionResult>Logout()
        {
            await _signInManager.SignOutAsync();
            TempData.Put("message",new AlertMessage()
                {
                    Title="Oturum Kapatıldı.",
                    Message="Hesabınızdan güvenli bir şekilde çıkış yapıldı.",
                    AlertType="warning"
                });
            return Redirect("~/");
        }

        public async Task<IActionResult> ConfirmEmail(string userId,string token)
        {
            if (userId==null || token == null)
            {
                TempData.Put("message",new AlertMessage()
                {
                    Title="Geçersiz token.",
                    Message="Geçersiz token.",
                    AlertType="danger"
                });
                // CreateMessage("Geçersiz token.","danger");
                
                return View();
            }
            var user = await _userManager.FindByIdAsync(userId);
            if(user!=null)
            {
                 var result = await _userManager.ConfirmEmailAsync(user,token);
                if(result.Succeeded)
                {
                    //cart bilgileri
                    _cartService.InitializeCart(user.Id);
                     TempData.Put("message",new AlertMessage()
                    {
                        Title="Hesabınız onaylandı.",
                        Message="Hesabınız onaylandı.",
                        AlertType="success"
                    });
                   
                    return View();
                }
                
            }
             TempData.Put("message",new AlertMessage()
                    {
                        Title="Hesabınız onaylanmadı.",
                        Message="Hesabınız onaylanmadı.",
                        AlertType="warning"
                    });
            
            return View();    
        }
        [HttpGet]
        public IActionResult forgotPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> forgotPassword(string Email)
        {
            if(string.IsNullOrEmpty(Email))
            {
                return View();
            }
            var user = await _userManager.FindByEmailAsync(Email);
            if(user == null)
            {
                return View();
            }
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            
                var url = Url.Action("ResetPassword","Account",new{
                    userId = user.Id,
                    token = code 
                });
                Console.WriteLine(url);
                //email
                await _emailSender.SendEmailAsync(Email,"Şifre Yenileme",$"Lütfen Paralanazı Yenilemek İçin Linke <a href='https://localhost:5000{url}'>Tıklayınız</a>");
            return View();
        }
        public IActionResult ResetPassword(string userId , string token)
        {
            if(userId==null || token==null)
            {
                return RedirectToAction("Home","Index");
            }
            var model = new ResetPasswordModel {Token=token};
            return View();
        }
        [HttpPost]
        public async Task<IActionResult>  ResetPassword(ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return RedirectToAction("Home","Index");
            }
            var result = await _userManager.ResetPasswordAsync(user,model.Token,model.Password);
            if(result.Succeeded)
            {
                return RedirectToAction("Login","Account");
            }
            return View(model);
        }
        public IActionResult AccessDenied()
        {
            return View();

        }
        
    }
    
}