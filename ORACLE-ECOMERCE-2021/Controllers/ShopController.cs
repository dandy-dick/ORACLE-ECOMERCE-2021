using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using ORACLE_ECOMERCE_2021.Models;
using ORACLE_ECOMERCE_2021.Models.ActionModels;
using Repository.DomainModels;
using Repository.Extentions;
using Repository.SqlDataProvider;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ORACLE_ECOMERCE_2021.Controllers
{
    public class ShopController : ApplicationController
    {
        private UserManager<Customer> _userManager { get; set; }
        private SignInManager<Customer> _signInManager { get; set; }

        public ShopController(
            UserManager<Customer> userManager, SignInManager<Customer> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [Route("/danh-muc/{urlKey}/{categoryId}")]
        public IActionResult Index(ShopIndexAction model)
        {
            var tree = model.GetCategories();
            ViewBag.Categories = tree;
            ViewBag.ParentCategories = tree.Where(p => p.PARENT_ID == 0).ToList();
            ViewBag.Category = tree.FirstOrDefault(p => p.ID == model.categoryId);

            ViewBag.Products = model.GetProducts();
            ViewBag.CurrentPage = model.currentPage;
            ViewBag.TotalPage = model.totalPage;
            ViewBag.PopularBrands = model.GetPopularBrands();

            var options = new RankProductOptions();
            options.ObjectAssign(model);
            ViewBag.Options = options;

            return View();
        }

        private List<CartItem> _getCartItems()
        {
            string cookie = Request.Cookies["shoppingCart"];

            var cartItems = new List<CartItem>();
            if (cookie != null && cookie.Length > 0)
            {
                string[] values;
                values = cookie.Split(':');

                int i = 0;
                while (i < values.Length)
                {
                    cartItems.Add(new CartItem()
                    {
                        ProductId = values[i],
                        Quantity = Convert.ToInt32(values[i+1])
                    });

                    i += 2;
                }
            }


            var ids = cartItems.Select(p => p.ProductId).ToArray();
            if (ids.Length != 0)
            {
                var action = new ShoppingCartAction();
                var products = action.Execute(ids);
                foreach (var item in cartItems)
                {
                    var p = products.First(p => "" + p.ID == item.ProductId);
                    item.Product = p;
                }
            }

            return cartItems;
        }

        [HttpGet]
        [Route("/chi-tiet")]
        public IActionResult ProductDetail(string product_id)
        {
            var action = new ProductDetailAction();
            action.Execute(product_id);

            var tree = action.GetCategories();
            ViewBag.Categories = tree;
            ViewBag.ParentCategories = tree.Where(p => p.PARENT_ID == 0).ToList();
            ViewBag.Product = action.ProductDetail;
            ViewBag.RateSummaries = action.RateSummaries;
            ViewBag.SimilarProducts = action.GetSimilarProducts();
            return View();
        }

        [Route("/gio-hang")]
        public IActionResult ShoppingCart()
        {
            var model = new ShopIndexAction();
            var tree = model.GetCategories();
            
            ViewBag.Categories = tree;
            ViewBag.ParentCategories = tree.Where(p => p.PARENT_ID == 0).ToList();
            ViewBag.Category = tree.FirstOrDefault(p => p.ID == model.categoryId);

            var options = new RankProductOptions();
            options.ObjectAssign(model);
            ViewBag.Options = options;
            return View(_getCartItems());
        }

        [Route("/dat-hang")]
        public IActionResult MakeOrder()
        {
            var model = new ShopIndexAction();
            var tree = model.GetCategories();

            ViewBag.Categories = tree;
            ViewBag.ParentCategories = tree.Where(p => p.PARENT_ID == 0).ToList();
            ViewBag.Category = tree.FirstOrDefault(p => p.ID == model.categoryId);

            var options = new RankProductOptions();
            options.ObjectAssign(model);
            ViewBag.Options = options;
            return View(_getCartItems());
        }

        [Route("/don-hang-thanh-cong")]
        public IActionResult OrderComplete()
        {
            var model = new ShopIndexAction();
            var tree = model.GetCategories();

            ViewBag.Categories = tree;
            ViewBag.ParentCategories = tree.Where(p => p.PARENT_ID == 0).ToList();
            ViewBag.Category = tree.FirstOrDefault(p => p.ID == model.categoryId);

            var options = new RankProductOptions();
            options.ObjectAssign(model);
            ViewBag.Options = options;

            var action = new OrderCompleteAction();
            action.Execute(_getCartItems());
            Response.Cookies.Delete("shoppingCart");
            return View();
        }

        public object LoadComment(string productId, int totalComment, int rateAt)
        {
            var result = new ProductCommentViewComponent()
                ._getReview( productId, rateAt, totalComment);

            if (result == null)
                result = new List<Review>();

            return new {
                success = true,
                comments = result,
                totalComment = result.Count
            };
        }

        [HttpPost]
        public object PostReview(string productId, int rate, string commented)
        {
            string customerId = null,
                customerName = "Chưa đăng ký";
            if (_signInManager.IsSignedIn(User))
            {
                customerId = _userManager.GetUserId(User);
                Customer customer = _userManager.Users.FirstOrDefault(p => p.Id == customerId);
                customerName = customer.UserName;
            }

            using (var conn = new OracleConnection(_connString))
            using (var db = new OracleHelper(conn))
            {
                string query;
                if (customerId == null)
                {
                    query = $@"
                        INSERT INTO REVIEWS (product_id, rate, commented) 
                        WITH recs AS (
                            SELECT {productId} as product_id, 
                                   {rate} as rate, 
                                   '{commented??""}' as commented
                            FROM DUAL
                        )
                        SELECT * FROM recs
                    ";
                }
                else
                {
                    query = $@"
                        INSERT INTO REVIEWS (customer_id, product_id, rate, commented) 
                        WITH recs AS (
                            SELECT {customerId} as customer_id, 
                                   {productId} as product_id, 
                                   {rate} as rate, 
                                   '{commented}' as commented
                            FROM DUAL
                        )
                        SELECT * FROM recs
                    ";
                }
                db.ExecuteNonQuery(query);

                return new
                {
                    success = true,
                    author = customerName
                };
            }
        }
    }
}
