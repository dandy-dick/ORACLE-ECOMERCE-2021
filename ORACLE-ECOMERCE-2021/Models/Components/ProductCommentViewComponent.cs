using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Repository.DomainModels;
using Oracle.ManagedDataAccess.Client;
using Repository.SqlDataProvider;

namespace ORACLE_ECOMERCE_2021.Controllers
{
    public class ProductCommentViewComponent : ViewComponent
    {
        public ProductCommentViewComponent(){}

        public async Task<IViewComponentResult> 
            InvokeAsync(string productId, int rateAt = 5, int totalComment = 0)
        {
            return View("ProductComment", _getReview(productId, rateAt, totalComment));
        }

        public List<Review> _getReview(string productId, int rateAt, int totalComment)
        {
            var connStr = new ApplicationController().GetConnectionString();
            using (var conn = new OracleConnection(connStr))
            using (var _db = new OracleHelper(conn))
            {
                // get highest rate commented review
                //
                if (rateAt == null)
                {
                    string q = @$"
                    SELECT MAX(RATE) as FROM REVIEWS
                    WHERE commented = 'asd'";
                    _db.ExecuteReader(q);
                    var val = _db.FetchVariableList<int>();

                }

                var offset = totalComment == 0 ? 0 : totalComment + 10;
                string query = @$"
                    SELECT * FROM REVIEWS 
                    WHERE COMMENTED IS NOT NULL AND PRODUCT_ID = '{productId}'
                        AND ROUND(RATE) = {rateAt}
                    OFFSET {offset} ROWS FETCH NEXT 10 ROWS ONLY
                ";
            
                _db.ExecuteReader(query);
                return _db.FetchRowSet<Review>();
            }
        }

    }
}
