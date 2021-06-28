using System.Collections.Generic;

namespace Repository.DomainModels
{
    public class FeatureAttribute
    {
        public string NAME { get; set; }
        public string VALUE { get; set; }

        public FeatureAttribute(string name, string value)
        {
            NAME = name;
            VALUE = value;
        }
    }
    public class ProductFeature
    {
        public int PRODUCT_ID { get; set; }
        public string NAME { get; set; }
        public string ATTRIBUTES { get; set; }

        public List<FeatureAttribute> Attributes { get; set; }
    }
}
