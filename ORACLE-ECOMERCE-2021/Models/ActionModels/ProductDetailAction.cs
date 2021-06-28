using Oracle.ManagedDataAccess.Client;
using ORACLE_ECOMERCE_2021.Controllers;
using Repository.DomainModels;
using Repository.SqlDataProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ORACLE_ECOMERCE_2021.Models.ActionModels
{
    public class RateSummary
    {
        public int RATE_AT { get; set; }
        public float TOTAL { get; set; } = 0;
    }

    public class ProductDetailAction : ControllerAction
    {
        private ISQLHelper _db;

        public List<RateSummary> RateSummaries { get; set; }
        public Product ProductDetail { get; set; }

        public ProductDetailAction()
        {
            var conn = new OracleConnection(_connStr);
            _db = new OracleHelper(conn);
        }

        public List<Product> GetSimilarProducts()
        {
            // Get Products With Rank Popular Method
            //
            var controller = new ProductController();
            var options = new RankProductOptions
            {
                method = RankProductMethod.rank_product_popular,
                categoryId = ProductDetail.CATEGORY_ID,
                pageSize = 20
            };
            var products = controller.RankProducts(options);

            // Get & Set product's images
            //
            if (products != null)
            {
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
            }    

            return products;
        }

        public List<Category> GetCategories()
        {
            using (var conn = new OracleConnection(_connStr))
            using (var _db = new OracleHelper(conn))
            {
                _db.ExecuteReader(" SELECT * FROM Categories ");
                List<Category> categories = _db.FetchRowSet<Category>();
                return categories;
            }
        }

        public void Execute(string id)
        {
            this.ProductDetail = _getProduct(id);
            this.RateSummaries = _getRateSummaries(id);
            _updateProductViewCount(id);
           
        }

        private void _updateProductViewCount(string id)
        {
            string query = @$"
                UPDATE products
                SET views = 1 + COALESCE( products.views, 0 ),
                    views_counting = 1 + COALESCE(products.views_counting, 0)
                WHERE products.id = {id}
            ";
            _db.ExecuteNonQuery(query);
        }

        private Product _getProduct(string id)
        {
            string query = $"SELECT * FROM Products WHERE id = {id}";
            _db.ExecuteReader(query);
            var product = _db.FetchRowSet<Product>().First();

            if (product == null)
                return null;

            query = $"SELECT * FROM PRODUCT_IMAGES WHERE product_id = {id}";
            _db.ExecuteReader(query);
            product.ProductImages = _db.FetchRowSet<ProductImage>();

            // handle product images 
            query = $"SELECT * FROM PRODUCT_FEATURES WHERE product_id = {id}";
            _db.ExecuteReader(query);
            product.ProductFeatures = _db.FetchRowSet<ProductFeature>();

            // handle product features attributes
            if (product.ProductFeatures != null)
                foreach (var feature in product.ProductFeatures)
                {
                    feature.Attributes = new List<FeatureAttribute>();

                    var arr = feature.ATTRIBUTES.Split("//");
                    foreach (var item in arr)
                    {
                        var val = item.Split("==");
                        feature.Attributes.Add(new FeatureAttribute(val[0], val[1]));
                    }
                }
            // handle brand
            query = $"SELECT * FROM BRANDS WHERE id = {product.BRAND_ID}";
            _db.ExecuteReader(query);
            product.Brand = _db.FetchRowSet<Brand>().First();
            _db.Dispose();

            return product;
        }

        private List<RateSummary> _getRateSummaries(string id)
        {
            string connStr = new ApplicationController().GetConnectionString();
            using (var connn = new OracleConnection(connStr))
            using (var database = new OracleHelper(connn))
            {
                string query = @$"rate_summary";

                _db.ExecuteReader(query, true, new { 
                    p_product_id = id
                });

                var data = _db.FetchRowSet<RateSummary>();

                var RateSummary = new List<RateSummary>()
                {
                    new RateSummary() { RATE_AT = 1 },
                    new RateSummary() { RATE_AT = 2 },
                    new RateSummary() { RATE_AT = 3 },
                    new RateSummary() { RATE_AT = 4 },
                    new RateSummary() { RATE_AT = 5 }
                };

                if(data != null)
                {
                    foreach (var item in RateSummary)
                    {
                        var temp = data.FirstOrDefault(p => p.RATE_AT == item.RATE_AT);
                        if (temp != null)
                        {
                            item.TOTAL = temp.TOTAL;
                        }
                    }
                }
                

                return RateSummary;
            }
        }
    }
}
