using Oracle.ManagedDataAccess.Client;
using ORACLE_ECOMERCE_2021.Controllers;
using Repository.DomainModels;
using Repository.Extentions;
using Repository.SqlDataProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ORACLE_ECOMERCE_2021.Models.ActionModels
{
    public class HomeIndexAction: ControllerAction
    {
        public List<Product> GetHotSearchProducts()
        {
            var options = new RankProductOptions()
            {
                method = RankProductMethod.rank_product_by_search
            };

            return new ProductController().RankProducts(options);
        }

        public List<Product> GetTrendingProducts()
        {
            var options = new RankProductOptions()
            {
                method = RankProductMethod.rank_product_trending
            };

            return new ProductController().RankProducts(options);
        }

        public List<Category> GetPopularCategories()
        {
            using (var conn = new OracleConnection(_connStr))
            using (var db = new OracleHelper(conn))
            {
                db.ExecuteReader("rank_category_popular", true);
                var result = db.FetchRowSet<Category>();
                return result;
            }
        }


        public List<Category> GetParentCategories()
        {
            using (var conn = new OracleConnection(_connStr))
            using (var _db = new OracleHelper(conn))
            {
                _db.ExecuteReader(" SELECT * FROM Categories WHERE parent_id = 0");
                List<Category> categories = _db.FetchRowSet<Category>();
                return categories;
            }
        }

        public int CountSearchResult(string searchText)
        {
            searchText = searchText.ToLower().Replace(" ", " AND ").LocDau();

            string query = @$"
                SElECT COUNT(*) FROM Products 
                WHERE CONTAINS ( computed_name, '{searchText}', 99) > 0 
            ";

            using (var conn = new OracleConnection(_connStr))
            using (var _db = new OracleHelper(conn))
            {
                _db.ExecuteReader(query);
                var result = _db.FetchVariableList<int>();
                return result[0][0];
            }
        }

        public List<Product> GetSearchResult(string searchText, int currentPage = 1)
        {
            searchText = searchText.ToLower().Replace(" ", " AND ").LocDau();

            string query = @$"
                SElECT * FROM Products 
                WHERE CONTAINS ( computed_name, '{searchText}', 99) > 0 
                ORDER BY score(99) DESC
                OFFSET {(currentPage - 1) * 30} ROWS
                FETCH NEXT 30 ROWS ONLY
            ";

            using (var conn = new OracleConnection(_connStr))
            using (var _db = new OracleHelper(conn))
            {
                _db.ExecuteReader(query);
                var result = _db.FetchRowSet<Product>();
                return result;
            }
        }
    }
}
