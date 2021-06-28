using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Repository.DomainModels;

namespace ORACLE_ECOMERCE_2021.Models
{
    public class PaginationPartial
    {
        public string Container { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPage { get; set; }

    }
}
