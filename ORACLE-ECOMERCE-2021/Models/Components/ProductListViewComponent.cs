using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Repository.DomainModels;

namespace ORACLE_ECOMERCE_2021.Controllers
{
    public class ProductListViewComponent: ViewComponent
    {
        public ProductListViewComponent()
        {   
        }

        public async Task<IViewComponentResult> 
            InvokeAsync(List<Product> products)
        {
            return View(products);
        }

    }
}
