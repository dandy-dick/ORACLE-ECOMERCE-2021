using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using ORACLE_ECOMERCE_2021.Models;
using ORACLE_ECOMERCE_2021.Models.ActionModels;
using Repository.DomainModels;
using Repository.SqlDataProvider;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ORACLE_ECOMERCE_2021.Controllers
{
    public class HomeController : ApplicationController
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var action = new HomeIndexAction();

            ViewBag.HotSearchProducts = action.GetHotSearchProducts();
            ViewBag.TrendingProducts = action.GetTrendingProducts();
            ViewBag.PopularCategories = action.GetPopularCategories();

            var tree = action.GetParentCategories();
            ViewBag.ParentCategories = tree;

            return View();
        }

        [Route("/search")]
        public IActionResult SearchResult(string searchText, int currentPage = 1)
        {
            var action = new HomeIndexAction();
            var tree = action.GetParentCategories();

            ViewBag.CurrentPage = currentPage;
            ViewBag.SearchText = searchText;
            ViewBag.SearchTotal = action.CountSearchResult(searchText);
            ViewBag.SearchResult = action.GetSearchResult(searchText, currentPage);
            ViewBag.ParentCategories = tree;

            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [HttpPost]
        public IActionResult PopularCategoryProducts(int categoryId)
        {
            return ViewComponent("ProductGrid", new { categoryId });
        }

        [HttpPost]
        public IActionResult MegaMenu(string parent_id)
        {
            using (var conn = new OracleConnection(_connString))
            using( var db = new OracleHelper(conn))
            {
                db.ExecuteReader($"SELECT * FROM CATEGORIES WHERE PARENT_ID = {parent_id}");
                var model = db.FetchRowSet<Category>();

                return PartialView("_MegaMenuPartial", model);
            }
        }
    }
}
