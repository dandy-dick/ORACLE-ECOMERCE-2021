using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using ORACLE_ECOMERCE_2021.Models;
using Repository.DomainModels;
using Repository.SqlDataProvider;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ORACLE_ECOMERCE_2021.Controllers
{
    public enum RankProductMethod
    {
        rank_product_by_search = 1,
        rank_product_trending = 2,
        rank_product_popular = 3,
        rank_product_newest = 4,
        rank_product_bestseller = 5
    }

    public class RankProductOptions
    {
        public RankProductMethod method { get; set; }
        public int currentPage { get; set; } = 1;

        public int? pageSize { get; set; }
        public int? categoryId { get; set; }

        public int[] brandIds { get; set; } = new int[] { };
        public int? minPrice { get; set; }
        public int? maxPrice { get; set; }

        public int totalPage { get; set; }

        public dynamic test(string _connString)
        {


            using (var conn = new OracleConnection(_connString))
            using (var db = new OracleHelper(conn))
            {
                string query = "Select * From Products";
                db.ExecuteReader(query);
                var result = db.FetchRowSet<Product>();
                return result;
            }

        }
    }

    public class ProductController : ApplicationController
    {
        [Route("/products/rank")]
        public List<Product> RankProducts(RankProductOptions options)
        {
            return options.method switch
            {
                RankProductMethod.rank_product_trending => rank_product_trending(options),
                RankProductMethod.rank_product_popular => rank_product_popular(options),
                RankProductMethod.rank_product_newest => rank_product_newest(options),
                RankProductMethod.rank_product_bestseller => rank_product_bestseller(options),
                _ => rank_product_by_search(options),
            };
        }

        private List<Product> rank_product_trending(RankProductOptions options)
        {
            // SET DEFAULT VALUE
            options.pageSize = options.pageSize ?? 10;

            using (var conn = new OracleConnection(_connString))
            using (var db = new OracleHelper(conn))
            {
                var query = "rank_product_trending";
                db.ExecuteReader(query, true, new
                {
                    v_skip = (options.currentPage - 1) * options.pageSize,
                    v_fetch_size = options.pageSize
                });

                var result = db.FetchRowSet<Product>();
                return result;
            }
        }

        private List<Product> rank_product_by_search(RankProductOptions options)
        {
            // SET DEFAULT VALUE
            options.pageSize = options.pageSize ?? 10;

            using (var conn = new OracleConnection(_connString))
            using (var db = new OracleHelper(conn))
            {
                var query = "rank_product_by_search";
                db.ExecuteReader(query, true, new
                {
                    v_skip = (options.currentPage - 1) * options.pageSize,
                    v_fetch_size = options.pageSize
                });

                var result = db.FetchRowSet<Product>();
                return result;
            }
        }

        private List<Product> rank_product_popular(RankProductOptions options)
        {
            // SET DEFAULT VALIES
            options.pageSize = options.pageSize ?? 30;

            using (var conn = new OracleConnection(_connString))
            using (var db = new OracleHelper(conn))
            {
                string where = "";

                if (options.categoryId != null)
                {
                    if (where.Length == 0)
                        where = " WHERE ";

                    where += @$" 
                        category_id = {options.categoryId} ";
                }
                if (options.brandIds != null && options.brandIds.Length > 0)
                {
                    if (where.Length == 0)
                        where = " WHERE ";
                    if (where.Length > 0)
                        where += "  AND  ";

                    where += @$" 
                        brand_id IN ({ string.Join(',' , options.brandIds) }) ";
                }
                if (options.minPrice != null)
                {
                    if (where.Length == 0)
                        where = " WHERE ";
                    if (where.Length > 0)
                        where += "  AND  ";

                    where += @$" 
                        min_of(UNIT_PRICE, DISCOUNT) >= {options.minPrice} ";
                }
                if (options.maxPrice != null)
                {
                    if (where.Length == 0)
                        where = " WHERE ";
                    if (where.Length > 0)
                        where += "  AND  ";

                    where += @$" 
                        min_of(UNIT_PRICE, DISCOUNT) <= {options.maxPrice} ";
                }

                string countQuery = @$"
                    SELECT COUNT(*) FROM PRODUCTS
                    {where}       
                ";

                string selectQuery = @$"
                    SELECT PRODUCTS.*, 
                            weight_rank_popular_product(purchases, views, rate_avg) as weight   
                    FROM PRODUCTS
                    {where}       
                    ORDER BY weight
                    OFFSET {(options.currentPage - 1) * options.pageSize} ROWS
                    FETCH NEXT {options.pageSize} ROWS ONLY
                ";

                // count total page
                db.ExecuteReader(countQuery);
                var totalProduct = db.FetchVariableList<int>()[0][0];
                options.totalPage = (int)Math.Ceiling((float)totalProduct / (float)options.pageSize);

                // get products
                db.ExecuteReader(selectQuery);
                var result = db.FetchRowSet<Product>();
                return result;
            }
        }

        private List<Product> rank_product_newest(RankProductOptions options)
        {
            // SET DEFAULT VALIES
            options.pageSize = options.pageSize ?? 30;

            using (var conn = new OracleConnection(_connString))
            using (var db = new OracleHelper(conn))
            {
                string where = "";

                if (options.categoryId != null)
                {
                    if (where.Length == 0)
                        where = " WHERE ";

                    where += @$" 
                        category_id = {options.categoryId} ";
                }
                if (options.brandIds != null && options.brandIds.Length > 0)
                {
                    if (where.Length == 0)
                        where = " WHERE ";
                    if (where.Length > 0)
                        where += "  AND  ";

                    where += @$" 
                        brand_id IN ({ string.Join(',', options.brandIds) }) ";
                }
                if (options.minPrice != null)
                {
                    if (where.Length == 0)
                        where = " WHERE ";
                    if (where.Length > 0)
                        where += "  AND  ";

                    where += @$" 
                        min_of(UNIT_PRICE, DISCOUNT) >= {options.minPrice} ";
                }
                if (options.maxPrice != null)
                {
                    if (where.Length == 0)
                        where = " WHERE ";
                    if (where.Length > 0)
                        where += "  AND  ";

                    where += @$" 
                        min_of(UNIT_PRICE, DISCOUNT) <= {options.maxPrice} ";
                }

                string countQuery = @$"
                    SELECT COUNT(*) FROM PRODUCTS
                    {where}       
                ";

                string selectQuery = @$"
                    SELECT PRODUCTS.*   
                    FROM PRODUCTS
                    {where}       
                    ORDER BY added_date
                    OFFSET {(options.currentPage - 1) * options.pageSize} ROWS
                    FETCH NEXT {options.pageSize} ROWS ONLY
                ";

                // count total page
                db.ExecuteReader(countQuery);
                var totalProduct = db.FetchVariableList<int>()[0][0];
                options.totalPage = (int)Math.Ceiling((float)totalProduct / (float)options.pageSize);

                // get products
                db.ExecuteReader(selectQuery);
                var result = db.FetchRowSet<Product>();
                return result;
            }
        }

        private List<Product> rank_product_bestseller(RankProductOptions options)
        {
            // SET DEFAULT VALIES
            options.pageSize = options.pageSize ?? 30;

            using (var conn = new OracleConnection(_connString))
            using (var db = new OracleHelper(conn))
            {
                string where = "";

                if (options.categoryId != null)
                {
                    if (where.Length == 0)
                        where = " WHERE ";

                    where += @$" 
                        category_id = {options.categoryId} ";
                }
                if (options.brandIds != null && options.brandIds.Length > 0)
                {
                    if (where.Length == 0)
                        where = " WHERE ";
                    if (where.Length > 0)
                        where += "  AND  ";

                    where += @$" 
                        brand_id IN ({ string.Join(',', options.brandIds) }) ";
                }
                if (options.minPrice != null)
                {
                    if (where.Length == 0)
                        where = " WHERE ";
                    if (where.Length > 0)
                        where += "  AND  ";

                    where += @$" 
                        min_of(UNIT_PRICE, DISCOUNT) >= {options.minPrice} ";
                }
                if (options.maxPrice != null)
                {
                    if (where.Length == 0)
                        where = " WHERE ";
                    if (where.Length > 0)
                        where += "  AND  ";

                    where += @$" 
                        min_of(UNIT_PRICE, DISCOUNT) <= {options.maxPrice} ";
                }

                string countQuery = @$"
                    SELECT COUNT(*) FROM PRODUCTS
                    {where}       
                ";

                string selectQuery = @$"
                    SELECT PRODUCTS.*   
                    FROM PRODUCTS
                    {where}       
                    ORDER BY purchases DESC
                    OFFSET {(options.currentPage - 1) * options.pageSize} ROWS
                    FETCH NEXT {options.pageSize} ROWS ONLY
                ";

                // count total page
                db.ExecuteReader(countQuery);
                var totalProduct = db.FetchVariableList<int>()[0][0];
                options.totalPage = (int)Math.Ceiling((float)totalProduct / (float)options.pageSize);

                // get products
                db.ExecuteReader(selectQuery);
                var result = db.FetchRowSet<Product>();
                return result;




            }
        }

    }
}
