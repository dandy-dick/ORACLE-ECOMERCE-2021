using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using ORACLE_ECOMERCE_2021.Models;
using System.ComponentModel.DataAnnotations.Schema;
using Repository.DomainModels;

namespace ORACLE_ECOMERCE_2021.Controllers
{

    public class CustomerController : ApplicationController
    {
        private UserManager<Customer> UserManager { get; set; }
        private SignInManager<Customer> SignInManager { get; set; }

        public CustomerController(
            UserManager<Customer> userManager, SignInManager<Customer> signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        [HttpGet]
        [Route("/register")]
        public IActionResult Authy_Register()
        {
            this.ViewBag.ModelStateErrorDictionary = this.GetModelStateDictionary<RegisterModel>();
            return View();
        }


        [HttpPost]
        [Route("/register")]
        public async Task<IActionResult> Authy_CreateUser(RegisterModel model)
        {
            var modelErrors = this.GetModelStateDictionary<RegisterModel>();
            if (ModelState.IsValid)
            {
                // kiem tra su ton tai cua tai khoan
                var user = new Customer() { UserName = model.UserName };
                var result = await UserManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    string sessionStr = JsonConvert.SerializeObject(model);
                    HttpContext.Session.SetString(_2faRegisterModelSessionName, sessionStr);

                    return RedirectToAction("Index", "PhoneVerification", new { model.PhoneNumber });
                }
                else if (result.Errors.FirstOrDefault(p => p.Code == "DuplicateUserName") != null)
                {
                    modelErrors.Add("Summary", new ModelStateError()
                    {
                        ErrorMessages = $"  Tên tài khoản '{ model.UserName }' đã tồn tại, xin hãy thử lai"
                    });

                    modelErrors["UserName"] = new ModelStateError()
                    {
                        ErrorMessages = "Tên tài khoản này đã tồn tại, xin hãy thử dùng 1 tài khoản khác"
                    };
                }
            }

            this.ViewBag.RegisterModel = model;
            this.ViewBag.ModelStateErrorDictionary = modelErrors;
            return View("Authy_Register");
        }

        public async Task<IActionResult> Authy_PhoneVerified()
        {
            string sessionStr = HttpContext.Session.GetString(_2faRegisterModelSessionName);
            var model = JsonConvert.DeserializeObject<RegisterModel>(sessionStr);

            await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, false);

            Customer user = UserManager.Users.FirstOrDefault(p => p.UserName == model.UserName);
            user.PhoneNumber = model.PhoneNumber;
            await UserManager.UpdateAsync(user);

            HttpContext.Session.Remove(_2faRegisterModelSessionName);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Route("/login")]
        public IActionResult Login()
        {
            this.ViewBag.ModelStateErrorDictionary = this.GetModelStateDictionary<LoginModel>();
            return View();
        }

        [HttpPost]
        [Route("/login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            var modelErrors = this.GetModelStateDictionary<LoginModel>();
            if (ModelState.IsValid)
            {
                // log user in
                var result = await SignInManager
                    .PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    // start 2fa authentication
                    var user = UserManager.Users.FirstOrDefault(p => p.UserName == model.UserName);
                    string sessionStr = JsonConvert.SerializeObject(user);
                    HttpContext.Session.SetString(_2faRegisterModelSessionName, sessionStr);

                    return RedirectToAction("Index", "PhoneVerification", new { user.PhoneNumber });
                }
                else
                {
                    string errorMessage = "Đăng nhập thất bại!! Tên đăng nhập và mật khẩu không khớp!!";
                    if (UserManager.Users.FirstOrDefault(p => p.UserName == model.UserName) == null)
                    {
                        errorMessage = "Đăng nhập thất bại!!" + " Tài khoản " + model.UserName + " chưa đăng ký";
                    }
                    
                    modelErrors.Add("Summary", new ModelStateError
                    {
                        ErrorMessages = errorMessage
                    });
                }
            }

            this.ViewBag.LoginModel = model;
            this.ViewBag.ModelStateErrorDictionary = modelErrors;
            return View();
        }

        [Route("/logout")]
        public async Task<IActionResult> LogOut()
        {
            await SignInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
