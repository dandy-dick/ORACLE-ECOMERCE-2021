using System.Collections.Generic;

namespace Repository.DomainModels
{
    public class Category 
    {
        public int ID { get; set; }
        public string NAME { get; set; }
        public int PARENT_ID { get; set; }
        public int NUM_OF_ORDERS { get; set; }
        public string IS_LEAF { get; set; }
        public string URL { get; set; }

        public List<Category> Childs { get; set; } = new List<Category>();
    }

   
}
