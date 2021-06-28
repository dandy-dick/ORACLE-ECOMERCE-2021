using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Repository.DomainModels;

namespace ORACLE_ECOMERCE_2021.Controllers
{
    public class ProductGridViewComponent: ViewComponent
    {

        public async Task<IViewComponentResult> 
            InvokeAsync(int categoryId)
        {
            var controller = new ProductController();
            var options = new RankProductOptions()
            { 
                method = RankProductMethod.rank_product_popular,
                categoryId = categoryId
            };


            return View( controller.RankProducts(options) );
        }

    }
}
