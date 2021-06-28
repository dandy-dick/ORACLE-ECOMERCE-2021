using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using ORACLE_ECOMERCE_2021.Controllers;
using Repository.DomainModels;

namespace TheAchEcom.Models
{

    public class PageMaster
    {
        private HttpContext Context;
        private SignInManager<Customer> SignInManager;
        private UserManager<Customer> UserManager;

        public PageMaster(
            IHttpContextAccessor httpContextAccessor,
            SignInManager<Customer> signInManager,
            UserManager<Customer> userManager)
        {
            Context = httpContextAccessor.HttpContext;
            SignInManager = signInManager;
            UserManager = userManager;
        }

        //private EcomRepository _repo = new EcomRepository();
        //private const string _cartCookieName = "_cartCookieName";
        //public ShoppingCart GetShoppingCart()
        //{
        //    ShoppingCart cart;
        //    if (SignInManager.IsSignedIn(Context.User))
        //    {
        //        string cartId = UserManager.GetUserId(Context.User);
        //        cart = _repo.GetCartById(cartId);


        //        if (cart == null)
        //        {
        //            var newCart = new ShoppingCart
        //            {
        //                Id = UserManager.GetUserId(Context.User)
        //            };
        //            _repo.ShoppingCart_Add(newCart);
        //            cart = newCart;
        //        }
        //    }
        //    else
        //    {
        //        try
        //        {
        //            string requestCookie = Context.Request.Cookies[_cartCookieName];
        //            cart = JsonConvert.DeserializeObject<ShoppingCart>(requestCookie);
        //        }
        //        catch (Exception)
        //        {
        //            cart = new ShoppingCart()
        //            {
        //                Id = Guid.NewGuid().ToString(),
        //                CartProducts = new List<CartProduct>()
        //            };
        //            Context.Response.Cookies
        //                .Append(_cartCookieName, JsonConvert.SerializeObject(cart));
        //        }
        //    }

        //    return cart;
        //}
    }
}
