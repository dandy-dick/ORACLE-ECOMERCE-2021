using System.Collections.Generic;

namespace Repository.DomainModels
{
    public class Product 
    {
        public int ID { get; set; }
        public int CATEGORY_ID { get; set; }
        public int BRAND_ID { get; set; }
        public string NAME { get; set; }
        public float UNIT_PRICE { get; set; }
        public float DISCOUNT { get; set; }
        public string URL_KEY { get; set; }
        public float RATE_AVG { get; set; }
        public int RATE_TOTAL { get; set; }
        public int RATE_TOTAL_RECENT { get; set; }
        public float RATE_AVG_RECENT { get; set; }
        public int RATE_TOTAL_COUNTING { get; set; }
        public float RATE_AVG_COUNTING { get; set; }
        public int VIEWS { get; set; }
        public int VIEWS_RECENT { get; set; }
        public int VIEWS_COUNTING { get; set; }
        public int PURCHASES { get; set; }
        public int PURCHASES_RECENT { get; set; }
        public int PURCHASES_COUNTING { get; set; }
        public int SEARCH_RECENT { get; set; }
        public int SEARCH_COUNTING { get; set; }
        public string THUMBNAIL_URL { get; set; }

        public Brand Brand { get; set; }

        public List<ProductImage> ProductImages { get; set; }
        public List<ProductFeature> ProductFeatures { get; set; }
    }
}
