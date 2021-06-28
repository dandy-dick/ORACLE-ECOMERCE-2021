using System.Collections.Generic;

namespace Repository.DomainModels
{
    public class Review 
    {
        public int PRODUCT_ID { get; set; }
        public int CUSTOMER_ID { get; set; }
        public float RATE { get; set; }
        public string COMMENTED { get; set; }

        public string AUTHOR { get; set; } = "Chưa đăng ký";
    }
}
