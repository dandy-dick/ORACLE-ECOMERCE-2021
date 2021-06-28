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
    public class ShopIndexAction: ControllerAction
    {
        public string urlKey { get; set; }
        public int? categoryId { get; set; }
        public int[] brandIds { get; set; } = new int[] { };
        public int? currentPage { get; set; } = 1;
        public int? pageSize { get; set; } = 30;
        public int? minPrice { get; set; }
        public int? maxPrice { get; set; }
        public RankProductMethod method { get; set; } = RankProductMethod.rank_product_popular;

        public int totalPage { get; set; }

        public List<Category> GetCategories()
        {
            using (var conn = new OracleConnection(_connStr))
            using (var _db = new OracleHelper(conn))
            {
                _db.ExecuteReader(" SELECT * FROM Categories");
                List<Category> categories = _db.FetchRowSet<Category>();
                return categories;
            }
        }

        public List<Product> GetProducts()
        {
            // Get Products With Rank Popular Method
            //
            var controller = new ProductController();
            var options = new RankProductOptions();
            options.ObjectAssign(this);
            var products = controller.RankProducts(options);
            totalPage = options.totalPage;

            // Get & Set product's images
            //
            if (products != null)
            using (var conn = new OracleConnection(_connStr))
            using (var db = new OracleHelper(conn))
            {
                string query = "SELECT * FROM PRODUCT_IMAGES WHERE product_id ";
                query += $" IN( {string.Join(',', products.Select(p => p.ID))} )";
                db.ExecuteReader(query);
                var imgs = db.FetchRowSet<ProductImage>();

                foreach (var p in products)
                {
                    p.ProductImages = imgs.Where(i => i.PRODUCT_ID == p.ID).ToList();
                }
            }

            return products;
        }

        public List<Brand> GetPopularBrands()
        {
            string query = $@"
                WITH BrandIds AS (
                    SELECT      brand_id, SUM( purchases ) as sold
                    FROM        Products
                    WHERE       category_id = {categoryId}
                    GROUP BY    brand_id
                    FETCH NEXT 10 ROWS ONLY
                )
                SELECT      * 
                FROM        BrandIds
                JOIN        Brands     ON BrandIds.brand_id = Brands.id
            ";

            using (var conn = new OracleConnection(_connStr))
            using (var db = new OracleHelper(conn))
            {
                db.ExecuteReader(query);
                return db.FetchRowSet<Brand>();
            }
        }
    }
}
